using UnityEngine;
using UnityEngine.UI;

public class StatusFlashControl : MonoBehaviour
{
    [SerializeField] Animator healthAnimator, hungerAnimator, happinessAnimator;
    private bool healthFlash, hungerFlash, happinessFlash;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider happinessSlider;
    [Range(0,1)] [SerializeField] private float percentageToFlash = 0.5f;

    void Start()
    {
        InvokeRepeating("CheckStatus", 0f, 0.5f);
    }

    private void CheckStatus()
    {
        if (healthSlider.value <= percentageToFlash * healthSlider.maxValue)
        {
            healthAnimator.Play("HealthFlash");
            if (!healthFlash)
            {
                healthFlash = true;
                AudioManager.Instance.PlaySFXSound(4);
            }
        }
        else
        {
            healthAnimator.Rebind();
            healthAnimator.Play("Idle");
            healthFlash = false;
        }

        if (hungerSlider.value <= percentageToFlash * hungerSlider.maxValue)
        {
            hungerAnimator.Play("HungerFlash");
            if (!hungerFlash)
            {
                hungerFlash = true;
                AudioManager.Instance.PlaySFXSound(4);
            }
        }
        else
        {
            hungerAnimator.Rebind();
            hungerAnimator.Play("Idle");
            hungerFlash = false;
        }

        if (happinessSlider.value <= percentageToFlash * happinessSlider.maxValue)
        {
            happinessAnimator.Play("HappinessFlash");
            if (!happinessFlash)
            {
                happinessFlash = true;
                AudioManager.Instance.PlaySFXSound(4);
            }
        }
        else
        {
            happinessAnimator.Rebind();
            happinessAnimator.Play("Idle");
            happinessFlash = false;
        }
    }
}
