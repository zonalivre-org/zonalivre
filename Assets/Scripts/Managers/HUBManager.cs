using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HUBManager : MonoBehaviour
{
    [HideInInspector] public static HUBManager Instance;
    [SerializeField] GameObject player;
    [SerializeField] GameObject videoPanel;
    [SerializeField] Image fadeImage;
    [SerializeField] ChangeScene changeScene;
    [SerializeField] SaveFile saveFile;
    [SerializeField] PopUp popUp;

    [Header("Level Selection")]
    [SerializeField] RectTransform levelPanel;
    [SerializeField] RectTransform levelSelectionPanel;
    [SerializeField] CanvasGroup backgroundCanvasGroup;
    [SerializeField] Image levelImage;
    [SerializeField] TMP_Text levelNameText;
    [SerializeField] GameObject[] HUDButtons;
    private int currentLevelIndex;
    private string currentCutSceneClipName;
    private string currentCutSceneName;

    [SerializeField] private GameObject map;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(0f, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
        }).SetUpdate(true);

        saveFile = SaveManager.Instance.LoadGame();

        if (saveFile.firstTime)
        {
            popUp.SetVideoPlayer("WalkingVet.mp4");
            popUp.SetPopUp("Bem Vindo!", "Você se econtra na cidade onde mora Pedrinho. Toque/clique no veterinário para iniciar o tutorial.");
            SaveManager.Instance.ToggleFirstTime(false);
        }

        changeScene.ChangeToSceneMusic(1);

        NavMeshAgent ag = player.GetComponent<NavMeshAgent>();

        ag.enabled = false;

        player.transform.position = saveFile.spawnPosition;

        ag.enabled = true;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Save File Spawn Point: " + saveFile.spawnPosition);
            Debug.Log("Player POS: " + player.transform.position);
        }
    }

    public void StartLevelSelection(int levelIndex, string cutSceneClipName, string cutSceneName)
    {
        saveFile = SaveManager.Instance.LoadGame();
        Debug.Log("Index do nível: " + levelIndex);
        if (saveFile.levelsUnlocked[levelIndex])
        {
            Debug.Log("Level " + levelIndex + " is unlocked.");
            currentLevelIndex = levelIndex;
            currentCutSceneClipName = cutSceneClipName;
            currentCutSceneName = cutSceneName;
            levelNameText.text = "Nível " + (levelIndex);

            if (saveFile.cutScenesUnlocked[levelIndex] && !saveFile.cutScenesWatched[levelIndex])
            {
                fadeImage.gameObject.SetActive(true);
                fadeImage.color = new Color(0f, 0f, 0f, 0f);
                fadeImage.DOFade(1f, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    videoPanel.GetComponent<VideoPanel>().SetVideoClip(cutSceneClipName);
                    videoPanel.SetActive(true);
                    videoPanel.GetComponent<VideoPanel>().cutSceneIndex = levelIndex;
                    videoPanel.GetComponent<VideoPanel>().levelToUnlock = levelIndex;
                    videoPanel.GetComponent<VideoPanel>().videoTitle.text = currentCutSceneName;
                    videoPanel.GetComponent<VideoPanel>().closeButton.GetComponent<ButtonAnimation>().SetClickable(false);
                    videoPanel.GetComponent<VideoPanel>().PlayVideoClip();
                    fadeImage.gameObject.SetActive(false);

                    OpenLevelSelection();
                }).SetUpdate(true);
                return;
            }
            else
            {
                OpenLevelSelection();
            }
        }
        else
        {
            Debug.Log("Level " + levelIndex + " is locked.");
        }
    }

    public void StartCurrentLevel()
    {
        changeScene.ChangeToSceneMusic(currentLevelIndex + 2);
        changeScene.LoadSceneDelay(currentLevelIndex + 2);
    }

    public void ReplayCurrentCutScene()
    {
        videoPanel.SetActive(true);
        videoPanel.GetComponent<VideoPanel>().closeButton.GetComponent<ButtonAnimation>().SetClickable(true);
        videoPanel.GetComponent<VideoPanel>().SetVideoClip(currentCutSceneClipName);
        videoPanel.GetComponent<VideoPanel>().PlayVideoClip();

    }

    public void OpenLevelSelection()
    {
        player.GetComponent<PlayerController>().ToggleMovement(false);
        videoPanel.GetComponent<VideoPanel>().videoTitle.text = currentCutSceneName;
        LevelSelectionIn();
    }

    public void CloseLevelSelection()
    {
        LevelSelectionOut();
    }

    private void LevelSelectionIn()
    {
        HideButtons();
        levelSelectionPanel.gameObject.SetActive(true);
        levelPanel.localScale = Vector3.zero;
        backgroundCanvasGroup.alpha = 0f;
        backgroundCanvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
        levelPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void LevelSelectionOut()
    {
        backgroundCanvasGroup.DOFade(0f, 0.5f).SetUpdate(true);
        levelPanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            player.GetComponent<PlayerController>().ToggleMovement(true);
            ShowButtons();
            levelSelectionPanel.gameObject.SetActive(false);
        });
    }

    private void HideButtons()
    {
        foreach (GameObject button in HUDButtons)
        {
            button.SetActive(false);
        }
    }

    private void ShowButtons()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (GameObject button in HUDButtons)
        {
            button.SetActive(true);
            button.transform.localScale = Vector3.zero;

            sequence.Append(
                button.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)
            );
        }
    }

    public void SendInfo()
    {
        Analytics.Instance.SendMail();
    }
}
