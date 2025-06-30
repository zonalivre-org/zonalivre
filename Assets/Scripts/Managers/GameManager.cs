using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    [Header("Save System")]
    public int levelIndex;
    public int levelToUnlock;

    [Header("Cutscene")]
    [SerializeField] private bool hasCutscene = false;
    [SerializeField] string cutsceneFileName;
    [SerializeField] string cutSceneTitle;
    [SerializeField] private VideoPanel cutsceneVideoPanel;
    [SerializeField] private int cutsceneIndex;

    [Header("Time")]
    [SerializeField] private int levelTime;

    [Header("Score Goal")]
    [SerializeField] private int goal;

    [Header("Pet Sliders")]
    public int health = 100;
    [SerializeField] private float healthMultiplier = 1;
    public int stamina = 100;
    [SerializeField] private float staminaMultiplier = 1;
    public int happyness = 100;
    [SerializeField] private float happynessMultiplier = 1;

    [Header("UI components")]
    [SerializeField] private Slider clockSlider;
    [SerializeField] private TextMeshProUGUI clockNumber;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private Slider happynessSlider;
    [SerializeField] private Image happynessFillImage;
    [SerializeField] private GameObject resultUI;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TMP_Text loseText;

    [Header("References")]
    public static GameManager Instance;
    [SerializeField] private PlayerController playerMovement;
    [SerializeField] private DogMovement petMovement;

    [Header("Constants and Variables")]
    public float currentTime, currentHealth, currentStamina, currentHappyness, wishHealthMultiplier, wishStaminaMultiplier, wishHappynessMultiplier;
    private int clockMultiplier = 1;
    private float sliderDelayTimer = 0f, clockDelayTimer = 0f;
    [SerializeField] private int scoreProgress = 0;
    private bool enablecountdown = false;
    private int win = 0; // Player starts the game in a neutral state. +1 = they win. -1 = they lose.
    public bool isMinigameActive = false;
    [HideInInspector] public bool timeCount = true;
    private bool ended = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //Setup Timer
        currentTime = levelTime;
        clockSlider.maxValue = levelTime;
        clockNumber.text = ConvertTimeToString(levelTime);

        //Setup Pet Sliders
        currentHealth = health;
        healthSlider.maxValue = health;

        currentStamina = stamina;
        staminaSlider.maxValue = stamina;

        currentHappyness = happyness;
        happynessSlider.maxValue = happyness;

        //Setup Slider Multipliers
        wishHealthMultiplier = healthMultiplier;
        wishStaminaMultiplier = staminaMultiplier;
        wishHappynessMultiplier = happynessMultiplier;

        switch (levelIndex)
        {
            case 0:
                AudioManager.Instance.PlayMusicWithFade(2, 2f);
                break;
            case 1:
                AudioManager.Instance.PlayMusicWithFade(3, 2f);
                break;
            case 2:
                AudioManager.Instance.PlayMusicWithFade(4, 2f);
                break;
            case 3:
                AudioManager.Instance.PlayMusicWithFade(5, 2f);
                break;
            case 4:
                AudioManager.Instance.PlayMusicWithFade(6, 2f);
                break;
            case 5:
                AudioManager.Instance.PlayMusicWithFade(7, 2f);
                break;
            default:
                AudioManager.Instance.PlayMusicWithFade(7, 2f);
                break;

        }
    }
    void Start()
    {
        goal = TaskManager.Instance.objectives.Count;

        Invoke(nameof(StartTimer), 3);
    }

    private void Update()
    {
        if (timeCount)
        {
            if (currentHealth <= 0 || currentStamina <= 0 || currentHappyness <= 0 || currentTime <= 0) win = -1;
            if (enablecountdown)
            {
                currentTime -= Time.deltaTime * clockMultiplier;

                currentHealth -= Time.deltaTime * wishHealthMultiplier;
                currentStamina -= Time.deltaTime * wishStaminaMultiplier;
                currentHappyness -= Time.deltaTime * wishHappynessMultiplier;

                if (wishHealthMultiplier <= 0 || wishStaminaMultiplier <= 0 || wishHappynessMultiplier <= 0)
                {
                    sliderDelayTimer += Time.deltaTime;
                    if (sliderDelayTimer >= 1.5f)
                    {
                        wishHealthMultiplier = healthMultiplier;
                        wishStaminaMultiplier = staminaMultiplier;
                        wishHappynessMultiplier = happynessMultiplier;
                        sliderDelayTimer = 0f;
                    }
                }

                if (clockMultiplier <= 0)
                {
                    clockDelayTimer += Time.deltaTime;
                    if (clockDelayTimer >= 0.7f)
                    {
                        clockMultiplier = 1;
                        clockDelayTimer = 0f;
                    }
                }
            }

            if (ended == false)
            {
                UpdateUI();
            }

        }
    }

    private void StartTimer()
    {
        enablecountdown = true;
    }

    private void UpdateUI()
    {
        clockSlider.value = currentTime;
        clockNumber.text = ConvertTimeToString(currentTime);

        UpdateDogStatsUI();

        if (win != 0) ShowResultPanel(win);
    }

    private void UpdateDogStatsUI()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        currentHappyness = Mathf.Clamp(currentHappyness, 0, 100);
        currentStamina = Mathf.Clamp(currentStamina, 0, 100);

        healthSlider.value = currentHealth;
        healthFillImage.fillAmount = healthSlider.value / healthSlider.maxValue;

        staminaSlider.value = currentStamina;
        staminaFillImage.fillAmount = staminaSlider.value / staminaSlider.maxValue;

        happynessSlider.value = currentHappyness;
        happynessFillImage.fillAmount = happynessSlider.value / happynessSlider.maxValue;
    }

    private void ShowResultPanel(int state)
    {
        ended = true;

        if (hasCutscene && SceneManager.GetActiveScene().buildIndex != 2)
        {
            cutsceneVideoPanel.gameObject.SetActive(true);
            cutsceneVideoPanel.SetVideoClip(cutsceneFileName);
            cutsceneVideoPanel.videoTitle.text = cutSceneTitle;
            cutsceneVideoPanel.cutSceneIndex = cutsceneIndex;
            cutsceneVideoPanel.gameObject.SetActive(true);
            cutsceneVideoPanel.PlayVideoClip();
        }

        if (SceneManager.GetActiveScene().buildIndex != 2) Time.timeScale = 0f;

        if (state != 0)
        {
            enablecountdown = false;
            playerMovement.ToggleMovement(false);
            petMovement.SetAutonomousMovement(false);
            if (resultUI != null && SceneManager.GetActiveScene().buildIndex != 2) resultUI.SetActive(true);
        }
        else Debug.Log("The results panel was called but the players hasn't won or lost yet!");

        if (state > 0)
        {
            if (resultUI != null) resultText.text = "Parabéns! Você venceu!";
        }

        else if (state < 0)
        {
            if (resultUI != null)
            {
                loseText.gameObject.SetActive(true);
                resultText.text = "Oh não! Voce perdeu!";
                if (healthSlider.value <= 0.0001) loseText.text = "Saúde do Pet zerada!";
                else if (staminaSlider.value <= 0.0001) loseText.text = "Fome do Pet zerada!";
                else if (happynessSlider.value <= 0.0001) loseText.text = "Felicidade do Pet zerada!";
                else if (clockSlider.value <= 0.0001) loseText.text = "Tempo zerado!";
                else loseText.text = "Motivo não listado! Vai resolver >:(";
            }
        }
    }

    public void AddScore(int score)
    {
        scoreProgress += score;
        if (score > 0) clockMultiplier = 0;
        if (scoreProgress >= goal) win = 1;
    }

    public void AddSliderValue(int amount, int which)
    {
        if (which == 0)
        {
            wishHealthMultiplier = 0f;
            currentHealth += amount;
        }
        else if (which == 1)
        {
            wishStaminaMultiplier = 0f;
            currentStamina += amount;
        }
        else if (which == 2)
        {
            wishHappynessMultiplier = 0f;
            currentHappyness += amount;
        }
        else Debug.Log(amount + " is not a valid Pet slider number!");

        UpdateUI();
    }

    private string ConvertTimeToString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Clamp minutes to a maximum of 99
        minutes = Mathf.Min(minutes, 99);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    #region CHEATS
    public void CompleteLevel()
    {
        win = 1;
        ShowResultPanel(win);
        timeCount = false;
    }

    #endregion
}
