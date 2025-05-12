using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

public class VideoPanel : MonoBehaviour
{
    public string clipPath;
    public VideoPlayer player;
    public RenderTexture renderTexture;
    private void OnEnable()
    {
        ShowPanel();
    }
    private void ShowPanel()
    {
        Sequence sequence = DOTween.Sequence();

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
        player.Play();
    }

    public void ClosePanel()
    {
        player.url = null;
        renderTexture.Release();

        transform.DOScale(0, 0.2f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(Deactivate);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
