using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour
{
    [Tooltip("Se verdadeiro, pausa o jogo. Se falso, despausa.")]
    [SerializeField] private bool pauseOnClick = true;

    public void TogglePauseManager(bool toggle)
    {
        toggle = !toggle;
        PauseManager.Instance.TogglePause(toggle);
    }
}