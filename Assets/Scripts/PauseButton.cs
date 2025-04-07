using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour
{
    private PlayerController player;
    private DogMovement pet;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        pet = FindObjectOfType<DogMovement>();
    }
    public void TogglePauseManager(bool toggle)
    {
        toggle = !toggle;
        //PauseManager.Instance.TogglePause(toggle);
        TogglePause(toggle);
    }
    public void TogglePause(bool pause)
    {
        if(pause) Time.timeScale = 0f;
        else Time.timeScale = 1f;
        player.ToggleMovement(!pause);
        pet.ToggleMovement(!pause);
    }
}