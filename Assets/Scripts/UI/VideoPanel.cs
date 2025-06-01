using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPanel : MonoBehaviour
{
    public GameObject closeButton;
    [HideInInspector] public int cutSceneIndex;
    [HideInInspector] public int levelToUnlock;
    [HideInInspector] public string clipPath;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public TMP_Text videoTitle;
    public VideoPlayer player;
    public RenderTexture renderTexture;
    public Slider videoDuration, videoVolume;
    public Sprite pausedButtonSprite, playButtonSprite;
    public Image playButtonImage;
    public bool ended;
    private SaveFile saveFile;

    void Awake()
    {
        player.SetTargetAudioSource(0, AudioManager.Instance.videoAudioSource);
    }

    void Start()
    {
        saveFile = SaveManager.Instance.LoadGame();
    }

    private void OnEnable()
    {
        AudioManager.Instance.PauseMusic();
        videoVolume.gameObject.SetActive(false);
        ShowPanel();
        InvokeRepeating("SyncSlider", 0.5f, 0.1f);
    }

    public void SyncSlider()
    {
        if (player.isPlaying)
        {
            videoDuration.value = (float)(player.time / player.length);
            if (videoDuration.value >= 0.9f && ended == false)
            {
                ended = true;
                SaveManager.Instance.SetCutSceneCompletion(cutSceneIndex, true);
                SaveManager.Instance.SetLevelLock(levelToUnlock, true);

                closeButton.GetComponent<ButtonAnimation>().SetClickable(true);
            }
        }
    }

    private void ShowPanel()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void SetVideoClip(string clipName)
    {
        if (clipName != String.Empty)
        {
            clipPath = System.IO.Path.Combine(Application.streamingAssetsPath, clipName);
        }
        else
        {
            Debug.LogError("Tá sem nome de video aí rapaz");
        }

        player.url = clipPath;
        Debug.Log("Caminho do arquivo: " + clipPath);
    }

    public void PlayVideoClip()
    {
        if (player.url != String.Empty)
        {
            player.Play();
        }
        else
        {
            Debug.LogError("Nome do vídeo ta errado ou não tem");
        }
    }

    public void ControlPause()
    {
        if (player.isPlaying)
        {
            player.Pause();
            playButtonImage.sprite = pausedButtonSprite;
        }
        else
        {
            player.Play();
            playButtonImage.sprite = playButtonSprite;
        }
    }

    public void ControlVolume()
    {
        if (videoVolume.gameObject.activeSelf)
        {
            videoVolume.gameObject.SetActive(false);
        }
        else
        {
            videoVolume.gameObject.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        player.url = null;

        videoDuration.value = 0;

        ended = false;

        renderTexture.Release();

        CancelInvoke();

        transform.DOScale(0, 0.2f).SetUpdate(true).OnComplete(Deactivate);
    }

    private void Deactivate()
    {
        AudioManager.Instance.UnpauseMusic();
        gameObject.SetActive(false);
    }
}
