using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MangoCatch : MonoBehaviour
{
    [Header("Rules Section")]
    [SerializeField] int goal;
    [SerializeField] int current;
    [SerializeField] float mangoFallSpeed;
    [SerializeField] float coolDownBetweenMangos;

    [Header("Components")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject mangoPrefab;
    [SerializeField] GameObject startPanel, endPanel;

    [Header("Variables")]
    [SerializeField] float spawnMinX;
    [SerializeField] float spawnMaxX;
    [SerializeField] float spawnY;
    private float actualTime, nextTime;
    [HideInInspector] public bool canGenerate;

    void Start()
    {
        updateScore();
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
                spawnMango();
                nextTime = Time.time + coolDownBetweenMangos;
            }
        }

    }

    public void StartMangoGame()
    {
        canGenerate = true;
        startPanel.SetActive(false);
    }

    private void spawnMango()
    {
        //Set random coordinates within the spawn area
        float randomX = Random.Range(spawnMinX, spawnMaxX);

        //Instantiate a mango inside the spawn area
        Vector3 randomPosition = new Vector3(randomX, spawnY, mangoPrefab.transform.position.z);
        GameObject mango = Instantiate(mangoPrefab, randomPosition, mangoPrefab.transform.rotation);

        // Reference the manager (this class here :D) to the mango generated
        mango.GetComponent<Mango>().mangoCatch = this;

        // Set the mango fall speed
        mango.GetComponent<Mango>().fallSpeed = mangoFallSpeed;

    }

    public void addPoint(int amount)
    {
        current += amount;
        updateScore();
    }

    private void updateScore()
    {
        string formattedText = $"<sprite=12> {current} / {goal}";
        scoreText.text = formattedText;

        if (current == goal)
        {
            canGenerate = false;
            endPanel.SetActive(true);
        }
    }
}
