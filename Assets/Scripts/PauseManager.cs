using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private PlayerController player;
    private DogMovement pet;
    [SerializeField] private RectTransform pauseMenuUI;
    [SerializeField] private CanvasGroup backgroundCanvasGroup;
    [SerializeField] float animationDuration = 0.5f;
    [SerializeField] float startX;
    private void Awake()
    {
        Time.timeScale = 1;

        player = FindObjectOfType<PlayerController>();
        pet = FindObjectOfType<DogMovement>();
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        player.ToggleMovement(false);
        pet.ToggleMovement(false);

        PauseIn();

    }

    public async void ResumeGame()
    {
        await PauseOut();
        gameObject.SetActive(false);

        Time.timeScale = 1f;

        player.ToggleMovement(true);
        pet.ToggleMovement(true);  
    }

    private void PauseIn()
    {
        pauseMenuUI.localScale = Vector3.zero;
        backgroundCanvasGroup.alpha = 0f;
        backgroundCanvasGroup.DOFade(1f, animationDuration).SetUpdate(true);
        pauseMenuUI.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private async Task PauseOut()
    {
        backgroundCanvasGroup.DOFade(0f, animationDuration).SetUpdate(true);
        await pauseMenuUI.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack).SetUpdate(true).AsyncWaitForCompletion();
    }

}