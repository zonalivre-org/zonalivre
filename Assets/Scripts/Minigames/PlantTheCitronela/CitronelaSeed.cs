using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CitronelaSeed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("References")]
    [SerializeField] private PlantTheCitronela plantTheCitronela;
    public RectTransform dirt;
    public RectTransform moveArea;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.GetComponent<Outline>().enabled = true;
        dirt.GetComponent<Outline>().enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(moveArea, eventData.position, eventData.pressEventCamera))
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<Outline>().enabled = false;
        dirt.GetComponent<Outline>().enabled = false;
        if (RectTransformUtility.RectangleContainsScreenPoint(dirt, Input.mousePosition, eventData.pressEventCamera))
            {
            plantTheCitronela.TransformSeedIntoSappling();
                dirt.GetComponent<Outline>().enabled = false;
            gameObject.SetActive(false);
            }
    }
}
