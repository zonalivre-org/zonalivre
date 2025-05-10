using Unity.VisualScripting;
using UnityEngine;

public class ObjectiveInteract : MonoBehaviour
{
    [Header("Interacton Values")]
    [SerializeField] private float detectionDelay = 0.5f;
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private float cooldown = 0f;

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

    [Header("If object opens a minigame")]
    [SerializeField] private bool hasMinigame = false;
    [SerializeField] private GameObject minigame;

    [Header("Minigames List")]
    [SerializeField] private MiniGames miniGameType;
    public enum MiniGames { MangoCatch, QuickTimeEvent, CleanMinigame }

    #region Objective Properties

    [Header("If object requires an item")]
    [SerializeField] private bool needsItem = false;
    [SerializeField] private int idCheck;

    [Header("If object gives an item to the player")]
    [SerializeField] private bool givesItem = false;
    [SerializeField] private int idGive;

    [Header("If object activates another object")]
    [SerializeField] private GameObject objectToActivate;

    [Header("Mango Catch")]
    [SerializeField] private int mangoGoal;
    [SerializeField] private float mangoFallSpeed;
    [SerializeField] private float coolDownBetweenMangos;

    [Header("Quick Time Event")]
    [SerializeField] private int QTEGoal;
    [SerializeField] private float QTEMoveSpeed;
    [SerializeField] private float QTESafeZoneSizePercentage;

    [Header("Clean Minigame")]
    [Range(0,1)] [SerializeField] private float cleanSpeed;
    [SerializeField] private int trashAmount = 5;

    #endregion

    private bool enable = false, interactable = true;
    private float cooldownTimer;
    [HideInInspector] public bool isComplete = false;
    private InGameProgress inGameProgress;
    private PlayerInventory playerInventory;

    private void Awake() { InitializeComponents(); }
    private void LateUpdate() { HandleInteraction(); }
    private void OnTriggerStay(Collider other) { if(enable && other.CompareTag("Player")) Invoke("StartMinigame", detectionDelay); }

    private void InitializeComponents() {
        inGameProgress = FindObjectOfType<InGameProgress>();
        playerMovement = FindObjectOfType<PlayerController>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        cooldownTimer = cooldown;
    }

    private void HandleInteraction() {
        if (interactable && Input.GetMouseButtonDown(0)) SelectObjective();
        if (!interactable) ManageCooldown();
    }

    private void ManageCooldown() {
        if (cooldown > 0f) cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f) { interactable = true; cooldownTimer = cooldown; }
    }

    private void SelectObjective() {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, clicklableLayers)) { enable = true; } else if (enable) enable = false;
    }


    public void CloseTask(){
        playerMovement.ToggleMovement(true);
    }
    public void CompleteTask() {
        if (spriteRenderer && objectiveCompleteSprite) spriteRenderer.sprite = objectiveCompleteSprite;

        playerMovement.ToggleMovement(true);
        taskItem.MarkAsComplete();
        interactable = false;
        isComplete = true;
        indicator.SetActive(false);

        playerInventory.RemoveItem();
        inGameProgress.AddScore(scoreValue);

        if (objectToActivate) objectToActivate.SetActive(true);
    }

    private void StartMinigame() {
        if (!enable || isComplete) return;

        playerMovement.ToggleMovement(false);
        enable = false;

        switch (miniGameType) {
            case MiniGames.MangoCatch: StartMangoCatch(); break;
            case MiniGames.QuickTimeEvent: StartQuickTimeEvent(); break;
            case MiniGames.CleanMinigame: StartCleanMinigame(); break;
        }
    }

    private void StartMangoCatch() { 
         minigame.GetComponent<MangoCatchMinigame>().SetMiniGameRules(mangoGoal, mangoFallSpeed, coolDownBetweenMangos);
                    minigame.GetComponent<MangoCatchMinigame>().objectivePlayerCheck = this;
                    minigame.GetComponent<MangoCatchMinigame>().StartMiniGame();
    }
    private void StartQuickTimeEvent() {
        if (!CheckIfCanStartMinigame("Tela")) return;

                    minigame.GetComponent<QuickTimeEventMinigame>().SetMiniGameRules(QTEGoal, QTEMoveSpeed, QTESafeZoneSizePercentage);
                    minigame.GetComponent<QuickTimeEventMinigame>().objectivePlayerCheck = this;
                    minigame.GetComponent<QuickTimeEventMinigame>().StartMiniGame();
                   
    }

    private void StartCleanMinigame() {
minigame.GetComponent<CleanMinigame>().SetMiniGameRules(cleanSpeed, trashAmount);
                    minigame.GetComponent<CleanMinigame>().objectiveInteract = this;
                    minigame.GetComponent<CleanMinigame>().StartMiniGame();
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
            Debug.Log("Você não tem o item necessário para iniciar o minigame.");
            return false;
        }
        
    }

}
