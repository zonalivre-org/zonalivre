using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic; // Adicionado para List
using Random = UnityEngine.Random; // Adicionado para Random, caso seja usado implicitamente

// Este script representa um ponto de interação no mundo que pode iniciar uma tarefa/minigame.
// A interação é iniciada por uma combinação de clique do jogador (via SelectObjective)
// que habilita um trigger de área (OnTriggerStay).
// Pode requerer/dar um item (usando IDs int antigos) e ativar outro objeto na conclusão.
[RequireComponent(typeof(Collider))] // Garante que há um Collider para o trigger
public class ObjectiveInteract : MonoBehaviour
{
    // --- Configurações de Interação e Progresso ---
    [Header("Configurações da Interação")]
    [Tooltip("Score dado ao completar a tarefa.")]
    [SerializeField] private int scoreValue = 1;
    [Tooltip("Tempo de cooldown (em segundos) após a interação.")]
    [SerializeField] private float cooldown = 0f;
    [Tooltip("Camadas que o player clica para habilitar a interação com este objetivo.")]
    [SerializeField] private LayerMask clicklableLayers; // Usado em SelectObjective

    [Header("Minigame Associado")]
    [Tooltip("Indica se este objetivo abre um minigame (Flag antiga, dependente da lógica existente).")]
    [SerializeField] private bool hasMinigame = false; // Flag antiga
    [Tooltip("O GameObject do minigame a ser ativado/usado.")]
    [SerializeField] private GameObject minigame; // Referência direta ao GameObject do minigame

    [Tooltip("O tipo de minigame para configurar as regras específicas.")]
    [SerializeField] private MiniGames miniGameType; // Usado no switch em StartMinigame
    public enum MiniGames // Enum para tipos de minigame (usado no switch)
    {
        MangoCatch,
        QuickTimeEvent,
        CleanMinigame, // Parece que este é o FillTheBowl refatorado?
    }

    // --- Requisitos e Efeitos (Lógica Antiga Baseada em IDs Int) ---
    #region Lógica Antiga de Itens e Efeitos

    [Header("Requisitos/Efeitos (Lógica Antiga c/ IDs)")]
    [Tooltip("Indica se o objetivo requer um item (Flag antiga).")]
    [SerializeField] private bool needsItem = false; // Flag antiga
    [Tooltip("ID int do item necessário para CheckIfCanStartMinigame (Lógica antiga).")]
    [SerializeField] private int idCheck; // ID int necessário (usado pela lógica antiga)

    [Tooltip("Indica se o objetivo dá um item (Flag antiga, não totalmente implementada na conclusão).")]
    [SerializeField] private bool givesItem = false; // Flag antiga
    [Tooltip("ID int do item a ser dado (Lógica antiga, não totalmente usada).")]
    [SerializeField] private int idGive; // ID int a ser dado (não usado na CompleteTask fornecida)

    [Tooltip("Outro GameObject a ser ativado na conclusão.")]
    [SerializeField] private GameObject objectToActivate;

    #endregion

    // --- Configurações Específicas dos Minigames (Lógica Antiga) ---
    #region Configurações Antigas de Minigames

    [Header("Configurações Específicas dos Minigames")] // Usadas no switch em StartMinigame

    [Header("Mango Catch Config")]
    [SerializeField] private int mangoGoal;
    [SerializeField] private float mangoFallSpeed;
    [SerializeField] private float coolDownBetweenMangos;

    [Header("Quick Time Event Config")]
    [SerializeField] private int QTEGoal;
    [SerializeField] private float QTEMoveSpeed;
    [SerializeField] private float QTESafeZoneSizePercentage;

    [Header("Clean Minigame Config")] // Assumindo que este é o FillTheBowl
    [Range(0, 1)][SerializeField] private float cleanSpeed;
    [SerializeField] private int trashAmount = 5;

    #endregion

    // --- Referências Essenciais (Encontradas em Awake) ---
    private PlayerController playerMovement; // Referência ao script de movimento do player
    private InGameProgress inGameProgress; // Referência ao script de progresso do jogo (para score)
    private PlayerInventory playerInventory; // Referência ao inventário do player

    // --- Referências de UI/Visual (Associadas a este Objetivo) ---
    [Header("Feedback Visual e UI de Tarefa")]
    [Tooltip("O GameObject indicador que mostra que o objetivo está ativo/interagível.")]
    [SerializeField] private GameObject indicator; // Indicador visual no mundo
    [HideInInspector] public TaskItem taskItem; // Referência à UI de tarefa associada (configurada externamente)
    public Sprite taskIcon; // Ícone para a UI de tarefa (usado externamente por quem configura taskItem)
    [Tooltip("SpriteRenderer para mudar a aparência visual deste objetivo ao ser completado.")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Sprite para mostrar no spriteRenderer quando o objetivo for completado.")]
    [SerializeField] private Sprite objectiveCompleteSprite;

    [Header("Propriedades da Tarefa (para UI de Tarefas)")]
    // Estas propriedades são para serem lidas pela UI de tarefas (TaskItem)
    public string objectiveDescription; // Descrição para a UI de tarefa
    public float averageTimeToComplete; // Tempo médio para a UI de tarefa


    // --- Estado Interno ---
    [HideInInspector] public bool isComplete = false; // Estado de conclusão da tarefa (configurado/lido externamente)
    private bool enable = false; // Flag antiga: Habilita a checagem de trigger em OnTriggerStay após clique.
    private bool interactable = true; // Flag antiga: Controla se a interação é permitida (afetada por cooldown e isComplete).
    private float cooldownTimer; // Timer para o cooldown.
    [Tooltip("Delay (em segundos) após entrar na área de trigger para iniciar a interação (Lógica antiga).")]
    [SerializeField] private float detectionDelay = 0.5f; // Usado com Invoke na lógica antiga

    // --- Inicialização ---
    private void Awake()
    {
        // Encontra as referências essenciais na cena
        inGameProgress = FindObjectOfType<InGameProgress>();
        playerMovement = FindObjectOfType<PlayerController>();
        playerInventory = FindObjectOfType<PlayerInventory>(); // Assumindo que PlayerInventory está na cena

        // Validações essenciais
        if (playerMovement == null) Debug.LogError("ObjectiveInteract: PlayerController não encontrado na cena!", this);
        if (playerInventory == null) Debug.LogError("ObjectiveInteract: PlayerInventory não encontrado na cena!", this);

        // Avisos para dependências menos essenciais ou opcionais
        if (inGameProgress == null) Debug.LogWarning("ObjectiveInteract: InGameProgress não encontrado na cena! Score não será adicionado.", this);
        if (spriteRenderer == null) Debug.LogWarning("ObjectiveInteract: SpriteRenderer não atribuído. O visual não mudará na conclusão.", this);
        if (indicator == null) Debug.LogWarning("ObjectiveInteract: Indicator GameObject não atribuído. Ele não será desativado na conclusão.", this);
        if (taskItem == null) Debug.LogWarning($"ObjectiveInteract: TaskItem não atribuído para '{gameObject.name}'. A UI de tarefa não será atualizada na conclusão.", this);


        cooldownTimer = cooldown; // Inicializa o timer do cooldown
        // isComplete deve ser configurado externamente (por save/load, por exemplo)
        UpdateVisualCompletionState(); // Atualiza a aparência com base no estado inicial
    }

    // --- Loops de Update (Gerenciamento de Cooldown e Input) ---
    private void LateUpdate()
    {
        // Gerencia o cooldown se a tarefa não estiver completa e o cooldown for maior que zero
        if (!isComplete && cooldown > 0f)
        {
             // Decrementa o timer apenas se a interação não for permitida atualmente (devido a cooldown)
            if (!interactable)
            {
                 cooldownTimer -= Time.deltaTime;
                 if (cooldownTimer <= 0f)
                 {
                     interactable = true; // Permite interação novamente
                     cooldownTimer = cooldown; // Reinicia o timer para o próximo uso/cooldown
                 }
            }
        }

        // Lógica de clique para *tentar* selecionar o objetivo (Lógica antiga)
        if (interactable && Input.GetMouseButtonDown(0))
        {
            SelectObjective();
        }
    }

    // --- Lógica de Trigger (Lógica Antiga) ---
    // Este método é chamado continuamente enquanto outro Collider está dentro deste trigger.
    private void OnTriggerStay(Collider other)
    {
        // A lógica de trigger só funciona se 'enable' for true (definido por SelectObjective)
        // e se o objeto que entrou no trigger for o Player.
        if (enable && other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Player na área de um Objetivo habilitado!"); // Para Debug

            // A lógica antiga usa Invoke após um detectionDelay
            // Para não invocar múltiplos StartMinigame se o player ficar na área,
            // geralmente se usa uma flag para garantir que a invocação ocorra apenas uma vez.
            // Como as restrições não permitem mudar muito, assumimos que o Invoke é limpo
            // por StartMinigame desabilitando 'enable'.
            // Invoke("StartMinigame", detectionDelay); // Cuidado: Pode disparar múltiplos Invokes sem controle adicional!

            // *** Para evitar múltiplos invokes sem quebrar o fluxo ***
            // Uma forma segura dentro das restrições seria:
             if (enable && !IsInvoking(nameof(StartMinigame))) // Checa se já não está agendado
             {
                 // Debug.Log($"Agendando StartMinigame em {detectionDelay}s."); // Para Debug
                 Invoke(nameof(StartMinigame), detectionDelay);
             }
        }
    }


    // --- Mecanismo de Seleção por Clique (Lógica Antiga) ---
    // Chamado por LateUpdate para verificar se o player clicou neste objetivo.
    private void SelectObjective()
    {
        RaycastHit hit;
        // Lança um raio a partir da posição do mouse nas camadas clicklableLayers.
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
  
                // Se clicou NESTE objetivo e ele está interagível, HABILITA o trigger check.
                if (interactable)
                {
                    enable = true;
                    // Debug.Log($"Objetivo {gameObject.name} selecionado por clique. Trigger habilitado!"); // Para Debug
                    // TODO: Opcional: Mostrar feedback visual de seleção
                }
                // else { Debug.Log($"Objetivo {gameObject.name} não interagível ou em cooldown."); } // Feedback
            
            else if (enable)
            {
                // Se clicou em outra coisa nas camadas clicklableLayers ENQUANTO este objetivo estava 'enable', CANCELA.
                enable = false;
                // Debug.Log($"Seleção de {gameObject.name} cancelada (clicou em outro lugar). Trigger desabilitado."); // Para Debug
                // TODO: Opcional: Remover feedback visual de seleção
                CancelInvoke(nameof(StartMinigame)); // Cancela qualquer Invoke pendente
            }
        }
        // Se clicou FORA das clicklableLayers, a flag 'enable' permanece como estava.
    }


    // --- Início e Fim da Interação (Minigame ou Ação Direta) ---
    // Chamado via Invoke na lógica antiga após Player entrar na área de trigger habilitada.
    private void StartMinigame()
    {
        // Só inicia se ainda estiver habilitado pela lógica antiga
        if (enable)
        {
            // Debug.Log($"Iniciando minigame/interação para {gameObject.name}!"); // Para Debug

            // Desabilita a flag 'enable' para impedir múltiplos starts pelo trigger.
            // A flag 'interactable' é separada e controlada pelo cooldown/conclusão.
            enable = false; // Desabilita o trigger check IMEDIATAMENTE

            // Desabilita movimento do player ENQUANTO o minigame roda
            if (playerMovement != null) playerMovement.ToggleMovement(false);

            // --- Lógica de Item (Checagem e Consumo) ---
            // Esta lógica está aqui na StartMinigame na versão original.
            // É uma checagem de item *antes* de iniciar o minigame, e o item é removido *aqui*.
            // O nome do método CheckIfCanStartMinigame foi mantido.
            if (needsItem) // Se a flag antiga 'needsItem' está marcada
            {
                // A lógica antiga chama CheckIfCanStartMinigame com um ID string "Tela".
                // Isso sugere que GetItem() e o 'id' do ItemData retornam uma string comparável.
                // Mantemos a chamada exata por restrição.
                 if (!CheckIfCanStartMinigame("Tela")) // Chama a checagem que também remove o item e lida com player movement
                 {
                     // CheckIfCanStartMinigame já desabilitou player movement se falhou.
                     // E já deu um Debug.Log.
                     // A StartMinigame simplesmente para aqui.
                     return;
                 }
                 // Se CheckIfCanStartMinigame retornou true, o item foi removido e player movement desabilitado.
            }
            // else { Debug.Log("Nenhum item necessário pela lógica antiga."); } // Para Debug

            // --- Lógica de Ativação/Configuração do Minigame ---
            if (hasMinigame && minigame != null) // Se a flag antiga 'hasMinigame' está marcada E a referência existe
            {
                // Debug.Log("Ativando GameObject do Minigame!"); // Para Debug
                minigame.SetActive(true); // Ativa o GameObject do minigame

                // Configura regras específicas do minigame usando o switch (Lógica antiga)
                // Assumimos que os scripts dos minigames (MangoCatchMinigame, etc.) existem no GameObject 'minigame'
                // e têm métodos SetMiniGameRules e variáveis objectivePlayerCheck/objectiveInteract.
                switch (miniGameType)
                {
                    case MiniGames.MangoCatch:
                        // Assumimos que MangoCatchMinigame existe no 'minigame' GameObject
                        minigame.GetComponent<MangoCatchMinigame>().SetMiniGameRules(mangoGoal, mangoFallSpeed, coolDownBetweenMangos);
                        minigame.GetComponent<MangoCatchMinigame>().objectivePlayerCheck = this; // Passa referência para callback
                        minigame.GetComponent<MangoCatchMinigame>().StartMiniGame(); // Inicia o minigame
                        break;

                    case MiniGames.QuickTimeEvent:
                         // Assumimos que QuickTimeEventMinigame existe no 'minigame' GameObject
                        minigame.GetComponent<QuickTimeEventMinigame>().SetMiniGameRules(QTEGoal, QTEMoveSpeed, QTESafeZoneSizePercentage);
                        minigame.GetComponent<QuickTimeEventMinigame>().objectivePlayerCheck = this; // Passa referência para callback
                        minigame.GetComponent<QuickTimeEventMinigame>().StartMiniGame(); // Inicia o minigame
                        break;

                    case MiniGames.CleanMinigame: // Assumindo que é o FillTheBowl/Clean
                        // Assumimos que CleanMinigame existe no 'minigame' GameObject
                        minigame.GetComponent<CleanMinigame>().SetMiniGameRules(cleanSpeed, trashAmount);
                        minigame.GetComponent<CleanMinigame>().objectiveInteract = this; // Passa referência para callback
                        minigame.GetComponent<CleanMinigame>().StartMiniGame(); // Inicia o minigame
                        break;
                }
                // Debug.Log($"Minigame '{miniGameType}' configurado e iniciado."); // Para Debug
            }
            else // --- Lógica de Ação Direta (Sem Minigame) ---
            {
                // Se não há minigame associado pela flag antiga, a interação é instantânea.
                // Debug.Log("Nenhum minigame associado pela lógica antiga. Completando imediatamente."); // Para Debug
                CompleteTask(); // Completa a tarefa imediatamente se não há minigame
            }
        }
        // else { Debug.Log("StartMinigame chamado mas 'enable' é false."); } // Para Debug
    }


    /// <summary>
    /// Método chamado pelos scripts dos minigames (ou lógicas de ação direta) quando a tarefa é concluída com sucesso.
    /// DEVE ser chamado por eles para finalizar a interação.
    /// </summary>
    public void CompleteTask()
    {
        // Impede completar de novo se já estiver completo
        if (isComplete)
        {
            // Debug.Log($"CompleteTask chamado, mas '{gameObject.name}' já está completo."); // Para Debug
            return;
        }

        // Debug.Log($"Tarefa '{gameObject.name}' Concluída com sucesso!"); // Para Debug

        // --- Marcar Tarefa/Objetivo Como Completo ---
        isComplete = true; // Marca como completo

        // Atualiza aparência visual (se configurado)
        UpdateVisualCompletionState();

        // Notifica o sistema de UI de tarefas (se taskItem estiver atribuído)
        if (taskItem != null) taskItem.MarkAsComplete();

        // --- Lógica de Item (Consumo e Concessão) ---
        // Conforme a CompleteTask original: Remove o item no inventário, independentemente de qual seja!
        if (playerInventory != null)
        {
             // A lógica original remove o item SEM verificar qual é.
             // playerInventory.RemoveItem(); // Removido pois CheckIfCanStartMinigame já remove se needsItem e item correto.
                                            // Se a tarefa não precisava de item, então nenhum item é removido aqui.
                                            // Se a tarefa precisava de item e CheckIfCanStartMinigame removeu, não remove de novo.
                                            // Se a tarefa precisava de item mas o player estava com o item ERRADO, CheckIfCanStartMinigame retornou false e StartMinigame parou.
                                            // Portanto, a remoção do item está ligada a CheckIfCanStartMinigame na lógica original.
        }
        // A lógica original *não* dava item na CompleteTask (apesar da flag givesItem existir).
        // if (givesItem) { ... } // Lógica original não implementada aqui.

        // --- Lógica de Progresso ---
        if (inGameProgress != null)
        {
             inGameProgress.AddScore(scoreValue);
             // Debug.Log($"Score Adicionado: {scoreValue}. Total: {inGameProgress.GetCurrentScore()}"); // Exemplo debug com score atual
        }

        // --- Lógica de Finalização da Interação ---
        // Reativa movimento do player
        if (playerMovement != null) playerMovement.ToggleMovement(true);

        // Desativa o GameObject do minigame (se foi ativado por este ObjectiveInteract)
        if (minigame != null && minigame.activeSelf) // Verifica se está ativo antes de desativar
        {
            minigame.SetActive(false);
        }

        // Inicia o cooldown (aplica para impedir interações futuras, se cooldown > 0)
        // Mesmo que isComplete seja true, setting interactable = false aqui respeita o cooldown antes de ser resetado em LateUpdate
        if (cooldown > 0f) interactable = false;
         else interactable = true; // Se cooldown for 0, a interação é sempre permitida após conclusão (se não isComplete)

        // A lógica de ativação de outro objeto na conclusão foi mantida.
         ActivateObjectOnCompletion(); // Chama a lógica de ativação


         // Debug.Log($"Finalizado {gameObject.name}. isComplete: {isComplete}, interactable: {interactable}."); // Para Debug

    }

     /// <summary>
     /// Método chamado pelos scripts dos minigames se a tarefa for cancelada ou falhar.
     /// DEVE ser chamado por eles.
     /// </summary>
    public void CloseTask() // Nome original mantido
    {
         // Não faz nada se já estiver completo
         if (isComplete)
        {
            // Debug.Log($"CloseTask chamado, mas '{gameObject.name}' já está completo."); // Para Debug
            return;
        }

         // Debug.Log($"Tarefa '{gameObject.name}' Cancelada/Falhou!"); // Para Debug
         // Não consome nem dá item na falha (conforme CompleteTask original)
         // Não marca como isComplete

         // Reativa movimento do player
        if (playerMovement != null) playerMovement.ToggleMovement(true);

        // Desativa o GameObject do minigame (se foi ativado)
        if (minigame != null && minigame.activeSelf)
        {
            minigame.SetActive(false);
        }

        // Não inicia cooldown na falha pela lógica original.
        interactable = true; // Garante que pode interagir novamente imediatamente após falha/cancelamento (se não isComplete)

         // Debug.Log($"Cancelado {gameObject.name}. isComplete: {isComplete}, interactable: {interactable}."); // Para Debug

    }

    // --- Lógica de Checagem de Item Necessário (Lógica Antiga) ---
    // Chamado por StartMinigame se needsItem for true.
    // Verifica se o item no inventário tem o 'id' string especificado E remove o item se verdadeiro.
    // Também lida com player movement.
    private bool CheckIfCanStartMinigame(string itemId = null) // Nome original mantido
    {
        // Debug.Log($"Verificando item necessário: ID '{itemId}'"); // Para Debug

        // Verifica se o playerInventory existe e se ele tem um item
        if (playerInventory == null || !playerInventory.GetItem()) // Assumindo GetItem() retorna o ItemData ou null
        {
            // Não tem item nenhum
            // Debug.Log("Inventário vazio. Você não tem o item necessário."); // Para Debug
             if (playerMovement != null) playerMovement.ToggleMovement(true); // Reativa movimento se estava desabilitado (por StartMinigame)
            // TODO: Mostrar UI de "precisa de item" aqui ou em OnInteract
            return false;
        }

        // Verifica se o ID do item no inventário corresponde ao ID string necessário
        // Assumindo que ItemData.id pode ser comparado com um string.
        if (playerInventory.GetItem().id == itemId) // Assumindo ItemData tem um campo 'id' comparável a string
        {
            // Tem o item correto
            // Debug.Log($"Item correto ({playerInventory.GetItem().displayName}) encontrado. Removendo item."); // Para Debug
             playerInventory.RemoveItem(); // Remove o item do inventário (Lógica original)
             // Player movement já foi desabilitado por StartMinigame.
            return true;
        }
        else
        {
            // Tem um item, mas é o item errado.
            // Debug.Log($"Item incorreto no inventário ({playerInventory.GetItem().displayName}). Você não tem o item necessário."); // Para Debug
            if (playerMovement != null) playerMovement.ToggleMovement(true); // Reativa movimento se estava desabilitado (por StartMinigame)
            // TODO: Mostrar UI de "precisa de item" aqui ou em OnInteract
            return false;
        }
    }

    // --- Lógica de Ativação de Objeto na Conclusão ---
    // Chamado por CompleteTask para ativar outro objeto.
    private void ActivateObjectOnCompletion()
    {
         if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            // Tenta encontrar o ObjectiveInteract no objeto ativado para ativar a TaskItem dele
            ObjectiveInteract activatedObjective = objectToActivate.GetComponent<ObjectiveInteract>();
            if (activatedObjective != null && activatedObjective.taskItem != null)
            {
                activatedObjective.taskItem.gameObject.SetActive(true);
                 // TODO: Opcional: Chamar algum método no activatedObjective se necessário,
                 // como SetInitialState() se ele precisar ser configurado ao ser ativado.
            } else if (activatedObjective == null) {
                 Debug.LogWarning($"ObjectiveInteract: Objeto para ativar '{objectToActivate.name}' não tem um ObjectiveInteract component.", this);
            } else { // activatedObjective != null, mas taskItem == null
                 Debug.LogWarning($"ObjectiveInteract: Objeto para ativar '{objectToActivate.name}' tem ObjectiveInteract mas taskItem não atribuído.", this);
            }
            // Debug.Log("Ativou o objeto: " + objectToActivate.name); // Para Debug
        }
    }

    // --- Métodos Auxiliares de Feedback Visual/Estado ---
    // Chamado em Awake e CompleteTask para atualizar a aparência visual.
    private void UpdateVisualCompletionState()
    {
        if (isComplete)
        {
            // Atualiza sprite se aplicável
            if (spriteRenderer != null && objectiveCompleteSprite != null)
            {
                spriteRenderer.sprite = objectiveCompleteSprite;
            }
            // Desativa indicador se aplicável
            if (indicator != null)
            {
                indicator.SetActive(false);
            }
            // Ativa objeto associado (chamado aqui para ser consistente com o estado visual)
            ActivateObjectOnCompletion();
             // TODO: Opcional: Tocar som/efeito de conclusão
        }
        else
        {
            // Configuração visual para estado NÃO COMPLETO (garante estado inicial correto)
             // if (spriteRenderer != null && originalSprite != null) spriteRenderer.sprite = originalSprite; // Precisa guardar sprite original no Awake
             if (indicator != null) indicator.SetActive(true);
             // Garante que o objeto a ativar esteja desativado inicialmente (se for ativado por este objetivo)
             // E que sua task item também esteja desativada.
             if (objectToActivate != null)
             {
                 // Só desativa se ele já estiver ativo, para não interferir com objetos que começam desativados por outros motivos
                 if (objectToActivate.activeSelf) objectToActivate.SetActive(false);
                 ObjectiveInteract activatedObjective = objectToActivate.GetComponent<ObjectiveInteract>();
                 if (activatedObjective != null && activatedObjective.taskItem != null)
                 {
                    if(activatedObjective.taskItem.gameObject.activeSelf) activatedObjective.taskItem.gameObject.SetActive(false);
                 }
             }
        }
    }

     // Método público para configurar o estado de conclusão (útil para save/load)
    public void SetCompletionState(bool completed)
    {
        isComplete = completed;
        UpdateVisualCompletionState();
        // Se estiver completo, talvez notificar taskItem na carga também?
        if (isComplete && taskItem != null) taskItem.MarkAsComplete(); // Notei que taskItem.gameObject.SetActive(true) é chamado em CompleteTask, mas não aqui.
                                                                        // Se SetCompletionState é para carregar estado, ele deveria replicar os efeitos visuais/UI.
                                                                        // A lógica original ativa taskItem.gameObject em CompleteTask.
                                                                        // Se TaskItem já gerencia sua própria ativação/desativação baseada na flag de conclusão,
                                                                        // chamar MarkAsComplete aqui pode ser suficiente.
                                                                        // Assumindo que MarkAsComplete já lida com taskItem.gameObject.SetActive(true).
    }

    // --- Métodos Adicionais (Se GetItem() não estiver em PlayerInventory) ---
    /*
    // PLACEHOLDER: Se PlayerInventory.GetItem() não existe ou não retorna ItemData:
    // VOCÊ PRECISARÁ ADICIONAR UM MÉTODO Público COMO ESTE ao PlayerInventory.cs
    // Exemplo:
    // public ItemData GetItem() { return _heldItem; } // Onde _heldItem é a variável interna do PlayerInventory
    */
}