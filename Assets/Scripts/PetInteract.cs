using System;
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
    private bool enableMinigameStart = false, interactable = true;
    private int defineObjective = 2;
    [Header("Place Holder Variables for Debugging Porpuses")]
    [SerializeField] private LayerMask healthLayer, staminaLayer, happynessLayer;
    private void Awake() => inGameProgress = FindObjectOfType<InGameProgress>();
    private void LateUpdate()
    {
        if (interactable && Input.GetMouseButtonDown(0)) SelectObjective();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (enableMinigameStart && other.gameObject.CompareTag("Player"))
        {
            dogMovement.canAutoMove = false;
            dogMovement.SetAutonomousMovement(false);
            Debug.Log("Entrou na area do pet!");
            Invoke("StartMinigame", detectionDelay);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (enableMinigameStart && other.gameObject.CompareTag("Player"))
        {
            dogMovement.canAutoMove = false;
            dogMovement.SetAutonomousMovement(false);
            Invoke(nameof(StartMinigame), detectionDelay);


        }


    }

    private void OnTriggerExit(Collider other)
    {
        dogMovement.canAutoMove = true;
        if (other.gameObject.CompareTag("Player"))
        {
            dogMovement.SetAutonomousMovement(true);
        }
    }

    private void SelectObjective()
    {
        RaycastHit hit;

        //Placehoulder code bellow VVV
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, healthLayer))
        {

        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, staminaLayer))
        {

        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, happynessLayer))
        {

        }
        //Placehoulder code above ^^^

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            enableMinigameStart = true;
            dogMovement.SetAutonomousMovement(false);
            Debug.Log("Indo olhar o pet!");
        }
        else if (enableMinigameStart)
        {
            enableMinigameStart = false;
            dogMovement.SetAutonomousMovement(true);
            Debug.Log("cancelou a ação");
        }
    }
    public void CompleteTask(int whichTask) // receives from another script a value between 0 and 2 then adds the score to the assigned pet slider value. 
    {
        if (whichTask == 0)
        {
            inGameProgress.AddSliderValue(healthGain, whichTask);
            Debug.Log("Vida do pet tratada!");
        }
        else if (whichTask == 1)
        {
            inGameProgress.AddSliderValue(staminaGain, whichTask);
            Debug.Log("Fome do pet saciada");
        }
        else if (whichTask == 2)
        {
            inGameProgress.AddSliderValue(happynessGain, whichTask);
            Debug.Log("Pet parece estar mais feliz!");
        }
        else Debug.Log(whichTask + " is not a valid Pet task number!");

        playerMovement.ToggleMovement(true);
        dogMovement.SetAutonomousMovement(true);
        dogMovement.canAutoMove = true;
        interactable = true;
    }

    public void CancelTask() // receives a value from another script to cancel the minigame and return to the game.
    {
        playerMovement.ToggleMovement(true);
        dogMovement.SetAutonomousMovement(true);
        enableMinigameStart = false;
        interactable = true;
        Debug.Log("Cancelou a ação");
    }

    private void StartMinigame() // receives a value from within this script between 0 and 2 then activate the minigame corresponding to whatever pet slider value is to be changed.
    {
        if (enableMinigameStart)
        {

            StartHealthMinigame();
        }
    }

    public void StartHealthMinigame()
    {
        playerMovement.ToggleMovement(false);

        healthMinigameUI.GetComponent<HoldButton>().petInteract = this;
        healthMinigameUI.SetActive(true);

        enableMinigameStart = false;
        interactable = false;
    }
    public void StartStaminaMinigame()
    {
        playerMovement.ToggleMovement(false);

        staminaMinigameUI.GetComponent<FillTheBowl>().petInteract = this;
        staminaMinigameUI.SetActive(true);

        enableMinigameStart = false;
        interactable = false;
    }
    public void StartHappynessMinigame()
    {
        playerMovement.ToggleMovement(false);

        happynessMinigameUI.GetComponent<HoldButton>().petInteract = this;
        happynessMinigameUI.SetActive(true);

        enableMinigameStart = false;
        interactable = false;
    }
}
