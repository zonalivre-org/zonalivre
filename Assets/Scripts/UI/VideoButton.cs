using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class VideoButton : MonoBehaviour
{
    [HideInInspector] public LevelSelection levelSelection;
    [HideInInspector] public int cutSceneIndex;
    public int levelToUnlock;
    public string videoTitle;
    public string clipName;
    public VideoPanel videoPanel;

    public void OpenAndPlay()
    {
        string clipPath = string.Empty;

        bool[] cutScenesWatched = SaveManager.Instance.LoadGame().cutScenesWatched;

        videoPanel.levelSelection = levelSelection;

        if (cutScenesWatched[cutSceneIndex])
        {
            Debug.Log("Custscene already watched.");
            videoPanel.closeButton.GetComponent<ButtonAnimation>().SetClickable(true);

        }
        else
        {
            Debug.Log("First time watching this cutscene.");
            videoPanel.closeButton.GetComponent<ButtonAnimation>().SetClickable(false);

            videoPanel.levelToUnlock = levelToUnlock;
        }

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
