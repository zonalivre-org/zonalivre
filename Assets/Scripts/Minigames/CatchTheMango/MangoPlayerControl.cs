using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MangoPlayerControl : MonoBehaviour, IPointerDownHandler

{
    private Outline playerOutline;
    private RectTransform playerRectTransform;
    [SerializeField] GameObject player;
    [SerializeField] RectTransform moveArea;
    [SerializeField] private float moveSpeed = 5f;
    [HideInInspector] public Vector2 destination;

    private void Start() {
        destination.y = 100;
        playerOutline = player.GetComponent<Outline>();
        playerRectTransform = player.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        MoveToDestination();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetDestination(eventData);

        MiniGameBase.OnMinigameInteract.Invoke();
    }

    private void SetDestination(PointerEventData eventData)
    {
        Vector2 localClickPosition;

        // Convert the screen-space position to local-space position relative to the moveArea
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            moveArea, // The RectTransform to use as a reference
            eventData.position, // The screen-space position of the click
            eventData.pressEventCamera, // The camera used for the UI (usually the event camera)
            out localClickPosition // The resulting local-space position
        );

        localClickPosition.y = playerRectTransform.anchoredPosition.y;

        localClickPosition.x = Mathf.Clamp(localClickPosition.x, moveArea.rect.min.x + (moveArea.anchoredPosition.x + playerRectTransform.rect.width / 2), moveArea.rect.max.x + (moveArea.anchoredPosition.x - playerRectTransform.rect.width / 2));

        destination = localClickPosition;
    }

    private void MoveToDestination()
    {
        if (Vector2.Distance(playerRectTransform.anchoredPosition, destination) > 0.1f)
        {
            playerOutline.enabled = true;
            Vector2 newPosition = Vector2.MoveTowards(playerRectTransform.anchoredPosition, destination, moveSpeed * Time.deltaTime);
            playerRectTransform.anchoredPosition = newPosition;
        }
        else
        {
            playerOutline.enabled = false;
        }
    }

    // public void OnDrag(PointerEventData eventData)
    // {

    //     MoveByClick(eventData);

    //     MiniGameBase.OnMinigameInteract.Invoke();
    // }

    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     playerOutline.enabled = false;
    // }

    // private void MoveByHolding(PointerEventData eventData)
    // {
    //     // Calculate the new position while dragging only in the x direction
    //     Vector2 newPosition = playerRectTransform.anchoredPosition + new Vector2(eventData.delta.x, 0);

    //     // Get the bottom left and right corner of the MoveArea, then sum with the MoveArea position according to the Canvas 
    //     // In this way, the MoveArea can be at any position of the canvas without breaking the allowed move border
    //     Vector2 minBounds = moveArea.rect.min + (Vector2)moveArea.anchoredPosition; 
    //     Vector2 maxBounds = moveArea.rect.max + (Vector2)moveArea.anchoredPosition;

    //     // Get player size (/2 because the pivot is in the center)
    //     Vector2 playerSize = playerRectTransform.rect.size / 2;

    //     // Keep the player in the area
    //     newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x + playerSize.x, maxBounds.x - playerSize.x);

    //     // Update his position, still keeping the original y value
    //     playerRectTransform.anchoredPosition = new Vector2(newPosition.x, playerRectTransform.anchoredPosition.y);
    // }
}
