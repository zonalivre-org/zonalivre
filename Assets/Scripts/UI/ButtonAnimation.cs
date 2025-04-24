using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonAnimation : EventTrigger
{
    [SerializeField] private float scaleFactor = 1.2f;
    private Vector3 originalScale;
    [SerializeField] private int clickSoundEffectIndex = 0;

    [SerializeField] private bool randomPitch = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        PlaySFX(0);
        transform.DOScale(scaleFactor, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        PlaySFX(clickSoundEffectIndex);
        transform.localScale = originalScale;
    }

    void OnDisable()
    {
        DOTween.Kill(transform, true);
    }

    private void PlaySFX(int index)
    {
        if (randomPitch)
        {
            AudioManager.Instance.PlayRandomPitchUISFXSound(index);
        }
        else
        {
            AudioManager.Instance.PlayUISound(index);
        }
    }

}