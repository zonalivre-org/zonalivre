using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : EventTrigger
{
    [SerializeField] private bool isClickable = true;
    [SerializeField] private GameObject buttonLock;
    [SerializeField] private float scaleFactor = 1.2f;
    private Vector3 originalScale;
    [SerializeField] private int clickSoundEffectIndex = 0;
    [SerializeField] private bool randomPitch = false;

    void Start()
    {
        if (isClickable)
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
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
        transform.localScale = originalScale;
        transform.DOKill(true);
        PlaySFX(clickSoundEffectIndex);
    }

    void OnDisable()
    {
        transform.localScale = originalScale;
        transform.DOKill(true);
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

    public void SetClickable(bool clickable)
    {
        isClickable = clickable;
        GetComponent<Button>().interactable = clickable;
        if (clickable)
        {
            buttonLock.SetActive(false);
        }
        else
        {
            buttonLock.SetActive(true);
        }
    }
}