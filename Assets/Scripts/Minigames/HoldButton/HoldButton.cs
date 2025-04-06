using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Rules")]
    [Range(0,1)] [SerializeField] private float fillSpeed;
    [SerializeField] private Image.FillMethod fillMethod;
    [Range(0,1)] [SerializeField] private int mode;

    [Header("Variables")]
    private bool isHolding = false;
    public float progress;

    [Header("Components")]
    [SerializeField] private Image fill;
    [SerializeField] private Image icon;
    [SerializeField] PetInteract petCheck;
    [Header("Debug Variables")]
    [SerializeField] private int target;

    void Start()
    {
        fill.fillMethod = fillMethod;
        icon.fillMethod = fillMethod;
        fill.fillAmount = 0;

        if (mode == 0) {icon.fillAmount = 0;}
    }
    private void OnEnable()
    {
        fill.fillMethod = fillMethod;
        icon.fillMethod = fillMethod;
        fill.fillAmount = 0;

        if (mode == 0) {icon.fillAmount = 0;}
    }
    void LateUpdate()
    {
        if (isHolding && mode == 0)
        {
            progress = fill.fillAmount;

            fill.fillAmount += fillSpeed * Time.deltaTime;
            icon.fillAmount += fillSpeed * Time.deltaTime;
        }

        if (progress >= 1)
        {
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
            icon.fillAmount = 0;
        }
    }

    public void EndMiniGame()
    {
        petCheck.CompleteTask(target);
        progress = 0;
        fill.fillAmount = 0;
        icon.fillAmount = 0;
        this.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mode == 1)
        {
        fill.fillAmount += fillSpeed * Time.deltaTime;

        progress = fill.fillAmount;
        }

    }
}
