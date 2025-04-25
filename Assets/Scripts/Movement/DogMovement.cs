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

    [Header("Comportamento de Wander")]
    [Tooltip("Tempo que o cão espera em um nó antes de ir para o próximo")]
    [SerializeField] private float waitAtNodeDuration = 1.5f;

    [Header("Comportamento de Follow")]
    [Tooltip("Distância que o cão tenta manter do jogador ao seguir")]
    [SerializeField] private float followSafeDistance = 3.0f;
    [Tooltip("Quão perto o cão precisa estar para parar de seguir ativamente")]
    [SerializeField] private float followStopThreshold = 0.5f; // Margem perto da safeDistance

    [Header("Comportamento de Flee")]
    [Tooltip("Distância que o cão tenta alcançar ao fugir")]
    [SerializeField] private float fleeDistance = 10.0f;

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
    private Coroutine waitCoroutine = null; // Referência para a coroutine de espera
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
                StartWaitTimer(waitAtNodeDuration); // Inicia a espera
                break;

            case DogState.FollowingPlayer:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; } // Segurança
                // A lógica de definir o destino está no Update
                break;

            case DogState.Fleeing:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; } // Segurança
                StartFleeing();
                break;
        }
    }

    private void UpdateCurrentStateLogic()
    {
        // Checagem prioritária: se não pode se mover autonomamente, força o Idle
        if (!canAutoMove && currentState != DogState.Idle)
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
                // A Coroutine está cuidando da espera, não faz nada aqui
                break;

            case DogState.FollowingPlayer:
                UpdateFollowPlayerPosition(); // Atualiza o destino continuamente
                break;

            case DogState.Fleeing:
                // Verifica se chegou ao ponto de fuga inicial ou se precisa recalcular
                if (HasAgentArrived())
                {
                   // Debug.Log("Chegou ao ponto de fuga."); // Para Debug
                    // Decide o que fazer após fugir (voltar a perambular, ficar idle etc.)
                    ChangeState(DogState.MovingToDestination);
                }
                // Opcional: Se o jogador se aproximar muito durante a fuga, recalcular destino
                // else if (IsPlayerTooCloseWhileFleeing()) { StartFleeing(); }
                break;
        }
    }

    private void ExitCurrentStateLogic()
    {
        // Limpeza ao sair de um estado
        switch (currentState)
        {
            case DogState.WaitingAtDestination:
                StopWaitTimer(); // Garante que a coroutine de espera pare
                break;
                // Adicione limpeza para outros estados se necessário
        }
    }

    // --- Lógica Específica dos Comportamentos ---

    private void StartWaitTimer(float duration)
    {
        StopWaitTimer(); // Garante que não haja coroutines duplicadas
        waitCoroutine = StartCoroutine(WaitCoroutine(duration));
    }

    private void StopWaitTimer()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }

    private IEnumerator WaitCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        waitCoroutine = null; // Limpa a referência

        // Só transiciona se ainda estiver no estado de espera e puder se mover
        if (currentState == DogState.WaitingAtDestination && canAutoMove)
        {
           // Debug.Log("Tempo de espera finalizado, movendo para próximo nó."); // Para Debug
            ChangeState(DogState.MovingToDestination); // Volta a mover
        }
        // Se canAutoMove foi desativado durante a espera, o Update tratará a mudança para Idle
    }

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
        // Parar a coroutine de espera se estiver ativa
        StopWaitTimer();
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
    }

    // --- Animação (Placeholder) ---
    private void UpdateAnimations()
    {
        if (animator == null || agent == null) return;

        float speed = agent.velocity.magnitude;
        // bool isMoving = speed > 0.1f;

        // Aqui você colocaria a lógica para tocar as animações corretas
        // baseadas no 'currentState' e na 'speed'.
        // Exemplo muito básico:
        // animator.SetBool("IsWalking", isMoving);
        // animator.SetFloat("Speed", speed);

        // Você também precisaria de lógica para as animações Idle_Right/Left
        // baseadas na última direção de movimento ou orientação desejada.
    }

    // --- Funções Antigas (Removidas ou Refatoradas) ---
    // Arrival() -> Substituída por HasAgentArrived() e lógica de estado
    // MoveToDestination() -> Refatorada para SetAgentDestination() e lógica de estado
    // MoveTowardsPlayer() -> Lógica movida para UpdateFollowPlayerPosition()
    // RandomizeMovement() -> Lógica movida para estados Wander
    // FollowNode() -> Lógica movida para estado MovingToDestination
    // WaitInPlace() -> Lógica movida para estado WaitingAtDestination e Coroutine
    // FollowPlayer() -> Substituída por RequestFollowPlayer() e estado FollowingPlayer
    // StopFollowPlayer() -> Transição de estado controlará isso
    // ToggleMovement() -> Substituída por SetAutonomousMovement()
    // StopMovement() -> Refatorada para StopAgentMovement()
    // FollowPlayerAtSafeDistance() -> Lógica movida para UpdateFollowPlayerPosition()
    // FleeFromPlayer() -> Lógica movida para StartFleeing()
}