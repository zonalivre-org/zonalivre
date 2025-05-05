using System;
using UnityEngine;
using UnityEngine.UI;

public class PetWalkMinigame : MiniGameBase
{
    [Header("Rules")]
    [SerializeField] private float arrivalTime;

    [Header("Components")]
    [SerializeField] private Slider walkSlider;
    [HideInInspector] public PetInteract petInteract;

    void OnEnable()
    {
        StartMiniGame();
    }

    void Update()
    {
        WalkPet();
        UpdateTipText();
    }

    public void WalkPet()
    {
        if (walkSlider.value < arrivalTime)
        {
            walkSlider.value += Time.deltaTime;
        }
        else
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }

    public void UpdateTipText()
    {
        tipText.text = $"Levando o pet ao veterinário. Aguarde <color=yellow>{Math.Round(arrivalTime - walkSlider.value)}</color> segundos.";
    }
    
    public override void StartMiniGame()
    {
        base.StartMiniGame();

        OnStart();

        walkSlider.value = 0f;
        walkSlider.maxValue = arrivalTime;

    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) petInteract.CompleteTask(1);
        else petInteract.CancelTask();

        isMiniGameActive = false;
        isMiniGameComplete = false;

        OnMiniGameEnd -= EndMiniGame;
        OnMinigameInteract -= RegisterPlayerClick;
    }
}
