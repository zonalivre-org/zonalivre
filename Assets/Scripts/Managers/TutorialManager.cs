using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Cutscene Settings")]
    [SerializeField] private VideoPanel cutsceneVideoPanel;
    [SerializeField] private string cutsceneFileName;
    [SerializeField] private string cutSceneTitle;
    [SerializeField] private int cutsceneIndex;
    #region References
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private GameObject taskButton;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PetInteract petInteract;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private ChangeScene changeScene;
    [SerializeField] private PopUp popUp;
    [SerializeField] private GameObject[] minigames;
    [SerializeField] private GameObject[] minigameIndicators;
    [SerializeField] private GameObject[] petItems;
    [SerializeField] private GameObject[] statusIcons;
    private bool skip = false; // Used to skip the tutorial in case of testing or debugging
    #endregion

    #region Tutorial Variables
    [SerializeField] private int currentStep = 0;
    [SerializeField] private int timesMoved = 0;
    [SerializeField] private int popUpsClosed = 0;
    [SerializeField] private int minigameCompleted = 0;
    [SerializeField] private int petMinigamesCompleted = 0;
    [SerializeField] private int itensCollected = 0;
    [SerializeField] private int petItensCollected = 0;
    [SerializeField] private List<PopUpData> tutorialPopUps = new List<PopUpData>();
    #endregion

    #region Replay System
    public int replayIndex = 0;
    private int replayStartIndex = 0;
    public int replayEndIndex = 0;
    public bool isReplaying = false;
    #endregion

    #region Unity Methods

    void Start()
    {
        InitializeGameState();
        SubscribeEvents();
        HideAllIndicators();
    }

    void Update()
    {
        if (isReplaying) return; // Prevent normal flow during replay

        HandleTutorialSteps();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion

    #region Tutorial Step Logic
    private void HandleTutorialSteps()
    {
        switch (currentStep)
        {
            case 0:
                Invoke(nameof(ShowFirstPopUp), 3f);
                currentStep++;
                break;
            case 1:
                if (popUpsClosed >= 1)
                {
                    currentStep++;
                    ShowPopUp(1);
                }
                break;
            case 2:
                if (popUpsClosed >= 2)
                {
                    minigames[0].GetComponent<BoxCollider>().enabled = false;
                    petInteract.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                    currentStep++;
                    ShowPopUp(2);
                    SetPupUpReplay(0, 2);
                }
                break;
            case 3:
                if (timesMoved >= 4)
                {
                    currentStep++;
                    minigames[0].GetComponent<BoxCollider>().enabled = true;
                    minigames[1].GetComponent<BoxCollider>().enabled = false;
                    minigames[1].GetComponent<ObjectiveInteract>().taskItem.gameObject.SetActive(false);
                    minigameIndicators[0].SetActive(true);
                    minigameIndicators[1].SetActive(false);
                    ShowPopUp(3);
                    SetPupUpReplay(3, 3);
                }
                break;
            case 4:
                if (minigameCompleted >= 1)
                {
                    currentStep++;
                    minigames[2].SetActive(true);
                    minigameIndicators[4].SetActive(true);
                    ShowPopUp(4);
                    SetPupUpReplay(4, 4);
                }
                break;
            case 5:
                if (itensCollected >= 1 && playerInventory.GetItemID() == "Tela")
                {
                    currentStep++;
                    minigameIndicators[2].SetActive(true);
                    minigameIndicators[4].SetActive(false);
                    minigames[4].GetComponent<BoxCollider>().enabled = true;
                    ShowPopUp(5);
                    SetPupUpReplay(5, 5);
                }
                break;
            case 6:
                if (minigameCompleted >= 2)
                {
                    currentStep++;
                    minigameIndicators[4].SetActive(false);
                    ShowPopUp(6);
                }
                break;
            case 7:
                if (popUpsClosed >= 7)
                {
                    currentStep++;
                    ShowPopUp(7);
                }
                break;
            case 8:
                if (popUpsClosed >= 8)
                {
                    currentStep++;
                    petInteract.gameObject.GetComponent<CapsuleCollider>().enabled = true;
                    minigameIndicators[3].SetActive(true);
                    minigameIndicators[5].SetActive(true);
                    petItems[0].SetActive(true);
                    ShowPopUp(8);
                    SetPupUpReplay(6, 8);
                    statusIcons[0].SetActive(true);
                    petInteract.canHeal = true;
                }
                break;
            case 9:
                if (petMinigamesCompleted >= 1)
                {
                    currentStep++;
                    petItems[1].SetActive(true);
                    minigameIndicators[5].SetActive(false);
                    minigameIndicators[6].SetActive(true);
                    ShowPopUp(9);
                    statusIcons[1].SetActive(true);
                    petInteract.canFeed = true;
                    petInteract.canHeal = false;
                    SetPupUpReplay(9, 9);
                }
                break;
            case 10:
                if (petMinigamesCompleted >= 2)
                {
                    currentStep++;
                    minigameIndicators[6].SetActive(false);
                    ShowPopUp(10);
                    statusIcons[2].SetActive(true);
                    petInteract.canPet = true;
                    petInteract.canFeed = false;
                }
                break;
            case 11:
                if (popUpsClosed >= 11)
                {
                    currentStep++;
                    ShowPopUp(11);
                    SetPupUpReplay(10, 11);
                }
                break;
            case 12: // Mao na massa
                if (petMinigamesCompleted >= 3)
                {
                    currentStep++;
                    ShowPopUp(12);
                    minigames[1].GetComponent<BoxCollider>().enabled = true;
                    minigameIndicators[1].SetActive(true);
                    minigameIndicators[4].SetActive(true);
                    minigames[4].SetActive(true);
                    minigames[4].GetComponent<ObjectiveInteract>().taskItem.gameObject.SetActive(true);
                    minigames[1].GetComponent<ObjectiveInteract>().taskItem.gameObject.SetActive(true);
                }
                break;
            case 13: // Tarefas
                if (popUpsClosed >= 13)
                {
                    currentStep++;
                    pauseMenu.HUDButtons.Add(taskButton);
                    pauseMenu.ShowButtons();
                    ShowPopUp(13);
                    SetPupUpReplay(12, 13);
                }
                break;
            case 14: // Finalmente acabou
                if (CheckCompleteMinigames() || skip == true)
                {
                    currentStep++;
                    ShowPopUp(14);
                    SetPupUpReplay(13, 14);

                    SaveManager.Instance.SetLevelCompletion(gameManager.levelIndex, true);
                    SaveManager.Instance.SetCutSceneLock(gameManager.levelToUnlock, true);
                    SaveManager.Instance.SetLevelLock(gameManager.levelToUnlock, true);

                    cutsceneVideoPanel.SetVideoClip(cutsceneFileName);
                    cutsceneVideoPanel.videoTitle.text = cutSceneTitle;
                    cutsceneVideoPanel.cutSceneIndex = cutsceneIndex;
                    cutsceneVideoPanel.gameObject.SetActive(true);
                    cutsceneVideoPanel.PlayVideoClip();

                    Time.timeScale = 1f; // Pause the game during the cutscene

                    Debug.Log(Time.timeScale+"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                }
                break;
            case 15: // Close tutorial
                if (popUpsClosed >= 15)
                {
                    changeScene.ChangeToSceneMusic(1);
                    changeScene.LoadSceneDelay(1);
                }
                break;
        }
    }
    #endregion

    #region PopUp Logic
    private void ShowPopUp(int popUpIndex)
    {
        if (popUpIndex >= 0 && popUpIndex < tutorialPopUps.Count)
        {
            var data = tutorialPopUps[popUpIndex];
            popUp.SetPopUp(data.title, data.description);
            popUp.SetVideoPlayer(data.videoFileName);
        }
    }

    private void ShowFirstPopUp()
    {
        ShowPopUp(0);
    }
    #endregion

    #region Replay System

    public void SetPupUpReplay(int startIndex, int endIndex)
    {
        replayStartIndex = startIndex;
        replayIndex = startIndex;
        replayEndIndex = endIndex;
    }

    public void ReplayPopUps()
    {
        if (replayStartIndex < 0 || replayEndIndex >= tutorialPopUps.Count)
        {
            return;
        }

        else
        {
            isReplaying = true;
            ShowReplayPopUp();
        }

    }

    private void ShowReplayPopUp()
    {
        if (replayIndex <= replayEndIndex)
        {
            var data = tutorialPopUps[replayIndex];
            popUp.SetPopUp(data.title, data.description);
            popUp.SetVideoPlayer(data.videoFileName);
            popUp.OnPopUpClosed += OnReplayPopUpClosed;
        }
        else
        {
            replayIndex = replayStartIndex;
            isReplaying = false;
        }
    }

    private void OnReplayPopUpClosed()
    {
        popUp.OnPopUpClosed -= OnReplayPopUpClosed;
        replayIndex++;
        ShowReplayPopUp();
    }
    #endregion

    #region Event Handlers
    private void OnDestinationReached() => timesMoved++;
    private void OnPopUpClosed() { if (isReplaying == false) popUpsClosed++; }
    private void OnMinigameCompleted() => minigameCompleted++;
    private void OnItemCollected(ItemData item) => itensCollected++;
    private void OnPetMinigameCompleted() => petMinigamesCompleted++;
    #endregion

    #region Helpers
    private void InitializeGameState()
    {
        gameManager.timeCount = false;
        gameManager.currentHappyness = gameManager.happyness / 2;
        gameManager.currentStamina = gameManager.stamina / 2;
        gameManager.currentHealth = gameManager.health / 2;

        petInteract.canHeal = false;
        petInteract.canFeed = false;
        petInteract.canPet = false;
    }

    private void HideAllIndicators()
    {
        foreach (var icon in statusIcons)
            icon.SetActive(false);
        foreach (var indicator in minigameIndicators)
            indicator.SetActive(false);
    }

    private void SubscribeEvents()
    {
        playerController.OnDestinationReached += OnDestinationReached;
        popUp.OnPopUpClosed += OnPopUpClosed;
        MiniGameBase.OnMiniGameComplete += OnMinigameCompleted;
        playerInventory.OnItemChanged += OnItemCollected;
        petInteract.OnMinigameComplete += OnPetMinigameCompleted;
    }

    private void UnsubscribeEvents()
    {
        playerController.OnDestinationReached -= OnDestinationReached;
        popUp.OnPopUpClosed -= OnPopUpClosed;
        MiniGameBase.OnMiniGameComplete -= OnMinigameCompleted;
        playerInventory.OnItemChanged -= OnItemCollected;
        petInteract.OnMinigameComplete -= OnPetMinigameCompleted;
    }

    private bool CheckCompleteMinigames()
    {
        int minigameCount = 0;
        foreach (GameObject item in taskManager.GetTaskListItems())
        {
            if (item.GetComponent<TaskItem>().isComplete)
                minigameCount++;
        }
        return minigameCount == taskManager.GetTaskListItems().Count;
    }

    public void CompleteTutorial()
    {
        skip = true; // Set skip to true to bypass the tutorial steps
        currentStep = 14; // Set to the last step to close the tutorial
        popUpsClosed = 14; // Simulate all pop-ups closed
    }

    #endregion
}
