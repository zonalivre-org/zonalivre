using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPanel : MonoBehaviour
{
    public string clipPath;
    public TMP_Text videoTitle;
    public VideoPlayer player;
    public RenderTexture renderTexture;
    public Slider videoDuration, videoVolume;
    public bool trigger;

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
        }
    }

    private void ShowPanel()
    {
        transform.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void SetVideoClip(string clipPath)
    {
        this.clipPath = clipPath;
        player.url = this.clipPath;
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
        }
        else
        {
            player.Play();
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
