using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource sFXAudioSource;
    public AudioSource uISFXAudioSource;
    public AudioSource musicAudioSource;

    [Header("Audio Clips")]
    public AudioClip[] uIClips;
    public AudioClip[] sfxClips;
    public AudioClip[] musicClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("SFXVolume") && PlayerPrefs.HasKey("MusicVolume") && PlayerPrefs.HasKey("UISFXVolume"))
        {
            Debug.Log("Volume settings found, loading values.");
            LoadVolumeSettings();
        }
        else 
        {
            Debug.Log("No volume settings found, setting to default values.");
            SetMusicVolume(0.5f);
            SetSFXVolume(0.5f);
            SetUISFXVolume(0.5f);
            SaveVolumeSetting("MusicVolume", 0.5f);
            SaveVolumeSetting("SFXVolume", 0.5f);
            SaveVolumeSetting("UISFXVolume", 0.5f);
        }
    }

    #region Volume

    public void SetSFXVolume(float normalizedValue)
    {
        Debug.Log("Setting SFX Volume: " + normalizedValue);
        float dB = ConvertToLog10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)); // Convert normalized value to dB
        audioMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", normalizedValue); // Save normalized value
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float normalizedValue)
    {
        Debug.Log("Setting Music Volume: " + normalizedValue);
        float dB = ConvertToLog10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)); // Convert normalized value to dB
        audioMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat("MusicVolume", normalizedValue); // Save normalized value
        PlayerPrefs.Save();
    }

    public void SetUISFXVolume(float normalizedValue)
    {
        Debug.Log("Setting UISFX Volume: " + normalizedValue);
        float dB =ConvertToLog10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)); // Convert normalized value to dB
        audioMixer.SetFloat("UISFXVolume", dB);
        PlayerPrefs.SetFloat("UISFXVolume", normalizedValue); // Save normalized value
        PlayerPrefs.Save();
    }
    private void SaveVolumeSetting(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        float sfxVolume = ConvertToLog10(PlayerPrefs.GetFloat("SFXVolume"));
        float musicVolume = ConvertToLog10(PlayerPrefs.GetFloat("MusicVolume"));
        float uISFXVolume = ConvertToLog10(PlayerPrefs.GetFloat("UISFXVolume"));

        audioMixer.SetFloat("SFXVolume", sfxVolume);
        audioMixer.SetFloat("MusicVolume", musicVolume);
        audioMixer.SetFloat("UISFXVolume", uISFXVolume);
    }

    private float ConvertToLog10(float value)
    {
        if (value <= 0f) return -80f; // Return a very low value for zero or negative input
        return Mathf.Log10(value) * 20f; // Convert to dB
    }

    #endregion

    #region UI 

    public void PlayUISound(int index)
    {
        uISFXAudioSource.pitch = 1f;
        uISFXAudioSource.PlayOneShot(uIClips[index]);
    }

    public void PlayRandomPitchUISFXSound(int index)
    {
        float randomPitch = Random.Range(0.8f, 2f);

        uISFXAudioSource.pitch = randomPitch;
        uISFXAudioSource.PlayOneShot(uIClips[0]);
    }

    #endregion

    #region SFX

    public void PlaySFXSound(int index)
    {
        sFXAudioSource.pitch = 1f;
        sFXAudioSource.PlayOneShot(sfxClips[index]);
    }

    public void PlayRandomPitchSFXSound(int index)
    {
        float randomPitch = Random.Range(0.8f, 2f);

        sFXAudioSource.pitch = randomPitch;
        sFXAudioSource.PlayOneShot(sfxClips[index]);
    }

    #endregion

    #region Music

    public void PlayMusic(int index)
    {
        musicAudioSource.clip = musicClips[index];
        musicAudioSource.Play();
    }

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void PauseMusic()
    {
        musicAudioSource.Pause();
    }

    public void UnpauseMusic()
    {
        musicAudioSource.UnPause();
    }

    public void PlayMusicWithFade(int index, float fadeDuration)
    {
        musicAudioSource.clip = musicClips[index];

        float musicvolume = ConvertToLog10(PlayerPrefs.GetFloat("MusicVolume", 0f));

        audioMixer.SetFloat("MusicVolume", -80f);

        audioMixer.DOSetFloat("MusicVolume", musicvolume, fadeDuration);

        musicAudioSource.Play();

    }

    public void StopMusicWithFade(float fadeDuration)
    {
        float musicvolume = ConvertToLog10(PlayerPrefs.GetFloat("MusicVolume", 0f));

        audioMixer.DOSetFloat("MusicVolume", -80f, fadeDuration).OnComplete(() => musicAudioSource.Stop());
    }

    public void ChangeMusicWithFade(int index, float fadeDuration)
    {
        
        DOTween.Sequence()
            .Append(audioMixer.DOSetFloat("MusicVolume", -80f, fadeDuration))
            .OnComplete(() =>
            {
                musicAudioSource.clip = musicClips[index];
                musicAudioSource.Play();
                audioMixer.DOSetFloat("MusicVolume", ConvertToLog10(PlayerPrefs.GetFloat("MusicVolume")), fadeDuration);
            });
    }

    #endregion
}