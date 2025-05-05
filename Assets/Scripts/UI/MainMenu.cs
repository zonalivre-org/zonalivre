using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject[] mainButtons;

    private void Awake()
    {
#if UNITY_STANDALONE || UNITY_EDITOR

        exitButton.SetActive(true);
#else

        Destroy(exitButton);

#endif
    }

    #region controllers

    public void ChangeToMidPanel(GameObject panel)
    {
        SideToMid(panel);
    }

    public void BackToSide(GameObject midPanel)
    {
        MidToSide(midPanel);
    }

    public void ChangeToFull(GameObject fullPanel)
    {
        StartCoroutine(ActivateCorroutine(fullPanel, infoPanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length));
    }

    public void DeactivatePanel(GameObject actualPanel)
    {
        StartCoroutine(DeactivateCorroutine(actualPanel, "ToFull"));
    }

    public void BackToMid(GameObject midPanel)
    {
        midPanel.SetActive(true);
        midPanel.GetComponent<Animator>().Play("BackToMid");
    }

    public void ExitGame()
    {
        Debug.Log("Saindo o jogo");
        Application.Quit();
    }

    #endregion

    #region Animations

    private void SideToMid(GameObject destinationpanel)
    {
        destinationpanel.SetActive(true);
        destinationpanel.GetComponent<Animator>().Play("ToMid");

        DOTween.Sequence()
            .AppendInterval(0.1f)
            .OnComplete(() =>
            {
                sidePanel.SetActive(false);
            });
    }

    private void MidToSide(GameObject originPanel)
    {
        sidePanel.SetActive(true);
        StartCoroutine(DeactivateCorroutine(originPanel, "ToSide"));
    }

    private IEnumerator DeactivateCorroutine(GameObject panel, string stateName)
    {
        Animator animator = panel.GetComponent<Animator>();
        animator.Play(stateName);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;

        yield return new WaitForSeconds(animationDuration);

        panel.SetActive(false);
    }

    private IEnumerator ActivateCorroutine(GameObject destinyPanel, float time)
    {
        yield return new WaitForSeconds(time);

        destinyPanel.SetActive(true);
    }

    public void ShowButtons()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(1.30f);

        foreach (GameObject button in mainButtons)
        {
            if (button != null)
            {
                button.transform.localScale = Vector3.zero;
                button.SetActive(true);

                sequence.Append(
                    button.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack)
                );
            }
        }

    }
    #endregion
}
