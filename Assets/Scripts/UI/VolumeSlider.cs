using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSlider : EventTrigger
{
    private Slider slider;

    private enum VolumeType
    {
        SFX,
        Music,
        UISFX
    }

    [SerializeField] private VolumeType volumeType;

    void Start()
    {
        slider = GetComponent<Slider>();

        // Initialize the slider value based on the current volume
        switch (volumeType)
        {
            case VolumeType.SFX:
                slider.value = PlayerPrefs.GetFloat("SFXVolume");
                break;
            case VolumeType.Music:
                slider.value = PlayerPrefs.GetFloat("MusicVolume");
                break;
            case VolumeType.UISFX:
                slider.value = PlayerPrefs.GetFloat("UISFXVolume");
                break;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        switch (volumeType)
        {
            case VolumeType.SFX:
                SoundManager.Instance.PlaySFXSound(0);
                break;
            case VolumeType.UISFX:
                SoundManager.Instance.PlayUISound(0);
                break;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        switch (volumeType)
        {
            case VolumeType.SFX:
                SoundManager.Instance.PlaySFXSound(0);
                break;
            case VolumeType.UISFX:
                SoundManager.Instance.PlayUISound(0);
                break;
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        float normalizedValue = slider.value;

    }
}