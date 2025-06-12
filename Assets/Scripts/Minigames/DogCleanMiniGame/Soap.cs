using UnityEngine;
using UnityEngine.EventSystems;

public class Soap : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform dragArea;
    [SerializeField] private DogCleanMiniGame dogCleanMiniGame;
    private bool isDragging = false;

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(dragArea, eventData.position, eventData.pressEventCamera))
        {
            transform.position = Input.mousePosition;
            isDragging = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DogDirt") && isDragging)
        {
            DogDirt dirt = other.GetComponent<DogDirt>();
            if (dirt != null)
            {
                dirt.CleanDirt();
            }
        }
    }
}
