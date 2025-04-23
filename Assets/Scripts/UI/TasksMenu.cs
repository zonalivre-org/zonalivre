using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TasksMenu : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private GameObject[] HUDButtons;
    [SerializeField] private CanvasGroup backgroundCanvasGroup;
    private PlayerController player;
    private DogMovement pet;
    public List<GameObject> items;

    [Header("Animation")]
    [SerializeField] private RectTransform taskMenuUI;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float Yposition;
    public float originalYPosition;

    void Awake()
    {
        originalYPosition = taskMenuUI.anchoredPosition.y;

        player = FindObjectOfType<PlayerController>();
        pet = FindObjectOfType<DogMovement>();
    }

    void OnEnable()
    {
        WriteItems();
    }

    private void WriteItems()
    {
        foreach (TaskItem task in TaskManager.Instance.GetTaskList())
        {
            task.transform.SetParent(content.transform, false);
        }
    }

    public void OpenTasksMenu()
    {
        HideButtons();
        gameObject.SetActive(true);

        Time.timeScale = 0f;

        player.ToggleMovement(false);
        pet.ToggleMovement(false);

        TasksIn();
    }

    public async void CloseTasksMenu()
    {
        await TasksOut();
        ShowButtons();
        gameObject.SetActive(false);

        Time.timeScale = 1f;

        player.ToggleMovement(true);
        pet.ToggleMovement(true);
    }


    #region Animation
    private void TasksIn()
    {
        backgroundCanvasGroup.alpha = 0f;
        backgroundCanvasGroup.DOFade(1f, animationDuration).SetUpdate(true);

        taskMenuUI.anchoredPosition = new Vector2(taskMenuUI.anchoredPosition.x, originalYPosition + Yposition);
        taskMenuUI.DOAnchorPosY(originalYPosition, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private async Task TasksOut()
    {
        backgroundCanvasGroup.DOFade(0f, animationDuration).SetUpdate(true);
        await taskMenuUI.DOAnchorPosY(Yposition, animationDuration).SetEase(Ease.InBack).SetUpdate(true).AsyncWaitForCompletion();
    }

    public void ShowButtons()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (GameObject button in HUDButtons)
        {
            button.SetActive(true);
            button.transform.localScale = Vector3.zero;
            sequence.Append(button.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true));
        }
    }

    public void HideButtons()
    {
        foreach (GameObject button in HUDButtons)
        {
            button.SetActive(false);
        }
    }
    #endregion
}