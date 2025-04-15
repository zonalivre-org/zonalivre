using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MiniGameBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Rules")]
    [Range(0,1)] [SerializeField] private float fillSpeed;
    [SerializeField] private Mode mode;
    enum Mode {
        Hold,
        Drag
    }

    [Header("Variables")]
    private bool isHolding = false;
    public float progress;

    [Header("Components")]
    [SerializeField] private Image fill;
    [SerializeField] PetInteract petCheck;

    [Header("Debug Variables")]
    [SerializeField] private int target;

    private void OnEnable()
    {
        StartMiniGame();
    }

    void LateUpdate()
    {
        TipCheck();

        if (isHolding && mode == Mode.Hold)
        {
            fill.fillAmount += fillSpeed * Time.deltaTime;

            progress = fill.fillAmount;

            OnMinigameInteract.Invoke();
        }

        if (progress >= 1)
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        if (progress < 1 && mode == 0)
        {
            fill.fillAmount = 0;
        }
    }

    public override void StartMiniGame()
    {
        gameObject.SetActive(true);

        fill.fillAmount = 0;
        
        OnStart();

        if (mode == Mode.Hold)
        {
            fill.fillMethod = Image.FillMethod.Radial360;
            fill.fillOrigin = 3;
        }
        else
        {
            fill.fillMethod = Image.FillMethod.Vertical;
        }
    }
    public override void EndMiniGame()
    {
        if (isMiniGameComplete) petCheck.CompleteTask(target);
        else petCheck.CancelTask();

        progress = 0;
        fill.fillAmount = 0;
        isMiniGameActive = false;
        gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnMinigameInteract.Invoke();

        if (mode == Mode.Drag)
        {
            fill.fillAmount += fillSpeed * Time.deltaTime;

            progress = fill.fillAmount;
        }
    }
}
