using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MangoCatch : MonoBehaviour
{
    [Header("Rules Section")]
    [SerializeField] int goal;
    int current;
    [SerializeField] float mangoFallSpeed;
    [SerializeField] float coolDownBetweenMangos;

    [Header("Variables")]
    [SerializeField] float spawnMinX;
    [SerializeField] float spawnMaxX;
    [SerializeField] float spawnY;
    private float actualTime, nextTime;
    [HideInInspector] public bool canGenerate;

    [Header("Components")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject mangoPrefab;
    [SerializeField] GameObject startPanel, endPanel;
    [SerializeField] Transform zPosition;

    void Start()
    {
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
                nextTime = Time.time + coolDownBetweenMangos;
            }
        }
    }

    public void StartMangoGame()
    {
        canGenerate = true;
        startPanel.SetActive(false);
    }

private void SpawnMango()
{
    // Set random coordinates within the spawn area
    float randomX = Random.Range(spawnMinX, spawnMaxX);

    // Instantiate a mango inside the spawn area
    Vector3 randomPosition = new Vector3(randomX, spawnY, zPosition.position.z);
    GameObject mango = Instantiate(mangoPrefab, randomPosition, mangoPrefab.transform.rotation);

    // Set the mango as a child of the Main Object
    mango.transform.SetParent(this.transform, false);

    // Reference the manager (this class here :D) to the mango generated
    mango.GetComponent<Mango>().mangoCatch = this;

    // Set the mango fall speed
    mango.GetComponent<Mango>().fallSpeed = mangoFallSpeed;
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

        if (current == goal)
        {
            canGenerate = false;
            endPanel.SetActive(true);
        }
    }
}
