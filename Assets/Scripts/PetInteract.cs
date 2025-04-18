using UnityEngine;

public class PetInteract : MonoBehaviour
{
    [Header("Interaction Values")]
    [SerializeField] private LayerMask clicklableLayers;
    [SerializeField] private float detectionDelay = 0.5f;
    [SerializeField] private int healthGain = 50;
    [SerializeField] private int staminaGain = 60;
    [SerializeField] private int happynessGain = 20;
    [Header("Movement Elements")]
    [SerializeField] private DogMovement dogMovement;
    [SerializeField] private PlayerController playerMovement;
    [Header("UI Elements")]
    [SerializeField] private GameObject healthMinigameUI;
    [SerializeField] private GameObject staminaMinigameUI;
    [SerializeField] private GameObject happynessMinigameUI;
    private InGameProgress inGameProgress;
    private bool enable = false, interactable = true;
    private int defineObjective = 2;
    [Header("Place Holder Variables for Debugging Porpuses")]
    [SerializeField] private LayerMask healthLayer, staminaLayer, happynessLayer;
    private void Awake() => inGameProgress = FindObjectOfType<InGameProgress>();
    private void LateUpdate()
    {
        if(interactable && Input.GetMouseButtonDown(0)) SelectObjective();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(enable && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entrou na area do pet!");
            Invoke ("StartMinigame", detectionDelay);
        }
    }
    private void SelectObjective()
    {
        RaycastHit hit;

        //Placehoulder code bellow VVV
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, healthLayer)) 
        {
            InventoryManager.instance.Search(101, true); 
            Debug.Log(InventoryManager.instance.currentItem.name);
        }
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, staminaLayer))
        {
            InventoryManager.instance.Search(102, true); 
            Debug.Log(InventoryManager.instance.currentItem.name);
        }
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, happynessLayer)) 
        {
            InventoryManager.instance.Search(103, true); 
            Debug.Log(InventoryManager.instance.currentItem.name);
        }
        //Placehoulder code above ^^^

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            enable = true;
            dogMovement.WaitInPlace(9999f);
            Debug.Log("Indo olhar o pet!");
        }
        else if(enable)
        {
            enable = false;
            dogMovement.FollowNode();
            Debug.Log("cancelou a ação");
        }
    }
    public void CompleteTask(int whichTask) // receives from another script a value between 0 and 2 then adds the score to the assigned pet slider value. 
    {
        if(whichTask == 0)
        {
            inGameProgress.AddSliderValue(healthGain, whichTask);
            Debug.Log("Vida do pet tratada!");
        }
        else if(whichTask == 1)
        {
            inGameProgress.AddSliderValue(staminaGain, whichTask);
            Debug.Log("Fome do pet saciada");
        }
        else if(whichTask == 2)
        {
            inGameProgress.AddSliderValue(happynessGain, whichTask);
            Debug.Log("Pet parece estar mais feliz!");
        }
        else Debug.Log(whichTask + " is not a valid Pet task number!");

        InventoryManager.instance.Search(0, true);
        playerMovement.ToggleMovement(true);
        dogMovement.FollowNode();
        interactable = true;
    }
    
    public void CancelTask() // receives a value from another script to cancel the minigame and return to the game.
    {
        playerMovement.ToggleMovement(true);
        dogMovement.FollowNode();
        enable = false;
        interactable = true;
        Debug.Log("Cancelou a ação");
    }

    private void StartMinigame() // receives a value from within this script between 0 and 2 then activate the minigame corresponding to whatever pet slider value is to be changed.
    {
        if(enable)
        {
            playerMovement.ToggleMovement(false);
            InventoryManager imi = InventoryManager.instance;
            if(imi.currentItem.id == 101) //Minigame de curar a vida
            {
                healthMinigameUI.SetActive(true);
            }
            else if(imi.currentItem.id == 102) //Minigame de alimentar
            {
                staminaMinigameUI.SetActive(true);
            }
            else if(imi.currentItem.id == 103 || imi.currentItem.id == 0) //Minigame de brincar
            {
                happynessMinigameUI.SetActive(true);
            }
            else Debug.Log(imi.currentItem.id + " is not a valid ID number!");
            enable = false;
            interactable = false;
        }
    }
}
