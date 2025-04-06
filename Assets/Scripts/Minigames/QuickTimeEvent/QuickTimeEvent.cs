using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{
    [Header("Rules")]
    [SerializeField][Range(1, 5)] private int goal;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField][Range(0.01f, 1.0f)] private float safeZoneSizePercentage = 0.25f;

    [Header("Variables")]
    private int current;
    private float direction = 1f;
    private float leftEdge;
    private float rightEdge;
    private float panelWidth;
    private bool canRun = true;

    [Header("Components")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform safeZone;
    [SerializeField] private RectTransform pointer;
    [SerializeField] private GameObject[] points;
    [SerializeField] private GameObject miniGameParent;
    [SerializeField] private ObjectiveInteract objectivePlayerCheck;
    void Start()
    {
        // Get the Canvas scale factor (if applicable)
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;

        // Calculate the panel width adjusted for Canvas scale
        panelWidth = panel.rect.width * scaleFactor;
        leftEdge = -panelWidth / 2f;  // Left edge in local coordinates (center is 0)
        rightEdge = panelWidth / 2f;  // Right edge in local coordinates

        // Set the initial pointer position at the left edge (local coordinates)
        pointer.localPosition = new Vector3(leftEdge, pointer.localPosition.y, pointer.localPosition.z);

        ResetPoints();

        SetSafeZone();
    }

    private void Update()
    {
        if (canRun)
        {
            // Calculate the target position based on direction (local coordinates)
            float targetX = direction > 0 ? rightEdge : leftEdge;
            Vector3 targetPosition = new Vector3(targetX, pointer.localPosition.y, pointer.localPosition.z);

            // Move the pointer towards the target edge
            pointer.localPosition = Vector3.MoveTowards(
                pointer.localPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Reverse direction when reaching an edge
            if (Mathf.Abs(pointer.localPosition.x - rightEdge) < 0.1f)
            {
                direction = -1f;
            }
            else if (Mathf.Abs(pointer.localPosition.x - leftEdge) < 0.1f)
            {
                direction = 1f;
            }

        }
    }

    public void SetMiniGameRules(int goal, float moveSpeed, float safeZoneSizePercentage)
    {
        this.goal = goal;
        this.moveSpeed = moveSpeed;
        this.safeZoneSizePercentage = safeZoneSizePercentage;
    }

    public void StartQTEGame()
    {
        ResetPoints();
        SetSafeZone();
        canRun = true;
    }

    private void SetSafeZone()
    {
        // Calculate and set the safe zone size based on the percentage of the panel width
        float safeZoneWidth = panelWidth * safeZoneSizePercentage;
        safeZone.sizeDelta = new Vector2(safeZoneWidth, safeZone.sizeDelta.y);

        // Spawn the safe zone at a random position within the panel boundaries (local coordinates)
        float safeZoneHalfWidth = safeZoneWidth / 2f;
        float minX = leftEdge + safeZoneHalfWidth;
        float maxX = rightEdge - safeZoneHalfWidth;
        float randomX = Random.Range(minX, maxX);
        safeZone.localPosition = new Vector3(randomX, safeZone.localPosition.y, safeZone.localPosition.z);
    }

    public void CheckSuccess()
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointer.position, null))
        {
            AddPoint();
            SetSafeZone();

            if (current >= goal)
            {

            }

        }
        else
        {
            ResetPoints();
            SetSafeZone();

            pointer.localPosition = new Vector3(leftEdge, pointer.localPosition.y, pointer.localPosition.z);
        }
    }

    private void AddPoint()
    {
        if (current < goal)
        {
            current++;

            for (int i = 0; i < current; i++)
            {
                points[i].GetComponent<Image>().color = Color.green;
            }

            if (current >= goal)
            {
                EndMiniGame();
            }
        }
    }

    private void ResetPoints()
    {
        current = 0;

        for (int i = 0; i < goal; i++)
        {
            points[i].SetActive(true);
            points[i].GetComponent<Image>().color = Color.red;
        }
    }

    public void EndMiniGame()
    {
        canRun = false;
        miniGameParent.SetActive(false);
        objectivePlayerCheck.CompleteTask();
        
    }
}