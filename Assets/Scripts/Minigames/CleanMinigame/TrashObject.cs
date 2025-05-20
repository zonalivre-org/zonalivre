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
    private Sprite trashFullImage;
    private Image trashImage;
    private RectTransform trashCan;
    private bool turnedIntoTrashBag = false;
    private Texture2D broomCursorTexture;
    private Texture2D handCursorTexture;
    private RectTransform dragArea;

    void Start()
    {
        trashImage = GetComponent<Image>();
    }
    public void SetTrashProperties(float cleanSpeed, CleanMinigame cleanMinigame, Sprite trashBagSprite, Sprite trashFullSprite, RectTransform trashCan, Texture2D broomCursorTexture, Texture2D handCursorTexture, RectTransform dragArea)
    {
        this.dragArea = dragArea;
        
        this.cleanSpeed = cleanSpeed;

        this.cleanMinigame = cleanMinigame;

        this.trashBagSprite = trashBagSprite;

        this.trashFullImage = trashFullSprite;

        this.trashCan = trashCan;

        this.broomCursorTexture = broomCursorTexture;

        this.handCursorTexture = handCursorTexture;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!turnedIntoTrashBag)
        {
            trashImage.fillAmount -= cleanSpeed * Time.deltaTime;
            cleanMinigame.RegisterPlayerClick();

            if (trashImage.fillAmount <= 0.15f)
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

            // Get the mouse position in local space relative to the dragArea
            Vector2 mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragArea, mousePosition, eventData.pressEventCamera, out Vector2 localPoint);

            // Clamp the local position within the bounds of the dragArea
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(localPoint.x, dragArea.rect.xMin, dragArea.rect.xMax),
                Mathf.Clamp(localPoint.y, dragArea.rect.yMin, dragArea.rect.yMax)
            );

            // Convert the clamped local position back to world space and set the object's position
            transform.position = dragArea.TransformPoint(clampedPosition);

            // Highlight the trashCan if the object is over it
            if (trashCan.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(trashCan, Input.mousePosition, eventData.pressEventCamera))
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

            if (trashCan.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(trashCan, Input.mousePosition, eventData.pressEventCamera))
            {
                cleanMinigame.ReduceTrashBagAmount();
                trashCan.GetComponent<Outline>().enabled = false;
                trashCan.GetComponent<Image>().sprite = trashFullImage;
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
