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
    [SerializeField] private PlayerController playerMovement;
    [SerializeField] private GameObject indicator;

    [Header("If object opens a minigame")]
    [SerializeField] private bool hasMinigame = false;
    [SerializeField] private GameObject minigame;
    [Header("Minigames List")]
    [SerializeField] private MiniGames miniGameType;
    enum MiniGames
    {
        MangoCatch,
        QuickTimeEvent,
        FillTheBowl,
    }

    #region Objective properties
    [Header("If object requires an item")] // not implemented yet!
    [SerializeField] private bool needsItem = false;
    [SerializeField] private GameObject itemR; // placeholderline!

    [Header("If object gives an item to the player")] // not implemented yet!
    [SerializeField] private bool givesItem = false;
    [SerializeField] private GameObject itemG; // placeholderline!

    [Header("If Object applies effect")] // not implemented yet!
    [SerializeField] private bool hasEffect = false;
    [SerializeField] private GameObject effect; // placeholderline!
    [SerializeField] private LayerMask clicklableLayers;
    #endregion

    #region  Minigame Variables
    [Header("Mango Catch")]
    [SerializeField] private int mangoGoal;
    [SerializeField] private float mangoFallSpeed;
    [SerializeField] private float coolDownBetweenMangos;

    [Header("Quick Time Event")]
    [SerializeField] private int QTEGoal;
    [SerializeField] private float QTEMoveSpeed;
    [SerializeField] private float QTESafeZoneSizePercentage; 
    #endregion

    private bool enable = false, interactable = true;
    private float cooldownTimer;
    public bool isComplete = false;
    private InGameProgress inGameProgress;
    private void Awake()
    {
        inGameProgress = FindObjectOfType<InGameProgress>();
        playerMovement = FindObjectOfType<PlayerController>();
        cooldownTimer = cooldown;
    }
    private void LateUpdate()
    {
        if(interactable && Input.GetMouseButtonDown(0)) SelectObjective();
        if(!interactable)
        {
            if(cooldown > 0f)
            {
                cooldownTimer -= Time.deltaTime;
                if(cooldownTimer <= 0f)
                {
                    interactable = true;
                    cooldownTimer = cooldown;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(enable && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entrou na area de um Objetivo!");
            if(hasMinigame) Invoke("StartMinigame", detectionDelay);
            if(hasEffect) Invoke("ApplyEffect", detectionDelay);
            if(givesItem) Invoke("GiveItem", detectionDelay);
        }
    }
    private void SelectObjective()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            enable = true;
            Debug.Log("Indo fazer esta tarefa!");
        }
        else if(enable)
        {
            enable = false;
            Debug.Log("Cancelou a tarefa!");
        }
    }
    public void CompleteTask()
    {
        playerMovement.ToggleMovement(true);
        interactable = false;
        isComplete = true;
        indicator.SetActive(false);
        TaskManager.Instance.objectives.Remove(this);
        inGameProgress.AddScore(scoreValue);
        TaskManager.Instance.UpdateList();
        Debug.Log("Tarefa completa!");
    }

    public void CloseTask()
    {
        playerMovement.ToggleMovement(true);
        Debug.Log("Tarefa fechada!");
    }
    private void StartMinigame()
    {
        if(enable)
        {
            playerMovement.ToggleMovement(false);
            enable = false;
            switch (miniGameType)
            {
                case MiniGames.MangoCatch:
                    minigame.GetComponent<MangoCatch>().SetMiniGameRules(mangoGoal, mangoFallSpeed, coolDownBetweenMangos);
                    minigame.GetComponent<MangoCatch>().objectivePlayerCheck = this;
                    minigame.GetComponent<MangoCatch>().StartMiniGame();
                    break;
                case MiniGames.QuickTimeEvent:
                    minigame.GetComponent<QuickTimeEvent>().SetMiniGameRules(QTEGoal, QTEMoveSpeed, QTESafeZoneSizePercentage);
                    minigame.GetComponent<QuickTimeEvent>().objectivePlayerCheck = this;
                    minigame.GetComponent<QuickTimeEvent>().StartMiniGame();
                    break;
                case MiniGames.FillTheBowl:

                    break;
            }
            Debug.Log("Iniciar Minigame!");
        }
    }
    private void ApplyEffect()
    {
        if(enable)
        {   
            enable = false;
            Debug.Log("Jogador recebeu um efeito magico unico!");
        }
    }
    private void GiveItem()
    {
        if(enable)
        {
            enable = false;
            Debug.Log("Jogador recebeu um presente misterioso!");
        }
    }
}
