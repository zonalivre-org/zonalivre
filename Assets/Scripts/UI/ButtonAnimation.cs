using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonAnimation : EventTrigger
{
    [SerializeField] private float scaleFactor = 1.2f;
    private Vector3 originalScale;
    [SerializeField] private int clickSoundEffectIndex = 0;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlayRandomHoverSound();
        transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);

    }

    public override void OnPointerExit(PointerEventData eventData)
    {

        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);

    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlayUISound(clickSoundEffectIndex);
        transform.localScale = originalScale;
    }
}