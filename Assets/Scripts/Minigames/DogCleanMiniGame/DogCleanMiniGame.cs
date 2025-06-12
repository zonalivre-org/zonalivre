using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogCleanMiniGame : MiniGameBase
{
    [Header("Rules")]
    [Range(1, 7)][SerializeField] private int maxDirtCount = 4; // Maximum number of dirt spots
    [HideInInspector] public int currentDirtCount = 4;
    [Range(0f, 1f)][SerializeField] private float cleanSpeed = 0.5f;

    [Header("Components")]
    [SerializeField] private Image[] dogDirts;
    [HideInInspector] public ObjectiveInteract objectiveInteract;
    [SerializeField] private Soap soap;
    private Vector2 soapStartPos;

    void Awake()
    {
        soapStartPos = soap.GetComponent<RectTransform>().anchoredPosition;
    }
    
    public override void StartMiniGame()
    {
        base.StartMiniGame();

        OnStart();

        soap.GetComponent<RectTransform>().anchoredPosition = soapStartPos;

        // Initialize the dirt count
        currentDirtCount = maxDirtCount;

        // Deactivate all dirts first
        foreach (var dirt in dogDirts)
        {
            dirt.gameObject.SetActive(false);
            Color color = dirt.color;
            color.a = 1f; // Fully visible dirt
            dirt.color = color;
            dirt.GetComponent<DogDirt>().cleanSpeed = cleanSpeed;
        }

        ActivateRandomDirts();
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) objectiveInteract.CompleteTask();
        else objectiveInteract.CloseTask();

        base.EndMiniGame();
    }

    public void SetMiniGameRules(int maxDirtCount, float cleanSpeed)
    {
        this.maxDirtCount = maxDirtCount;
        this.cleanSpeed = cleanSpeed;

        // Update the dirt spots based on the new rules
        foreach (var dirt in dogDirts)
        {
            dirt.GetComponent<DogDirt>().cleanSpeed = cleanSpeed;
        }
    }

    private void ActivateRandomDirts()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < dogDirts.Length; i++)
        {
            if (!dogDirts[i].gameObject.activeSelf)
                availableIndices.Add(i);
        }

        int countToActivate = Mathf.Min(maxDirtCount, availableIndices.Count);
        for (int i = 0; i < countToActivate; i++)
        {
            int randomIdx = Random.Range(0, availableIndices.Count);
            int dirtIdx = availableIndices[randomIdx];
            dogDirts[dirtIdx].gameObject.SetActive(true);
            availableIndices.RemoveAt(randomIdx);
        }
    }

    public void DecreaseDirtCount()
    {
        currentDirtCount--;
        if (currentDirtCount <= 0)
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }
}
