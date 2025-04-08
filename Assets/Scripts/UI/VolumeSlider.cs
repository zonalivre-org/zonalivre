using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
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
}