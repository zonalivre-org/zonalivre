using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvent : MiniGameBase
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
    [SerializeField] private RectTransform moveArea;
    [SerializeField] private RectTransform safeZone;
    [SerializeField] private RectTransform pointer;
    [SerializeField] private GameObject[] points;
    [HideInInspector] public ObjectiveInteract objectivePlayerCheck;
    void Start()
    {
        // Set the panelWidth to the width of the MoveArea
        panelWidth = moveArea.rect.width;

        leftEdge = -panelWidth / 2;  // Left edge in local coordinates (center is 0)
        rightEdge = panelWidth / 2;  // Right edge in local coordinates

        // Set the initial pointer position at the left edge (local coordinates)
        pointer.localPosition = new Vector3(leftEdge, pointer.localPosition.y, pointer.localPosition.z);
    }

    private void OnEnable()
    {
        ResetPoints();
        StartMiniGame();
    }

    private void Update()
    {
        if (canRun)
        {
            MovePointer();
        }

    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();
        
        canRun = true;
        ResetPoints();
        SetSafeZone();
    }

    public override void EndMiniGame()
    {
        canRun = false;

        if (isMiniGameComplete) objectivePlayerCheck.CompleteTask();
        else objectivePlayerCheck.CloseTask();

        base.EndMiniGame();

        gameObject.SetActive(false);
    }

    public void SetMiniGameRules(int goal, float moveSpeed, float safeZoneSizePercentage)
    {
        this.goal = goal;
        this.moveSpeed = moveSpeed;
        this.safeZoneSizePercentage = safeZoneSizePercentage;
    }

    private void MovePointer()
    {
        float targetX = direction > 0 ? rightEdge : leftEdge;
        pointer.localPosition = Vector3.MoveTowards(pointer.localPosition, new Vector3(targetX, pointer.localPosition.y, pointer.localPosition.z), moveSpeed * Time.deltaTime);

        // Clamp the pointer's position to ensure it stays within the moveArea
        pointer.localPosition = new Vector3(
            Mathf.Clamp(pointer.localPosition.x, leftEdge, rightEdge),
            pointer.localPosition.y,
            pointer.localPosition.z
        );

        if (Mathf.Abs(pointer.localPosition.x - rightEdge) < 0.1f) direction = -1f;
        else if (Mathf.Abs(pointer.localPosition.x - leftEdge) < 0.1f) direction = 1f;
    }

    private void SetSafeZone()
    {
        // Ensure panelWidth is up-to-date
        panelWidth = moveArea.rect.width;

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
                isMiniGameComplete = true;
                EndMiniGame();
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

            AudioManager.Instance.PlayRandomPitchSFXSound(1);

            for (int i = 0; i < current; i++)
            {
                points[i].GetComponent<Image>().color = Color.green;
            }
        }
    }

    private void ResetPoints()
    {
        current = 0;

        // Hide all points and set their color to red
        for (int i = 0; i < points.Length; i++)
        {
            points[i].GetComponent<Image>().color = Color.red;
            points[i].SetActive(false);
        }

        // Active the points up to the goal
        for (int i = 0; i < goal; i++)
        {
            points[i].SetActive(true);
        }
    }
}