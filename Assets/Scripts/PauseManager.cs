using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    private PlayerController player;
    private DogMovement pet;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        player = FindObjectOfType<PlayerController>();
        pet = FindObjectOfType<DogMovement>();
    }
    public void TogglePause(bool pause)
    {
        if(pause) Time.timeScale = 0f;
        else Time.timeScale = 1f;
        player.ToggleMovement(!pause);
        pet.ToggleMovement(!pause);
    }
    public void Pause() => Time.timeScale = 0;
    public void Resume() => Time.timeScale = 1;
}