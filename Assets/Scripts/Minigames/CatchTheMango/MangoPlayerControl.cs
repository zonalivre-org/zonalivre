using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MangoPlayerControl : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler

{
    private Outline playerOutline;
    private RectTransform playerRectTransform;
    [SerializeField] RectTransform moveArea;
    private void Start() {
        playerOutline = GetComponent<Outline>();
        playerRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        playerOutline.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calculate the new position while dragging only in the x direction
        Vector2 newPosition = playerRectTransform.anchoredPosition + new Vector2(eventData.delta.x, 0);

        // Get the bottom left and right corner of the MoveArea, then sum with the MoveArea position according to the Canvas 
        // In this way, the MoveArea can be at any position of the canvas without breaking the allowed move border
        Vector2 minBounds = moveArea.rect.min + (Vector2)moveArea.anchoredPosition; 
        Vector2 maxBounds = moveArea.rect.max + (Vector2)moveArea.anchoredPosition;

        // Get player size (/2 because the pivot is in the center)
        Vector2 playerSize = playerRectTransform.rect.size / 2;

        // Keep the player in the area
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x + playerSize.x, maxBounds.x - playerSize.x);

        // Update his position, still keeping the original y value
        playerRectTransform.anchoredPosition = new Vector2(newPosition.x, playerRectTransform.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        playerOutline.enabled = false;
    }
}
