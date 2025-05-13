using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine;
using DG.Tweening;
using System;

public class VideoButton : MonoBehaviour
{
    public string videoTitle;
    public string clipName;
    public VideoPanel videoPanel;

    public void OpenAndPlay()
    {
        string clipPath = string.Empty;

        if (clipName != String.Empty)
        {
            clipPath = System.IO.Path.Combine(Application.streamingAssetsPath, clipName);
        }
        else
        {
            Debug.LogError("Tá sem nome de video aí rapaz");
        }
        
        videoPanel.videoTitle.text = videoTitle;
        videoPanel.gameObject.SetActive(true);
        videoPanel.SetVideoClip(clipPath);
        videoPanel.PlayVideoClip();
    }
}
