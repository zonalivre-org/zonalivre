using System;
using TMPro;
using UnityEngine;

public abstract class MiniGameBase : MonoBehaviour
{
    public static Action OnMiniGameStart;
    public static Action OnMiniGameEnd;
    public static Action OnMinigameInteract;
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private float tipDelay = 5f; // Time in seconds before showing the tip

    private float timeSinceLastClick;
    protected bool isMiniGameActive;
    protected bool firstActionTriggered = false;
    protected bool isMiniGameComplete = false;

    void Start() 
    {
        OnStart(); // You will need to call this OnStart method in every child class, because Unity is Unity :D 
    }

    protected void OnStart()
    {
        OnMiniGameStart += StartMiniGame;
        OnMiniGameEnd += EndMiniGame;
        OnMinigameInteract += RegisterPlayerClick;

        isMiniGameComplete = false;

        firstActionTriggered = false;

        isMiniGameActive = true;
    }
    void Update()
    {
        TipCheck(); // And this one too :/
    }

    protected void TipCheck()
    {
        if (isMiniGameActive)
        {
            timeSinceLastClick += Time.deltaTime;

            // Show the tipText if the delay is exceeded
            if (timeSinceLastClick >= tipDelay && tipText != null && !tipText.gameObject.activeSelf)
            {
                tipText.gameObject.SetActive(true);
            }
        }
    }

    public void RegisterPlayerClick()
    {
        if (firstActionTriggered == false) firstActionTriggered = true;

        // Reset the timer when the player clicks
        timeSinceLastClick = 0f;

        // Hide the tipText if it's active
        if (tipText != null)
            tipText.gameObject.SetActive(false);
    }

    public abstract void StartMiniGame();

    public abstract void EndMiniGame();

}