using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoskitoSlayer : MiniGameBase
{
    [Header("Rules")]
    [SerializeField] private int goal = 4;
    [SerializeField] private float moskitoSpeed = 50f;
    [SerializeField] private float spawnInterval = 1f;
    private int currentMoskitoCount = 0;

    [Header("Components")]
    [SerializeField] private GameObject moskitoPrefab;
    [SerializeField] private RectTransform[] spawnAreas;
    [HideInInspector] public ObjectiveInteract objectiveInteract;
    [SerializeField] private TMP_Text titleText;
    private List<GameObject> moskitos = new List<GameObject>();

    public override void StartMiniGame()
    {
        base.StartMiniGame();
        
        currentMoskitoCount = 0;

        UpdateScore();
        
        InvokeRepeating(nameof(SpawnMoskito), 0f, spawnInterval);
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) objectiveInteract.CompleteTask();
        else objectiveInteract.CloseTask();

        base.EndMiniGame();

        foreach (GameObject moskito in moskitos)
        {
            if (moskito != null)
                Destroy(moskito);
        }

        CancelInvoke(nameof(SpawnMoskito));
    }

    public void SetMiniGameRules(int goal, float moskitoSpeed, float spawnInterval)
    {
        this.goal = goal;
        this.moskitoSpeed = moskitoSpeed;
        this.spawnInterval = spawnInterval;
    }

    private void SpawnMoskito()
    {
        if (currentMoskitoCount >= goal)
            return;

        int randomIndex = Random.Range(0, spawnAreas.Length);
        RectTransform spawnArea = spawnAreas[randomIndex];

        Vector2 moskitoDirection = Vector2.right;
        if (randomIndex == 0) moskitoDirection = Vector2.right;
        else if (randomIndex == 1) moskitoDirection = Vector2.up;
        else if (randomIndex == 2) moskitoDirection = Vector2.left;

        Vector2 spawnPosition = new Vector2(
            0f,
            Random.Range(spawnArea.rect.yMin, spawnArea.rect.yMax)
        );

        GameObject moskito = Instantiate(moskitoPrefab, spawnPosition, Quaternion.identity);
        moskito.transform.SetParent(spawnArea, false);
        moskitos.Add(moskito);

        Moskito moskitoScript = moskito.GetComponent<Moskito>();
        moskitoScript.moskitoSlayer = this;
        moskitoScript.Initialize(moskitoDirection.normalized, moskitoSpeed);
    }

    public void MoskitoSlain()
    {
        currentMoskitoCount++;
        UpdateScore();
        if (currentMoskitoCount >= goal)
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }

    private void UpdateScore()
    {
        titleText.text = $"Toque nos mosquitos para afugentá-los. <color=yellow>{goal - currentMoskitoCount}</color> restantes.";
    }

}
