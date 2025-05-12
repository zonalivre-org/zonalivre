using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine;
using DG.Tweening;

public class VideoButton : MonoBehaviour
{
    public string clipName;
    public VideoPanel videoPanel;

    public void OpenAndPlay()
    {
        string clipPath = System.IO.Path.Combine(Application.streamingAssetsPath, clipName);
        videoPanel.gameObject.SetActive(true);
        videoPanel.SetVideoClip(clipPath);
        videoPanel.PlayVideoClip();
    }
}
