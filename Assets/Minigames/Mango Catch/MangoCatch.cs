using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MangoCatch : MonoBehaviour
{
    [Header("Rules Section")]
    [SerializeField] int goal;
    [SerializeField] int current;
    [SerializeField] float mangoFallSpeed;
    [SerializeField] float cooldownBetweenMangos;

    [Header("Components")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] RectTransform spawnArea;
    [SerializeField] GameObject mangoPrefab;
    [SerializeField] GameObject player;
    [SerializeField] GameObject startPanel, endPanel;
    [SerializeField] ObjectivePlayerCheck objectivePlayerCheck;
    [SerializeField] GameObject miniGameParent;

    [Header("Variables")]
    private float actualTime, nextTime;
    [SerializeField] private bool canGenerate;
    [SerializeField] private Vector2 playerInitialPosition;
    //[SerializeField] private GameState gameState; 

    //enum GameState
    //{
    //    Start,
    //    Playing,
    //    End
    //}

    void Start()
    {
        UpdateScore();
    }

    private void OnEnable()
    {
        ResetMiniGame();
        UpdateScore();
    }

    void Update()
    {
        if (canGenerate)
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

    public void ResetMiniGame()
    {
        current = 0;
        startPanel.SetActive(true);
        player.GetComponent<RectTransform>().anchoredPosition = playerInitialPosition;
    }

    public void StartMangoGame()
    {
        canGenerate = true;
        startPanel.SetActive(false);
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

    }

    public void AddPoint(int amount)
    {
        current += amount;
        UpdateScore();
    }

    private void UpdateScore()
    {
        string formattedText = $"<sprite=12> {current} / {goal}";
        scoreText.text = formattedText;

        if (current >= goal)
        {
            EndMiniGame();
        }
    }

    public void EndMiniGame()
    {
        canGenerate = false;
        objectivePlayerCheck.CompleteTask();
        miniGameParent.SetActive(false);
    }
}
