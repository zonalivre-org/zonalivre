using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject settingsPanel;

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
    #region Animations

    private void SideToMid(GameObject destinationpanel)
    {
        sidePanel.SetActive(false);
        destinationpanel.SetActive(true);
        destinationpanel.GetComponent<Animator>().Play("ToMid");
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
    
    #endregion
}
