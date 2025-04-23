using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MiniGameBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Rules")]
    [Range(0, 1)][SerializeField] private float fillSpeed;
    [SerializeField] private Mode mode;
    enum Mode
    {
        Hold,
        Drag
    }

    [Header("Variables")]
    private bool isHolding = false;
    public float progress;

    [Header("Components")]
    [SerializeField] private Image fill;
    [SerializeField] PetInteract petCheck;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Texture2D handSprite;

    [Header("Debug Variables")]
    [SerializeField] private int target;

    private void OnEnable()
    {
        backgroundImage.GetComponent<Animator>().Play("RedDefault");
        fill.GetComponent<Animator>().Play("GreenDefault");

        Cursor.SetCursor(handSprite, new Vector2(0, 0), CursorMode.Auto);

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

        if (mode == Mode.Hold)
        {
            fill.fillAmount = 0;
            progress = 0;
        }

        else if (mode == Mode.Drag)
        {
            backgroundImage.GetComponent<Animator>().Play("RedDefault");
            fill.GetComponent<Animator>().Play("GreenDefault");
        }

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
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

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

            fill.GetComponent<Animator>().Play("DogPetGreen");
            backgroundImage.GetComponent<Animator>().Play("DogPetRed");

            fill.fillAmount += fillSpeed * Time.deltaTime;

            progress = fill.fillAmount;
        }
    }
}
