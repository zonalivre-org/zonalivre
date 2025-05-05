using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CleanMinigame : MiniGameBase
{
    [Header("Rules")]
    [Range(0, 1)] [SerializeField] private float cleanSpeed;
    [SerializeField] private int trashAmount = 5;
    private int trashBagRemaining;
    private int trashRemaining;

    [Header("Components")]
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private Sprite trashBagSprite;
    [SerializeField] private Sprite trashSprite;
    [SerializeField] private RectTransform trashCan;
    [HideInInspector] public ObjectiveInteract objectiveInteract;
    [SerializeField] private Texture2D broomCursorTexture;
    [SerializeField] private Texture2D handCursorTexture;
    private string originalTipText;

    void Awake()
    {
        originalTipText = tipText.text;

    }

    void Update()
    {
        TipCheck();
    }

    public void SetMiniGameRules(float cleanSpeed, int trashAmount)
    {
        this.cleanSpeed = cleanSpeed;
        this.trashAmount = trashAmount;
    }
    
    public override void StartMiniGame()
    {
        base.StartMiniGame();

        OnStart();

        trashCan.gameObject.SetActive(false);

        trashRemaining = trashAmount;

        tipText.text = originalTipText;

        for (int i = 0; i < trashAmount; i++)
        {
            SpawnTrash();
        }
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) objectiveInteract.CompleteTask();
        else objectiveInteract.CloseTask();

        ResetMiniGame();
        base.EndMiniGame();
    }

    public void ResetMiniGame()
    {
        foreach (Transform child in spawnArea)
        {
            Destroy(child.gameObject);
        }

        trashBagRemaining = 0;
        isMiniGameComplete = false;
        trashCan.gameObject.SetActive(false);
        tipText.text = originalTipText;
    }
    public void SpawnTrash()
    {
        GameObject trash = new GameObject("Trash");
        trash.transform.SetParent(spawnArea);

        trash.AddComponent<Image>().sprite = trashSprite;
        trash.GetComponent<Image>().type = Image.Type.Filled;
        trash.GetComponent<Image>().fillMethod = Image.FillMethod.Radial360;

        trash.AddComponent<Outline>().effectColor = new Color(255, 255, 0, 1f);
        trash.GetComponent<Outline>().effectDistance = new Vector2(7, -7);
        trash.GetComponent<Outline>().enabled = false;

        trash.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        trash.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2), Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2));
        
        trash.AddComponent<TrashObject>();
        trash.GetComponent<TrashObject>().SetTrashProperties(cleanSpeed, this, trashBagSprite, trashCan, broomCursorTexture, handCursorTexture);
    }

    public void ReduceTrashAmount()
    {
        trashRemaining--;
        trashBagRemaining++;
        AudioManager.Instance.PlayRandomPitchSFXSound(2);

        if (trashRemaining <= 0)
        {
            trashCan.gameObject.SetActive(true);
            tipText.text = "Agora leve o lixo até a lixeira!";
        }
    }

    public void ReduceTrashBagAmount()
    {
        trashBagRemaining--;
        AudioManager.Instance.PlayRandomPitchSFXSound(2);

        if (trashBagRemaining <= 0)
        {
            isMiniGameComplete = true;
            EndMiniGame();
        }
    }
}
