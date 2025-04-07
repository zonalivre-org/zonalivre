using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonAnimation : EventTrigger
{
    [SerializeField] private float scaleFactor = 1.2f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}