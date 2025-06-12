using System;
using TMPro;
using UnityEngine;

public abstract class MiniGameBase : MonoBehaviour
{
    public static Action OnMiniGameStart;
    public static Action OnMiniGameEnd;
    public static Action OnMiniGameComplete;

    public static Action OnMinigameInteract;
    [SerializeField] protected TMP_Text tipText;
    [SerializeField] private float tipDelay = 5f; // Time in seconds before showing the tip

    private float timeSinceLastClick;
    protected bool isMiniGameActive;
    protected bool firstActionTriggered = false;
    protected bool isMiniGameComplete = false;

    private int elapsedTime = 0;

    void Start()
    {
        OnStart(); // You will need to call this OnStart method in every child class, because Unity is Unity :D 
    }

    protected void OnStart()
    {
        tipText.gameObject.SetActive(true);

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
            elapsedTime += (int)Time.deltaTime;

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

    public virtual void StartMiniGame()
    {
        elapsedTime = 0;

        InvokeRepeating(nameof(CountElapsedTime), 0f, 1f); // Start counting elapsed time every second

        gameObject.SetActive(true);
        //GameManager.Instance.isMinigameActive = true;

        isMiniGameActive = true;
        isMiniGameComplete = false;
        firstActionTriggered = false;

        timeSinceLastClick = 0f;

        if (tipText != null) tipText.gameObject.SetActive(true);

        OnMiniGameStart?.Invoke();
    }

    public virtual void EndMiniGame()
    {
        OnMiniGameEnd?.Invoke();

        if (isMiniGameComplete)
        {
            OnMiniGameComplete?.Invoke();
        }

        GameManager.Instance.isMinigameActive = false;
        gameObject.SetActive(false);
        isMiniGameActive = false;
        isMiniGameComplete = false;
        firstActionTriggered = false;

        CancelInvoke(nameof(CountElapsedTime)); // Stop counting elapsed time
        //Analytics.Instance.AddAnalytics(Time.time, gameObject.name, "Time to finish: ", elapsedTime.ToString());

    }

    private void CountElapsedTime()
    {
        elapsedTime += 1;
    }
}