using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CleanMinigame : MiniGameBase
{
    [Header("Rules")]
    [Range(0, 1)] [SerializeField] private float cleanSpeed;
    [SerializeField] private int trashAmout = 5;
    private int trashBagRemaining;

    [Header("Components")]
    private string originalTipText;
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private Sprite trashBagSprite;
    [SerializeField] private Sprite trashSprite;
    [SerializeField] private RectTransform trashCan;
    [HideInInspector] public PetInteract petInteract;
    [SerializeField] private Texture2D broomCursorTexture;
    [SerializeField] private Texture2D handCursorTexture;

    void Awake()
    {
        originalTipText = tipText.text;
    }
    void OnEnable()
    {
        StartMiniGame();
    }

    void Update()
    {
        TipCheck();
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();

        OnStart();

        trashCan.gameObject.SetActive(false);

        tipText.text = originalTipText;

        for (int i = 0; i < trashAmout; i++)
        {
            SpawnTrash();
        }
    }

    public override void EndMiniGame()
    {
        if (isMiniGameComplete) petInteract.CompleteTask(1);
        else petInteract.CancelTask();
;
        isMiniGameActive = false;
        isMiniGameComplete = false;
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
        trashAmout--;
        trashBagRemaining++;
        AudioManager.Instance.PlayRandomPitchSFXSound(2);

        if (trashAmout <= 0)
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
