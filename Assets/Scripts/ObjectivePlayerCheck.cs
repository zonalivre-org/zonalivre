using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectivePlayerCheck : MonoBehaviour
{
    [HideInInspector] public bool enable = false;
    [SerializeField] private float detectionDelay = 0.5f;
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private PlayerController playerReference;

    [Header("If object opens a minigame")]
    [SerializeField] private bool hasMinigame = false;
    [SerializeField] private GameObject minigame;

    [Header("If object requires an item")] // not implemented yet!
    [SerializeField] private bool needsItem = false;
    [SerializeField] private GameObject itemR; // placeholderline!

    [Header("If object gives an item to the player")] // not implemented yet!
    [SerializeField] private bool givesItem = false;
    [SerializeField] private GameObject itemG; // placeholderline!

    [Header("If Object applies effect")] // not implemented yet!
    [SerializeField] private bool hasEffect = false;
    [SerializeField] private GameObject effect; // placeholderline!
    [SerializeField] private GameObject inGameProgress;
    private bool isTaskDone = false;
    private bool enabletimer = false;
    private float timer;
    private InGameProgress notify;

    [Header("MiniGame Constructor")]
    [SerializeField] private MiniGames miniGameType;
    enum MiniGames
    {
        MangoCatch,
        QuickTimeEvent,
        HoldTheButton,
    }

    [Header("Mango Catch")]
    [SerializeField] private int mangoGoal;
    [SerializeField] private float mangoFallSpeed;
    [SerializeField] private float coolDownBetweenMangos;

    [Header("Quick Time Event")]
    [SerializeField] private int QTEGoal;
    [SerializeField] private float QTEMoveSpeed;
    [SerializeField] private float QTESafeZoneSizePercentage; 

    private void Awake()
    {
        ToggleState(false);
        notify = inGameProgress.GetComponent<InGameProgress>();
    }
    private void LateUpdate()
    {
        if (enable && enabletimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                if (!needsItem) // verificar se o jogador tem ou não o item se essa boolean for verdadeira caso contrario o lugar não pode ser interagido
                {
                    if (hasMinigame) ToggleUI(true);
                    if (givesItem) { } // entregar o item para o jogador ser essa boolean for verdadeira
                    if (hasEffect) { } // aplicar o efeito do objetivo no jogador se essa boolean for verdadeira
                }
                ToggleState(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTaskDone && enable && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entrou na area!");
            enabletimer = true;
            Debug.Log("Contador ativado! = " + enabletimer);
        }
    }
    // private void OnTriggerExit(Collider other)
    // {
    //     if (enable && other.gameObject.CompareTag("Player"))
    //     {
    //         enabletimer = false;
    //         ToggleState(false);
    //         Debug.Log("Saiu da area! = " + enable);
    //     }
    // }
    public void ToggleUI(bool _enable)
    {
        if (!isTaskDone)
        {
            playerReference.canMove = false;

            switch (miniGameType)
            {
                case MiniGames.MangoCatch:
                    minigame.GetComponent<MangoCatch>().SetMiniGameRules(mangoGoal, mangoFallSpeed, coolDownBetweenMangos);
                    minigame.GetComponent<MangoCatch>().objectivePlayerCheck = this;
                    minigame.GetComponent<MangoCatch>().ResetMiniGame();
                    break;
                case MiniGames.QuickTimeEvent:
                    minigame.GetComponent<QuickTimeEvent>().SetMiniGameRules(QTEGoal, QTEMoveSpeed, QTESafeZoneSizePercentage);
                    minigame.GetComponent<QuickTimeEvent>().StartQTEGame();
                    break;
            }
        }
    }
    public void ToggleState(bool _enable)
    {
        if (!isTaskDone)
        {
            timer = detectionDelay;
            enable = _enable;
        }
    }
    public void CompleteTask()
    {
        isTaskDone = true;
        Debug.Log("Tarefa completa = " + isTaskDone);
        notify.AddScore(scoreValue);
        playerReference.canMove = true;
    }
}
