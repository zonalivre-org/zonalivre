using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class WhackAMole : MiniGameBase, IPointerDownHandler
{
    [Header("Rules")]
    [Range(1, 5)][SerializeField] private int scoreToWin = 3;
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("Components")]
    [SerializeField] private RectTransform[] holes;
    [SerializeField] private Sprite[] chickenSprites;
    [SerializeField] private Sprite[] trashSprites;
    [SerializeField] private Image[] scorePoints;
    [SerializeField] private Sprite[] checkSprites;
    [SerializeField] private Sprite[] crossSprites;
    [HideInInspector] public ObjectiveInteract objectiveInteract;
    private GameObject lastTarget;

    [Header("Variables")]
    private int currentScore = 0;

    private enum ScoreState { None, Cross, Check }
    private ScoreState[] scorePointStates;

    private Coroutine scorePointsAnimationCoroutine;
    private int animationFrame = 0;
    private float frameDuration = 0.1f;

    void OnEnable()
    {
        StartMiniGame();
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
        Sprite[] targetSprites = isChicken ? chickenSprites : trashSprites;

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
        image.sprite = targetSprites[0];
        image.preserveAspect = true;

        TargetType type = target.AddComponent<TargetType>();
        type.isChicken = isChicken;

        // Animate the sprite of the target
        StartCoroutine(AnimateSprite(image, targetSprites));

        lastTarget = target;
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();

        currentScore = 0;

        InvokeRepeating(nameof(SpawnTarget), spawnInterval, spawnInterval);

        // Activate the score points and set them to red
        scorePointStates = new ScoreState[scorePoints.Length];

        for (int i = 0; i < scorePoints.Length; i++)
        {
            if (i < scoreToWin)
            {
                scorePoints[i].gameObject.SetActive(true);
                scorePointStates[i] = ScoreState.Cross;
            }
            else
            {
                scorePoints[i].gameObject.SetActive(false);
                scorePointStates[i] = ScoreState.None;
            }
        }

        // Start score point animation coroutine
        if (scorePointsAnimationCoroutine != null)
            StopCoroutine(scorePointsAnimationCoroutine);

        scorePointsAnimationCoroutine = StartCoroutine(AnimateScorePoints());
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) objectiveInteract.CompleteTask();
        else objectiveInteract.CloseTask();

        CancelInvoke(nameof(SpawnTarget));

        Destroy(lastTarget);

        lastTarget = null;

        if (scorePointsAnimationCoroutine != null)
        {
            StopCoroutine(scorePointsAnimationCoroutine);
            scorePointsAnimationCoroutine = null;
        }

        base.EndMiniGame();
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
            scorePointStates[i] = ScoreState.Cross;
        }
    }

    private void AddScore()
    {
        currentScore++;
        scorePointStates[currentScore - 1] = ScoreState.Check;

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

                if (holes[i].childCount > 0)
                {
                    var target = holes[i].GetChild(0).GetComponent<TargetType>();

                    // If the target is a chicken, increment score
                    if (!target.isChicken)
                    {
                        AddScore();
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFXSound(6);

                        // If the target is trash, decrement score
                        ResetMinigame();
                    }

                    // Destroy the target
                    Destroy(holes[i].GetChild(0).gameObject);
                }

                break;
            }
        }
    }

    private IEnumerator AnimateSprite(Image image, Sprite[] animationFrames, float frameDuration = 0.1f)
    {
        int frame = 0;
        while (image != null && image.gameObject != null)
        {
            image.sprite = animationFrames[frame];
            frame = (frame + 1) % animationFrames.Length;
            yield return new WaitForSeconds(frameDuration);
        }
    }

    private IEnumerator AnimateScorePoints()
    {
        while (true)
        {
            animationFrame = (animationFrame + 1) % crossSprites.Length;

            for (int i = 0; i < scorePoints.Length; i++)
            {
                if (!scorePoints[i].gameObject.activeSelf)
                    continue;

                switch (scorePointStates[i])
                {
                    case ScoreState.Cross:
                        scorePoints[i].sprite = crossSprites[animationFrame];
                        break;
                    case ScoreState.Check:
                        scorePoints[i].sprite = checkSprites[animationFrame];
                        break;
                }
            }

            yield return new WaitForSeconds(frameDuration);
        }
    }

    // Helper class to store target type
    private class TargetType : MonoBehaviour
    {
        public bool isChicken;
    }
}
