using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WhackAMole : MiniGameBase, IPointerDownHandler
{
    [Header("Rules")]
    [Range(1,5)] [SerializeField] private int scoreToWin = 3;
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("Components")]
    [SerializeField] private RectTransform[] holes;
    [SerializeField] private Sprite chickenSprite;
    [SerializeField] private Sprite trashSprite;
    [SerializeField] private Image[] scorePoints;
    [SerializeField] private Sprite checkSprite;
    [SerializeField] private Sprite crossSprite;
    [HideInInspector] public ObjectiveInteract objectiveInteract;
    private GameObject lastTarget;

    [Header("Variables")]
    private int currentScore = 0;

    void OnEnable()
    {
        StartMiniGame();
    }

    void OnDisable()
    {
        EndMiniGame();
    }
    private void SpawnTarget()
    {
        Destroy(lastTarget);

        // Randomly select a hole
        int randomIndex = Random.Range(0, holes.Length);
        RectTransform selectedHole = holes[randomIndex];

        // Randomly select a target type (chicken or trash)
        bool isChicken = Random.Range(0, 2) == 0;

        // Set the sprite based on the target type
        Sprite targetSprite = isChicken ? chickenSprite : trashSprite;

        // Create the target GameObject
        GameObject target = new GameObject("Target");
        target.transform.SetParent(selectedHole);

        // Set RectTransform to stretch and fill the parent (hole)
        RectTransform rect = target.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;

        // Add an Image component to the target
        Image image = target.AddComponent<Image>();
        image.sprite = targetSprite;
        image.preserveAspect = true;

        //image.rectTransform.position = new Vector3(selectedHole.position.x, selectedHole.position.y - -35.4f, 0);

        lastTarget = target;
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();

        currentScore = 0;

        InvokeRepeating(nameof(SpawnTarget), spawnInterval, spawnInterval);

        //Activate the score points and set them to red
        for (int i = 0; i < scorePoints.Length; i++)
        {
            scorePoints[i].sprite = crossSprite;

            if (i < scoreToWin)
            {
                scorePoints[i].gameObject.SetActive(true);
            }
                
            else
            {
                scorePoints[i].gameObject.SetActive(false);
            }
        }
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) objectiveInteract.CompleteTask();
        else objectiveInteract.CloseTask(); 

        base.EndMiniGame();

        CancelInvoke(nameof(SpawnTarget));

        Destroy(lastTarget);

        lastTarget = null;
    }

    public void SetMinigameRules(int scoreToWin, float spawnInterval)
    {
        this.scoreToWin = scoreToWin;
        this.spawnInterval = spawnInterval;
    }

    public void ResetMinigame()
    {
        currentScore = 0;
        Destroy(lastTarget);
        lastTarget = null;

        for (int i = 0; i < scoreToWin; i++)
        {
            scorePoints[i].sprite = crossSprite;
        }
    }

    private void AddScore()
    {
        currentScore++;
        scorePoints[currentScore - 1].sprite = checkSprite;

        AudioManager.Instance.PlaySFXSound(5);

        if (currentScore >= scoreToWin)
        {
            isMiniGameComplete = true;
            // Trigger the end of the minigame
            EndMiniGame();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the pointer is over a hole
        for (int i = 0; i < holes.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(holes[i], eventData.position))
            {
                RegisterPlayerClick();
                // If the target is a chicken, increment score
                if (holes[i].childCount > 0 && holes[i].GetChild(0).GetComponent<Image>().sprite == trashSprite)
                {
                    AddScore();
                }
                else if (holes[i].childCount > 0 && holes[i].GetChild(0).GetComponent<Image>().sprite == chickenSprite)
                {
                    AudioManager.Instance.PlaySFXSound(6);

                    // If the target is trash, decrement score
                    ResetMinigame();
                    
                }

                // Destroy the target
                if (holes[i].childCount > 0) Destroy(holes[i].GetChild(0).gameObject);
                break;
            }
        }
    }
}
