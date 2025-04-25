using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InGameProgress : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private int levelTime;

    [Header("Score Goal")]
    [SerializeField] private int goal;

    [Header("Pet Sliders")]
    [SerializeField] private int health = 100;
    [SerializeField] private float healthMultiplier = 1;
    [SerializeField] private int stamina = 100;
    [SerializeField] private float staminaMultiplier = 1;
    [SerializeField] private int happyness = 100;
    [SerializeField] private float happynessMultiplier = 1;

    [Header("UI components")]
    [SerializeField] private Slider clockSlider;
    [SerializeField] private TextMeshProUGUI clockNumber;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider happynessSlider;
    [SerializeField] private GameObject resultUI;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject nextLevelButton;
    [Header("References")]
    [SerializeField] private PlayerController playerMovement;
    [SerializeField] private DogMovement petMovement;

    // Constant Values
    private float currentTime, currentHealth, currentStamina, currentHappyness, wishHealthMultiplier, wishStaminaMultiplier, wishHappynessMultiplier;
    private int clockMultiplier = 1;
    private float sliderDelayTimer = 0f, clockDelayTimer = 0f;
    private int scoreProgress = 0;
    private bool enablecountdown = true;
    private int win = 0; // Player starts the game in a neutral state. +1 = they win. -1 = they lose.
    private void Awake()
    {
        //Setup Timer
        currentTime = levelTime;
        clockSlider.maxValue = levelTime;
        clockNumber.text = levelTime.ToString();

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
    private void LateUpdate()
    {
        if(currentHealth <= 0|| currentStamina <= 0|| currentHappyness <= 0 || currentTime <= 0) win = -1;

        if(enablecountdown)
        {
            currentTime -= Time.deltaTime * clockMultiplier;

            currentHealth -= Time.deltaTime * wishHealthMultiplier;
            currentStamina -= Time.deltaTime * wishStaminaMultiplier;
            currentHappyness -= Time.deltaTime * wishHappynessMultiplier;

            if(wishHealthMultiplier <= 0|| wishStaminaMultiplier <= 0|| wishHappynessMultiplier <= 0)
            {
                sliderDelayTimer += Time.deltaTime;
                if(sliderDelayTimer >= 1.5f)
                {
                    wishHealthMultiplier = healthMultiplier;
                    wishStaminaMultiplier = staminaMultiplier;
                    wishHappynessMultiplier = happynessMultiplier;
                    sliderDelayTimer = 0f;
                }
            }

            if(clockMultiplier <= 0)
            {
                clockDelayTimer += Time.deltaTime;
                if(clockDelayTimer >= 0.7f)
                {
                    clockMultiplier = 1;
                    clockDelayTimer = 0f;
                }
            }
        }
        UpdateUI();
    }
    private void UpdateUI()
    {
        clockSlider.value = currentTime;
        clockNumber.text = Mathf.Round(currentTime).ToString();

        healthSlider.value = currentHealth;
        staminaSlider.value = currentStamina;
        happynessSlider.value = currentHappyness;

        if(win != 0) ShowResultPanel(win);
    }
    private void ShowResultPanel(int state)
    {
        if (state != 0)
        {
            enablecountdown = false;
            playerMovement.ToggleMovement(false);
            petMovement.SetAutonomousMovement(false);
            resultUI.SetActive(true);
        }
        else Debug.Log("The results panel was called but the players hasn't won or lost yet!");

        if (state > 0) resultText.text = "Parabéns! Você venceu!";
        else if(state < 0) 
        {
            nextLevelButton.SetActive(false);
            resultText.text = "Oh não! Voce perdeu!";
        }
    } 
    public void AddScore(int score)
    {
        scoreProgress += score;
        if(score > 0) clockMultiplier = 0;
        if(scoreProgress >= goal) win = 1;
    }
    public void AddSliderValue(int amount, int which)
    {
        if(which == 0)
        {
            wishHealthMultiplier = 0f;
            currentHealth += amount;
        }
        else if(which == 1)
        {
            wishStaminaMultiplier = 0f;
            currentStamina += amount;
        }
        else if(which == 2) 
        {
            wishHappynessMultiplier = 0f;
            currentHappyness += amount;
        }
        else Debug.Log(amount + " is not a valid Pet slider number!");

        UpdateUI();
    }
}
