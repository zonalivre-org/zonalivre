using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : EventTrigger
{
    [SerializeField] private Vector2 hoverSizePercentage = new Vector2(120f, 120f); // Percentage increase for width and height
    [SerializeField] private float animationDuration = 0.2f; // Duration of the scaling animation

    private Vector2 originalSize; // Store the original width and height of the button
    private Coroutine currentCoroutine; // To manage ongoing animations
    private RectTransform rectTransform; // Reference to the RectTransform

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // Get the RectTransform component
        originalSize = rectTransform.sizeDelta; // Save the original size
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine); // Stop any ongoing animation

        // Calculate the hover size based on the percentage
        Vector2 hoverSize = new Vector2(
            originalSize.x * (hoverSizePercentage.x / 100f),
            originalSize.y * (hoverSizePercentage.y / 100f)
        );

        currentCoroutine = StartCoroutine(ResizeTo(hoverSize));
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine); // Stop any ongoing animation

        currentCoroutine = StartCoroutine(ResizeTo(originalSize));
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine); // Stop any ongoing animation

        rectTransform.sizeDelta = originalSize; // Reset to original size immediately
    }

    private IEnumerator ResizeTo(Vector2 targetSize)
    {
        Vector2 startSize = rectTransform.sizeDelta;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.sizeDelta = targetSize; // Ensure the final size is set
    }
}