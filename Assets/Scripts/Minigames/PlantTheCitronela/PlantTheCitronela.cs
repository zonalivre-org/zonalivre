using UnityEngine;

public class PlantTheCitronela : MiniGameBase
{
    [Header("Rules")]
    [Range(1, 5)][SerializeField] private float growthSpeed = 3f;

    [Header("References")]
    [SerializeField] private RectTransform waterCan;
    [SerializeField] private RectTransform sappling;
    [SerializeField] private RectTransform seed;
    [HideInInspector] public ObjectiveInteract objectiveInteract;

    [Header("Variables")]
    [SerializeField] private Vector2 seedStartPos;
    [SerializeField] private Vector2 waterCanStartPos;
    public bool isPlanted = false;

    void Awake()
    {
        seedStartPos = seed.anchoredPosition;
        waterCanStartPos = waterCan.anchoredPosition;
    }

    void Update()
    {
        TipCheck();
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();
        tipText.text = "Arraste a semente at� o vaso para plantar";
        seed.gameObject.SetActive(true);
        waterCan.gameObject.SetActive(false);
        seed.anchoredPosition = seedStartPos;
        waterCan.anchoredPosition = waterCanStartPos;
        sappling.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        sappling.gameObject.SetActive(false);
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) objectiveInteract.CompleteTask();
        else objectiveInteract.CloseTask();

        isPlanted = false;
        
        base.EndMiniGame();
    }

    public void SetMinigameRules(float growthSpeed)
    {
        this.growthSpeed = growthSpeed;
    }
    public void WaterPlant()
    {
        if (isMiniGameComplete) return;

        sappling.gameObject.SetActive(true);
        sappling.localScale = Vector3.Lerp(sappling.localScale, new Vector3(2.25f, 2.25f, 2.25f), Time.deltaTime * growthSpeed);

        if (sappling.localScale.y >= 2.23f)
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }

    public void TransformSeedIntoSappling()
    {
        tipText.text = "Arraste o regador para regar a planta";
        waterCan.gameObject.SetActive(true);
        isPlanted = true;
        seed.gameObject.SetActive(false);
        sappling.gameObject.SetActive(true);
    }
}
