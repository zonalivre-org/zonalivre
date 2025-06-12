using Unity.VisualScripting;
using UnityEngine;

public class ObjectiveInteract : MonoBehaviour
{
    [Header("Interacton Values")]
    [SerializeField] private float detectionDelay = 0.5f;
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private float cooldown = 0f;
    public bool enable = false, interactable = true;

    [Header("Elements")]
    public string objectiveDescription;
    public float averageTimeToComplete;
    private PlayerController playerMovement;
    [SerializeField] private GameObject indicator;
    [HideInInspector] public TaskItem taskItem;
    public Sprite taskIcon;
    [SerializeField] private Sprite objectiveCompleteSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask clicklableLayers;

    [Header("Minigames List")]
    [SerializeField] private MiniGames miniGameType;
    public enum MiniGames { MangoCatch, QuickTimeEvent, CleanMinigame, WhackAMole, PlantTheCitronela, DogClean }

    #region Objective Properties
    [Header("If object opens a minigame")]
    [SerializeField] private GameObject minigame;

    [Header("If object requires an item")]
    [SerializeField] private int idCheck;

    [Header("If object gives an item to the player")]
    [SerializeField] private int idGive;

    [Header("If object activates another object")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private bool deactivateItself = false;

    [Header("Mango Catch")]
    [SerializeField] private int mangoGoal;
    [SerializeField] private float mangoFallSpeed;
    [SerializeField] private float coolDownBetweenMangos;

    [Header("Quick Time Event")]
    [SerializeField] private int QTEGoal;
    [SerializeField] private float QTEMoveSpeed;
    [SerializeField] private float QTESafeZoneSizePercentage;

    [Header("Clean Minigame")]
    [Range(0, 1)][SerializeField] private float cleanSpeed;
    [SerializeField] private int trashAmount = 5;

    [Header("Whack A Mole")]
    [Range(1, 5)] [SerializeField] private int scoreToWin = 3;
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("Plant The Citronela")]
    [Range(0, 5)] [SerializeField] private float growthSpeed = 3f;

    [Header("Dog Clean")]
    [Range(1, 7)][SerializeField] private int maxDirtCount = 4; // Maximum number of dirt spots
    [Range(0f, 1f)][SerializeField] private float dogCleanSpeed = 0.5f;

    #endregion

    private float cooldownTimer;
    [HideInInspector] public bool isComplete = false;
    private PlayerInventory playerInventory;
    [SerializeField] private ObjectConnectionVisualizer objectConnectionVisualizer;

    private void Awake() { InitializeComponents(); }
    private void LateUpdate() { HandleInteraction(); }
    private void OnTriggerStay(Collider other) { if (enable && other.CompareTag("Player")) Invoke("StartMinigame", detectionDelay); }

    private void InitializeComponents()
    {
        playerMovement = FindObjectOfType<PlayerController>();
        playerInventory = FindObjectOfType<PlayerInventory>();

        cooldownTimer = cooldown;
    }
    void OnEnable()
    {
        MiniGameBase.OnMiniGameEnd += TurnOffEnable;
    }

    void OnDisable()
    {
        MiniGameBase.OnMiniGameEnd -= TurnOffEnable;
    }

    private void HandleInteraction()
    {
        if (interactable && Input.GetMouseButtonDown(0)) SelectObjective();
        if (!interactable) ManageCooldown();
    }

    private void ManageCooldown()
    {
        if (cooldown > 0f) cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f) { interactable = true; cooldownTimer = cooldown; }
    }

    private void SelectObjective()
    {
        RaycastHit hit;
        // Lança um raio a partir da posição do mouse nas camadas clicklableLayers.
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            // *** CORREÇÃO: Verificar se o objeto clicado É este GameObject ***
            if (hit.collider.gameObject == this.gameObject)
            {
                // Se clicou NESTE objetivo e ele está interagível (não completo e não em cooldown), HABILITA o trigger check.
                // A flag 'interactable' já garante que não está completo ou em cooldown.
                if (interactable)
                {
                    enable = true; // Habilita a flag para que OnTriggerStay possa iniciar o minigame
                    // Debug.Log($"Objetivo {gameObject.name} selecionado por clique. Trigger habilitado!"); // Para Debug
                    // TODO: Opcional: Mostrar feedback visual de seleção
                }
                 // else { Debug.Log($"Objetivo {gameObject.name} clicado, mas não interagível."); } // Feedback se clicou mas não interagiu
            }
            else if (enable)
            {
                // Se clicou em outro objeto nas camadas clicklableLayers ENQUANTO este objetivo estava 'enable', CANCELA.
                // Isso impede que um clique em outro objetivo dispare o minigame deste.
                enable = false; // Desabilita a flag 'enable'
                // Debug.Log($"Seleção de {gameObject.name} cancelada (clicou em outro lugar)."); // Para Debug
                 CancelInvoke(nameof(StartMinigame)); // Cancela qualquer agendamento pendente
                // A lógica de cancelamento de minigame em execução deve ser chamada externamente
                // pelo sistema que gerencia minigames (ex: um GameManager).
            }
        }
        // Se clicou FORA das clicklableLayers, o Raycast falha e 'enable' permanece como estava.
    }

    public void CloseTask()
    {
        playerMovement.ToggleMovement(true);
    }

    public void CompleteTask()
    {
        if (spriteRenderer && objectiveCompleteSprite) spriteRenderer.sprite = objectiveCompleteSprite;

        playerMovement.ToggleMovement(true);
        taskItem.MarkAsComplete();
        interactable = false;
        isComplete = true;
        indicator.SetActive(false);

        playerInventory.RemoveItem();
        GameManager.Instance.AddScore(scoreValue);

        if (objectToActivate)
        {
            objectToActivate.SetActive(true);
            objectToActivate.GetComponent<ObjectiveInteract>().taskItem.gameObject.SetActive(true);
        }
        if (deactivateItself) gameObject.SetActive(false);
    }

    private void StartMinigame()
    {
        if (GameManager.Instance.isMinigameActive) return;

        if (!enable || isComplete) return;

        playerMovement.ToggleMovement(false);
        enable = false;

        switch (miniGameType)
        {
            case MiniGames.MangoCatch: StartMangoCatch(); break;
            case MiniGames.QuickTimeEvent: StartQuickTimeEvent(); break;
            case MiniGames.CleanMinigame: StartCleanMinigame(); break;
            case MiniGames.WhackAMole: StartWhackAMoleMinigame(); break;
            case MiniGames.PlantTheCitronela: StartPlantTheCitronelaMinigame(); break;
            case MiniGames.DogClean: StartDogCleanMinigame(); break;
        }

    }

    private void StartMangoCatch()
    {
        minigame.GetComponent<MangoCatchMinigame>().SetMiniGameRules(mangoGoal, mangoFallSpeed, coolDownBetweenMangos);
        minigame.GetComponent<MangoCatchMinigame>().objectivePlayerCheck = this;
        minigame.GetComponent<MangoCatchMinigame>().StartMiniGame();
        GameManager.Instance.isMinigameActive = true;
    }
    private void StartQuickTimeEvent()
    {
        if (!CheckIfCanStartMinigame("Tela")) return;

        minigame.GetComponent<QuickTimeEventMinigame>().SetMiniGameRules(QTEGoal, QTEMoveSpeed, QTESafeZoneSizePercentage);
        minigame.GetComponent<QuickTimeEventMinigame>().objectivePlayerCheck = this;
        minigame.GetComponent<QuickTimeEventMinigame>().StartMiniGame();
        GameManager.Instance.isMinigameActive = true;
    }

    private void StartCleanMinigame()
    {
        minigame.GetComponent<CleanMinigame>().SetMiniGameRules(cleanSpeed, trashAmount);
        minigame.GetComponent<CleanMinigame>().objectiveInteract = this;
        minigame.GetComponent<CleanMinigame>().StartMiniGame();
        GameManager.Instance.isMinigameActive = true;
    }

    private void StartWhackAMoleMinigame()
    {
        minigame.GetComponent<WhackAMole>().SetMinigameRules(scoreToWin, spawnInterval);
        minigame.GetComponent<WhackAMole>().objectiveInteract = this;
        minigame.GetComponent<WhackAMole>().StartMiniGame();
        GameManager.Instance.isMinigameActive = true;
    }

    private void StartPlantTheCitronelaMinigame()
    {
        minigame.GetComponent<PlantTheCitronela>().SetMinigameRules(growthSpeed);
        minigame.GetComponent<PlantTheCitronela>().objectiveInteract = this;
        minigame.GetComponent<PlantTheCitronela>().StartMiniGame();
        GameManager.Instance.isMinigameActive = true;
    }
    
    private void StartDogCleanMinigame()
    {
        //if (!CheckIfCanStartMinigame("Sabao")) return;

        minigame.GetComponent<DogCleanMiniGame>().SetMiniGameRules(maxDirtCount, dogCleanSpeed);
        minigame.GetComponent<DogCleanMiniGame>().objectiveInteract = this;
        minigame.GetComponent<DogCleanMiniGame>().StartMiniGame();
        GameManager.Instance.isMinigameActive = true;
    }
    
    private bool CheckIfCanStartMinigame(string itemId = null)
    {

        if (playerInventory.GetItem() && playerInventory.GetItem().id == itemId)
        {
            playerInventory.RemoveItem();
            return true;
        }
        else
        {
            playerMovement.ToggleMovement(true);
            // Checa se tem o script ObjectConnecionVisualizer
            if (objectConnectionVisualizer)
            {
                objectConnectionVisualizer.ShowConnector();
                Debug.Log("Você não tem o item necessário para iniciar o minigame.");
            }

            // playerMovement.ToggleMovement(true);
            return false;
        }
    }

    private void TurnOffEnable()
    {
        enable = false;
    }
}
