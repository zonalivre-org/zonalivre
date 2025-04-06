using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Se verdadeiro, pausa o jogo. Se falso, despausa.")]
    [SerializeField] private bool pauseOnClick = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pauseOnClick)
            PauseManager.Instance.Pause();
        else
            PauseManager.Instance.Resume();
    }
}