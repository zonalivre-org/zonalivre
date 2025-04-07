using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MangoCatch : MiniGameBase
{
    [Header("Rules Section")]
    [SerializeField] int goal;
    [SerializeField] int currentPoints;
    [SerializeField] float mangoFallSpeed;
    [SerializeField] float cooldownBetweenMangos;

    [Header("Components")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] RectTransform spawnArea;
    [SerializeField] GameObject mangoPrefab;
    [SerializeField] GameObject player;
    [SerializeField] GameObject startPanel, endPanel;
    public ObjectiveInteract objectivePlayerCheck;

    [Header("Variables")]
    private float actualTime, nextTime;
    [SerializeField] private bool canGenerate;
    [SerializeField] private Vector2 playerInitialPosition;
    private List<GameObject> spawnedMangos = new List<GameObject>();

    private void OnEnable()
    {
        StartMiniGame();
    }

    void Update()
    {
        TipCheck();

        if (canGenerate && firstActionTriggered == true)
        {
            if (actualTime < nextTime)
            {
                actualTime = Time.time;
            }
            else 
            {
                SpawnMango();
                nextTime = Time.time + cooldownBetweenMangos;
            }
        }
    }

    public override void StartMiniGame()
    {
        gameObject.SetActive(true);

        OnStart();

        UpdateScore();

        canGenerate = true;
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) objectivePlayerCheck.CompleteTask();
        else objectivePlayerCheck.CloseTask();

        MiniGameReset();

        gameObject.SetActive(false);
        //OnMiniGameEnd?.Invoke();
    }

    public void MiniGameReset()
    {
        canGenerate = false;
        currentPoints = 0;

        foreach (var mango in spawnedMangos.ToArray())
        {
            if (mango != null)
            {
                Destroy(mango);
            }
        }

        spawnedMangos.Clear();

        player.GetComponent<RectTransform>().anchoredPosition = playerInitialPosition;

        isMiniGameActive = false;
        isMiniGameComplete = false;
        firstActionTriggered = false;

    }
    
    public void SetMiniGameRules(int goal, float mangoFallSpeed, float cooldownBetweenMangos)
    {
        this.goal = goal;
        this.mangoFallSpeed = mangoFallSpeed;
        this.cooldownBetweenMangos = cooldownBetweenMangos;
    }

    private void SpawnMango()
    {
        //Get width and height from the SpawnArea panel
        float width = spawnArea.rect.width;
        float height = spawnArea.rect.height;

        //Set random coordinates within the panel area
        float randomX = Random.Range(-width / 2, width / 2);
        float randomY = Random.Range(-height / 2, height / 2);

        //Instantiate the mango inside the spawn area
        Vector3 randomPosition = new Vector3(randomX, randomY, 0);
        GameObject mango = Instantiate(mangoPrefab, spawnArea);

        // Reference the manager (this class here :D) to the mango generated
        mango.GetComponent<Mango>().mangoCatch = this;

        // Set the mango fall speed
        mango.GetComponent<Mango>().fallSpeed = this.mangoFallSpeed;

        //Set the mango position to the random position
        mango.GetComponent<RectTransform>().anchoredPosition = randomPosition;

        spawnedMangos.Add(mango);
    }

    public void AddPoint(int amount)
    {
        currentPoints += amount;
        UpdateScore();
    }

    private void UpdateScore()
    {
        string formattedText = $"<sprite=12> {currentPoints} / {goal}";
        scoreText.text = formattedText;

        if (currentPoints >= goal)
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }
}
