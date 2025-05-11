using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PetDogMinigame : MiniGameBase, IPointerUpHandler, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Rules")]
    [Range(0, 3)][SerializeField] private float fillSpeed;

    [Header("Variables")]
    private bool isHolding = false;
    public float progress;

    [Header("Components")]
    [SerializeField] private Image fill;
    [HideInInspector] public PetInteract petInteract;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Texture2D handSprite;

    [Header("Debug Variables")]
    [SerializeField] private int target;

    private void OnEnable()
    {
        backgroundImage.GetComponent<Animator>().Play("RedDefault");
        fill.GetComponent<Animator>().Play("GreenDefault");

        StartMiniGame();
    }

    void LateUpdate()
    {
        TipCheck();

        if (isHolding)
        {
            fill.fillAmount += fillSpeed * Time.deltaTime;
            progress = fill.fillAmount;
        }

        if (progress >= 1)
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }

    public override void StartMiniGame()
    {
        gameObject.SetActive(true);

        fill.fillAmount = 0;

        OnStart();

        fill.fillMethod = Image.FillMethod.Vertical;
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) petInteract.CompleteTask(target);
        else petInteract.CancelTask();

        progress = 0;
        fill.fillAmount = 0;
        isHolding = false;
        base.EndMiniGame();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isHolding = true;
        OnMinigameInteract.Invoke();

        fill.GetComponent<Animator>().Play("DogPetGreen");
        backgroundImage.GetComponent<Animator>().Play("DogPetRed");

        fill.fillAmount += fillSpeed * Time.deltaTime;
        progress = fill.fillAmount;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up");
        isHolding = false;

        backgroundImage.GetComponent<Animator>().Play("RedDefault");
        fill.GetComponent<Animator>().Play("GreenDefault");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(handSprite, new Vector2(0, 0), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
