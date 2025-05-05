using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrashObject : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Rules")]
    private float cleanSpeed;
    private CleanMinigame cleanMinigame;

    [Header("Components")]
    private Sprite trashBagSprite;
    private Image trashImage;
    private RectTransform trashCan;
    private bool turnedIntoTrashBag = false;
    private Texture2D broomCursorTexture;
    private Texture2D handCursorTexture;
    void Start()
    {
        trashImage = GetComponent<Image>();
    }
    public void SetTrashProperties(float cleanSpeed, CleanMinigame cleanMinigame, Sprite trashBagSprite, RectTransform trashCan, Texture2D broomCursorTexture, Texture2D handCursorTexture)
    {
        this.cleanSpeed = cleanSpeed;

        this.cleanMinigame = cleanMinigame;

        this.trashBagSprite = trashBagSprite;

        this.trashCan = trashCan;

        this.broomCursorTexture = broomCursorTexture;

        this.handCursorTexture = handCursorTexture;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (turnedIntoTrashBag == false){
            trashImage.fillAmount -= cleanSpeed * Time.deltaTime;

            cleanMinigame.RegisterPlayerClick();

            if (trashImage.fillAmount <= 0f)
            {
                trashImage.sprite = trashBagSprite;
                cleanMinigame.ReduceTrashAmount();
                turnedIntoTrashBag = true;
                trashImage.fillAmount = 1f;
                Cursor.SetCursor(handCursorTexture, Vector2.zero, CursorMode.Auto);
                GetComponent<Outline>().enabled = true;
            }
        }

        else
        {
            cleanMinigame.RegisterPlayerClick();
            Vector2 mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(trashCan, mousePosition, eventData.pressEventCamera, out Vector2 localPoint);
            transform.position = trashCan.TransformPoint(localPoint);

            if (RectTransformUtility.RectangleContainsScreenPoint(trashCan, Input.mousePosition, eventData.pressEventCamera))
            {
                trashCan.GetComponent<Outline>().enabled = true;
            }
            else
            {
                trashCan.GetComponent<Outline>().enabled = false;
            }
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<Outline>().enabled = false;

            if (RectTransformUtility.RectangleContainsScreenPoint(trashCan, Input.mousePosition, eventData.pressEventCamera))
            {
                cleanMinigame.ReduceTrashBagAmount();
                trashCan.GetComponent<Outline>().enabled = false;
                Destroy(gameObject);
            }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (turnedIntoTrashBag)
        {
            GetComponent<Outline>().enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (turnedIntoTrashBag == false)
        {
            Cursor.SetCursor(broomCursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(handCursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
