using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random; // Para evitar ambiguidade com System.Random

[RequireComponent(typeof(NavMeshAgent))] // Garante que o componente NavMeshAgent exista
public class DogMovement : MonoBehaviour
{
    // --- Configurações ---
    [Header("Referências")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> destinationNodes; // Renomeado para clareza
    [SerializeField] private GameObject waypointsParent;

    [Header("Comportamento de Wander")]
    [Tooltip("Tempo que o cão espera em um nó antes de ir para o próximo")]
    [SerializeField] private float waitAtNodeDuration = 1.5f;

    [Header("Comportamento de Follow")]
    [Tooltip("Distância que o cão tenta manter do jogador ao seguir")]
    [SerializeField] private float followSafeDistance = 3.0f;
    [Tooltip("Quão perto o cão precisa estar para parar de seguir ativamente")]
    [SerializeField] private float followStopThreshold = 0.5f; // Margem perto da safeDistance
    [Tooltip("Tempo máximo (em segundos) que o cão seguirá o jogador antes de voltar a perambular")]
    [SerializeField] private float maxFollowDuration = 10.0f; // Novo: Tempo limite para seguir

    [Header("Comportamento de Flee")]
    [Tooltip("Distância que o cão tenta alcançar ao fugir")]
    [SerializeField] private float fleeDistance = 10.0f;
    // Removido: fleeDuration - A fuga agora termina ao chegar ou por trigger externo

    [Header("Controle")]
    [Tooltip("Permite que o cão execute comportamentos autônomos como Wander")]
    public bool canAutoMove = true;

    // --- Componentes ---
    private NavMeshAgent agent;
    private Animator animator; // Adicionado para futuras animações

    // --- Estado Interno ---
    private enum DogState
    {
        Idle,
        MovingToDestination,
        WaitingAtDestination,
        FollowingPlayer,
        Fleeing
    }
    [SerializeField] // Expor no Inspector para Debug
    private DogState currentState = DogState.Idle;
    private Coroutine currentBehaviorCoroutine = null; // Referência para coroutines de espera, follow ou flee
    private Vector3 currentDestination; // O destino atual sendo buscado

    // --- Inicialização ---
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Obter Animator se existir

        if (agent == null) Debug.LogError("NavMeshAgent não encontrado!", this);
        //if (animator == null) Debug.LogWarning("Animator não encontrado!", this); // Aviso opcional

        // Garante que a rotação seja controlada manualmente ou via Animator
        if (agent != null) agent.updateRotation = false;
    }

    private void Start()
    {

        foreach (Transform child in waypointsParent.transform)
        {
            destinationNodes.Add(child);
        }
        // Inicia o comportamento baseado no estado inicial e canAutoMove
        if (canAutoMove)
        {
            ChangeState(DogState.MovingToDestination); // Começa a perambular
        }
        else
        {
            ChangeState(DogState.Idle);
        }
    }

    // --- Loop Principal (Gerenciamento de Estado) ---
    private void Update()
    {
        // Lógica que roda continuamente para cada estado
        UpdateCurrentStateLogic();
        UpdateAnimations();
    }

    // --- Gerenciamento de Transição de Estado ---
    private void ChangeState(DogState newState)
    {
        if (currentState == newState) return; // Não faz nada se já está no estado

        // Lógica de SAÍDA do estado ATUAL
        ExitCurrentStateLogic();

        currentState = newState;
        //Debug.Log($"Entrando no estado: {currentState}"); // Para Debug

        // Lógica de ENTRADA no NOVO estado
        EnterCurrentStateLogic();
    }

    // --- Lógica de Entrada/Saída/Update dos Estados ---

    private void EnterCurrentStateLogic()
    {
        switch (currentState)
        {
            case DogState.Idle:
                StopAgentMovement(); // Garante que o agente esteja parado
                break;

            case DogState.MovingToDestination:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; } // Segurança
                currentDestination = GetRandomNavMeshNodePosition();
                SetAgentDestination(currentDestination);
                break;

            case DogState.WaitingAtDestination:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; } // Segurança
                StopAgentMovement(); // Para no local antes de esperar
                StartWaitTimer(waitAtNodeDuration); // Inicia a espera (usando a coroutine genérica)
                break;

            case DogState.FollowingPlayer:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; } // Segurança
                StartFollowTimer(maxFollowDuration); // Inicia timer para parar de seguir
                break;

            case DogState.Fleeing:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; } // Segurança
                StartFleeing(); // Inicia a lógica de fuga
                // A fuga agora termina ao chegar ao destino ou por comando externo
                break;
        }
    }

    private void UpdateCurrentStateLogic()
    {
        // Checagem prioritária: se não pode se mover autonomamente, força o Idle
        if (!canAutoMove && currentState != DogState.Idle && Time.timeScale != 0)
        {
            ChangeState(DogState.Idle);
            return; // Sai do Update após forçar o Idle
        }

        switch (currentState)
        {
            case DogState.Idle:
                // Se pode se mover autonomamente, talvez devesse voltar a perambular?
                if (canAutoMove)
                {
                    // Decide se volta a perambular ou fica Idle por mais tempo
                    // Ex: ChangeState(DogState.MovingToDestination);
                }
                break;

            case DogState.MovingToDestination:
                // Verifica se chegou ao destino do nó
                if (HasAgentArrived())
                {
                   // Debug.Log("Chegou ao nó de destino."); // Para Debug
                    ChangeState(DogState.WaitingAtDestination); // Muda para o estado de espera
                }
                break;

            case DogState.WaitingAtDestination:
                // A Coroutine genérica está cuidando da espera, não faz nada aqui
                break;

            case DogState.FollowingPlayer:
                UpdateFollowPlayerPosition(); // Atualiza o destino continuamente
                // O timer iniciado em EnterCurrentStateLogic cuidará da saída após maxFollowDuration
                break;

            case DogState.Fleeing:
                // Verifica se chegou ao ponto de fuga
                if (HasAgentArrived())
                {
                   // Debug.Log("Chegou ao ponto de fuga."); // Para Debug
                    // Decide o que fazer após fugir (voltar a perambular, ficar idle etc.)
                    ChangeState(DogState.MovingToDestination); // Ex: Volta a perambular
                }
                // Opcional: Se o jogador se aproximar muito durante a fuga, recalcular destino
                // else if (IsPlayerTooCloseWhileFleeing()) { StartFleeing(); }
                break;
        }
    }

    private void ExitCurrentStateLogic()
    {
        // Limpeza ao sair de um estado
        StopCurrentBehaviorCoroutine(); // Para qualquer coroutine de timer (espera, follow, flee)
        switch (currentState)
        {
             // Adicione limpeza específica para outros estados se necessário
            case DogState.FollowingPlayer:
            case DogState.Fleeing:
                // Talvez parar o movimento ao sair desses estados? Depende do fluxo desejado
                // StopAgentMovement();
                break;
        }
    }

    // --- Lógica Específica dos Comportamentos (Timers e Coroutines) ---

    private void StartWaitTimer(float duration)
    {
        StopCurrentBehaviorCoroutine(); // Garante que não haja coroutines duplicadas
        currentBehaviorCoroutine = StartCoroutine(WaitCoroutine(duration));
    }

    private void StartFollowTimer(float duration)
    {
        StopCurrentBehaviorCoroutine();
        currentBehaviorCoroutine = StartCoroutine(FollowTimerCoroutine(duration));
    }

    // Não precisamos mais de StartFleeTimer, a fuga termina ao chegar ou por comando

    private void StopCurrentBehaviorCoroutine()
    {
        if (currentBehaviorCoroutine != null)
        {
            StopCoroutine(currentBehaviorCoroutine);
            currentBehaviorCoroutine = null;
        }
    }

    // Coroutine para a espera simples em nós
    private IEnumerator WaitCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentBehaviorCoroutine = null; // Limpa a referência

        // Só transiciona se ainda estiver no estado de espera e puder se mover
        if (currentState == DogState.WaitingAtDestination && canAutoMove)
        {
           // Debug.Log("Tempo de espera finalizado, movendo para próximo nó."); // Para Debug
            ChangeState(DogState.MovingToDestination); // Volta a mover
        }
    }

    // Coroutine para o tempo limite de seguir o jogador
    private IEnumerator FollowTimerCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentBehaviorCoroutine = null; // Limpa a referência

        // Só transiciona de volta para Wander se ainda estiver seguindo e puder se mover
        if (currentState == DogState.FollowingPlayer && canAutoMove)
        {
           // Debug.Log("Tempo máximo de seguir atingido, voltando a perambular."); // Para Debug
            ChangeState(DogState.MovingToDestination); // Volta a perambular
        }
    }


    // --- Lógica Específica dos Comportamentos (Movimento) ---

    private void UpdateFollowPlayerPosition()
    {
        if (playerTransform == null || !canAutoMove)
        {
            ChangeState(DogState.Idle); // Se perder o jogador ou não puder mover, fica Idle
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 directionToPlayer = transform.position - playerTransform.position; // Vetor do jogador para o cão

        // Decide se precisa ajustar a posição
        bool needsToMove = false;
        Vector3 targetPosition = transform.position; // Posição padrão é ficar parado

        if (distanceToPlayer > followSafeDistance)
        {
            // Longe demais: move em direção ao jogador
            targetPosition = playerTransform.position;
            needsToMove = true;
           // Debug.Log("Seguindo: Longe demais, aproximando."); // Para Debug
        }
        else if (distanceToPlayer < (followSafeDistance - followStopThreshold))
        {
            // Perto demais: recua para a distância segura
            targetPosition = playerTransform.position + directionToPlayer.normalized * followSafeDistance;
            needsToMove = true;
           // Debug.Log("Seguindo: Perto demais, recuando."); // Para Debug
        }
        // Se estiver dentro da zona segura (entre safeDistance - threshold e safeDistance), não precisa mover ativamente

        // Define o destino apenas se precisar se mover E o destino mudou significativamente
        if (needsToMove)
        {
            // Verifica se o novo destino é diferente o suficiente do destino atual do agente
            // para evitar spam de SetDestination
            if (!agent.hasPath || Vector3.Distance(agent.destination, targetPosition) > 0.1f)
            {
                SetAgentDestination(targetPosition);
            }
        }
        else if(agent.hasPath) // Se não precisa mover, mas estava se movendo, para.
        {
            StopAgentMovement();
           // Debug.Log("Seguindo: Na distância correta, parado."); // Para Debug
        }
    }

    private void StartFleeing()
    {
        if (playerTransform == null || !canAutoMove)
        {
            ChangeState(DogState.Idle);
            return;
        }

        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        // Define um ponto de fuga um pouco além da fleeDistance para garantir
        Vector3 fleeTargetAttempt = transform.position + directionAwayFromPlayer * (fleeDistance * 1.2f);

        // Encontra um ponto válido no NavMesh para ir
        if (NavMesh.SamplePosition(fleeTargetAttempt, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
        {
            SetAgentDestination(hit.position);
           // Debug.Log($"Fugindo para: {hit.position}"); // Para Debug
        }
        else
        {
            // Se não encontrar ponto para fugir, talvez tentar outra coisa ou ficar Idle
            Debug.LogWarning("Não foi possível encontrar ponto de fuga válido no NavMesh.", this);
            ChangeState(DogState.Idle); // Ou Wander? Depende do que faz mais sentido
        }
    }


    // --- Funções Auxiliares de Movimento e Verificação ---

    private void SetAgentDestination(Vector3 destination)
    {
        if (agent != null && agent.enabled)
        {
            currentDestination = destination; // Guarda o destino atual
            agent.SetDestination(destination);
        }
    }

    private void StopAgentMovement()
    {
        if (agent != null && agent.enabled && agent.hasPath)
        {
            agent.ResetPath();
        }
        // Parar a coroutine de comportamento atual ao parar o movimento
        StopCurrentBehaviorCoroutine();
    }

    private bool HasAgentArrived()
    {
        if (agent == null || !agent.enabled || agent.pathPending || !agent.hasPath)
        {
            return false;
        }
        // Considera a stoppingDistance do agente para a chegada
        return agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.01f;
    }

    private Vector3 GetRandomNavMeshNodePosition()
    {
        if (destinationNodes == null || destinationNodes.Count == 0)
        {
            Debug.LogWarning("Lista destinationNodes está vazia. Usando posição atual.", this);
            return transform.position;
        }

        Vector3 randomPos = destinationNodes[Random.Range(0, destinationNodes.Count)].position;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            Debug.LogWarning($"Não foi possível encontrar ponto no NavMesh perto do nó {randomPos}. Usando posição atual.", this);
            return transform.position;
        }
    }

    // --- Controle Externo (Chamado por outros Scripts) ---

    public void RequestFollowPlayer()
    {
        if (!canAutoMove) SetAutonomousMovement(true); // Permite seguir mesmo se não estiver auto
        ChangeState(DogState.FollowingPlayer);
    }

    public void RequestFlee()
    {
        if (!canAutoMove) SetAutonomousMovement(true); // Permite fugir mesmo se não estiver auto
        ChangeState(DogState.Fleeing);
    }

    public void RequestStopAndIdle()
    {
        SetAutonomousMovement(false); // Força o estado Idle no próximo Update
    }

    public void RequestResumeWander()
    {
        SetAutonomousMovement(true); // Permite voltar a perambular no próximo Update (se estava Idle)
         // Se já estiver em outro estado auto, pode não fazer nada ou forçar Wander
        if (currentState == DogState.Idle)
        {
            ChangeState(DogState.MovingToDestination);
        }
    }

    // Método central para habilitar/desabilitar comportamento autônomo
    public void SetAutonomousMovement(bool enabled)
    {
        if (canAutoMove == enabled) return; // Sem mudança

        canAutoMove = enabled;
       // Debug.Log($"Movimento Autônomo: {canAutoMove}"); // Para Debug

        // O Update() agora reagirá a essa mudança
        if (!enabled)
        {
            // Força a transição para Idle imediatamente se desativado
            ChangeState(DogState.Idle);
        }
        else if (currentState == DogState.Idle) // Se estava Idle e foi reativado
        {
             // Começa a perambular novamente
             ChangeState(DogState.MovingToDestination);
        }
        // Se estava em outro estado (ex: Following) e canAutoMove é reativado,
        // a lógica do Update daquele estado pode decidir o que fazer
        // (continuar seguindo, voltar a perambular, etc.)
    }

    private void UpdateAnimations()
    {
        if (animator == null || agent == null) return;

        // 1. Verificar se o agente está se movendo
        float speed = agent.velocity.magnitude;
        bool isMoving = speed > 0.1f;

        // (Opcional) Se tiver o parâmetro IsMoving:
        // animator.SetBool(IS_MOVING_PARAM, isMoving);

        bool isWalkingLeft = false;
        bool isWalkingRight = false;

        // 2. Se estiver se movendo, determinar a direção relativa
        if (isMoving)
        {
            // Direção que o agente está se movendo
            Vector3 velocity = agent.velocity;
            // Direção para a qual o agente está virado
            Vector3 forward = transform.forward;

            // Calcula o ângulo entre a direção de movimento e a direção que está virado
            // Usando Vector3.up como eixo (plano horizontal XZ)
            float angle = Vector3.SignedAngle(forward, velocity, Vector3.up);

            // Se o ângulo é positivo, está movendo para a direita relativa
            if (angle > 0)
            {
                isWalkingRight = true;
            }
            // Se o ângulo é negativo (ou zero), está movendo para a esquerda relativa (ou reto)
            else // angle <= 0
            {
                isWalkingLeft = true;
            }
        }
        else{
            animator.SetBool("Idle", true);
        }
        // else: Não está se movendo, ambos os booleanos permanecem false,
        //       o Animator deve transicionar para Idle (se configurado corretamente).

        // 3. Atualizar os parâmetros do Animator
        animator.SetBool("IsWalkingLeft", isWalkingLeft);
        animator.SetBool("IsWalkingRight", isWalkingRight);

        // Debug (opcional):
      
    }



}