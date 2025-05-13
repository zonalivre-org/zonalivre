using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PopUp : MonoBehaviour
{
    public TMP_Text popUpTille;
    public TMP_Text popUpDescription;
    public GameObject background;
    public Action OnPopUpClosed;
    [SerializeField] private VideoPlayer videoPlayer;
    public bool reopened = false;

    public void SetPopUp(string title, string description)
    {
        background.SetActive(true);
        background.GetComponent<Image>().DOFade(0.5f, 0.5f).SetUpdate(true);
        popUpTille.text = title;
        popUpDescription.text = description;

        Time.timeScale = 0f;

        gameObject.SetActive(true);

        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void ReopenPopUp()
    {
        reopened = true;

        Time.timeScale = 0f;

        gameObject.SetActive(true);

        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void ClosePopUp()
    {
        background.GetComponent<Image>().DOFade(0f, 0.5f).SetUpdate(true);

        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
            background.SetActive(false);

            if (reopened)
            {
                reopened = false;
                return;
            }

            OnPopUpClosed?.Invoke();
        });
    }

    public void SetVideoPlayer(string fileName)
    {
        videoPlayer.targetTexture.Release();
        string pathToVideo = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);;
        videoPlayer.url = pathToVideo;
        videoPlayer.Play();
    }
}