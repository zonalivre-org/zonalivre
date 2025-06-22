using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : MonoBehaviour
{
    private Image backgroundImage;
    public RectTransform namePanel;
    public TMP_Text nameText;

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        StartAnimation();
    }

    private void StartAnimation()
    {
        namePanel.transform.localScale = Vector3.zero;
        // Animate the background color to fully opaque
        backgroundImage.DOColor(new Color(0, 0, 0, 0.5f), 0.5f)
            .OnComplete(() =>
            {
                namePanel.transform.DOScale(Vector3.one, 0.5f)
                    .OnComplete(() =>
                    {
                        // Optionally, you can add more animations or logic here
                    });
            });
    }

    public void ConfirmName()
    {
        if (string.IsNullOrEmpty(nameText.text) || nameText.text.Length < 1)
        {
            return;
        }

        SaveManager.Instance.SetPlayerName(nameText.text);
        gameObject.SetActive(false);
    }
}
