using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    [Header("Save System")]
    [SerializeField] private int levelIndex;
    [SerializeField] private int levelToUnlock;
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

            UpdateUI();
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
        if (resultUI != null)
        {
            Time.timeScale = 0f;
            if (state != 0)
            {
                enablecountdown = false;
                playerMovement.ToggleMovement(false);
                petMovement.SetAutonomousMovement(false);
                resultUI.SetActive(true);
            }
            else Debug.Log("The results panel was called but the players hasn't won or lost yet!");

            if (state > 0)
            {
                resultText.text = "Parabéns! Você venceu!";

                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SetLevelCompletion(levelIndex, true);
                    SaveManager.Instance.SetCutSceneLock(levelToUnlock, true);
                    SaveManager.Instance.SetLevelLock(levelToUnlock, true);
                }
                else
                {
                    Debug.LogWarning("SaveManager is not present in the scene, cannot save game progress.");
                }
            }

            else if (state < 0)
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
