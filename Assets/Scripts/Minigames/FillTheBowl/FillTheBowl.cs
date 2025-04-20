using UnityEngine;
using UnityEngine.UI;

public class FillTheBowl : MiniGameBase
{
    [Header("Rules")]
    [Range(0,1)] [SerializeField] private float fillSpeed;

    [Header("Components")]
    [SerializeField] private Image rationFill;
    [SerializeField] private PetInteract petCheck;

    [Header("Variables")]
    private float progress;

    void OnEnable()
    {
        StartMiniGame();
    }

    void Update()
    {
        TipCheck();
    }

    public void FillBowl()
    {
        if (progress < 1f)
        {
            rationFill.fillAmount += 0.1f * fillSpeed;

            progress = rationFill.fillAmount;

            SoundManager.Instance.PlayRandomPitchSFXSound(2);
        }

        else
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();

        OnStart();

        rationFill.fillAmount = 0f;

        progress = 0f;
        
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) petCheck.CompleteTask(1);
        else petCheck.CancelTask();

        base.EndMiniGame();
        gameObject.SetActive(false);
    }
}
