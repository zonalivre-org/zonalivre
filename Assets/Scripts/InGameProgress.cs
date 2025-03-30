using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InGameProgress : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private Slider clockSlider;
    [SerializeField] private TextMeshProUGUI clockNumber;
    [SerializeField] private int levelTime;
    [SerializeField] private int pointsToWin;
    [Header("Pet Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private int health = 100;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private int stamina = 100;
    [SerializeField] private Slider happynessSlider;
    [SerializeField] private int happyness = 100;
    [Header("UI components")]
    [SerializeField] GameObject resultUI;
    [SerializeField] TextMeshProUGUI resultText;
    private float currentTime;
    private float currentHealth;
    private float currentStamina;
    private float staminaMultiply = 1;
    private float currentHappyness;
    private float happynessMultiply = 1;
    private int scoreProgress = 0;
    private bool enablecountdown = true;
    private bool petishealthy = true;
    private bool win = false;
    private void Awake()
    {
        currentTime = levelTime;
        clockSlider.maxValue = levelTime;
        clockNumber.text = levelTime.ToString();

        currentHealth = health;
        healthSlider.maxValue = health;
        currentStamina = stamina;
        staminaSlider.maxValue = stamina;
        currentHappyness = happyness;
        happynessSlider.maxValue = happyness;
    }
    private void LateUpdate()
    {
        if(currentHealth <= 0) petishealthy = false;
         
        if(!petishealthy)
        {
            currentTime = 0;
            enablecountdown = false;
            win = false;
        }

        healthSlider.value = currentHealth;
        if(currentStamina > 0)
        {
            currentStamina -= (Time.deltaTime * 0.3f) * staminaMultiply;
            staminaSlider.value = currentStamina;
        }
        else currentHealth -= Time.deltaTime * happynessMultiply;
        if(currentHappyness > 0)
        {    
            currentHappyness -= (Time.deltaTime * 0.8f) * happynessMultiply;
            happynessSlider.value = currentHappyness;
        }
        else
        {
            staminaMultiply = 2;
            happynessMultiply = 2;
        }

        if(currentTime > 0 && enablecountdown)
        {
            currentTime -= Time.deltaTime;
            clockSlider.value = currentTime;
            clockNumber.text = Mathf.Round(currentTime).ToString();
        }
        else
        {
            ShowResultPanel();
        }
    }
    public void AddScore(int score)
    {
        scoreProgress += score;
        if (scoreProgress >= pointsToWin)
        {
            win = true;
            currentTime = 0;
        }
    }
    public void AddSliderValue(int amount, int which)
    {
        if(which == 0) healthSlider.value += amount;
        else if(which == 1) healthSlider.value += amount;
        else if(which == 2) healthSlider.value += amount;
        else Debug.Log(amount + " is not a valid slider number!");
    }

    private void ShowResultPanel()
    {
        resultUI.SetActive(true);
        if (win) resultText.text = "Parabéns! Você venceu!";
        else resultText.text = "Oh não! Voce perdeu!";
    } 
}
