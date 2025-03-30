using UnityEngine;

public class PetInteract : MonoBehaviour
{
    [SerializeField] private LayerMask clicklableLayers;
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private float detectionDelay = 0.5f;
    [SerializeField] private InGameProgress inGameProgress;
    [SerializeField] private int petValue = 20;
    [SerializeField] private GameObject pettingUI;
    private InGameProgress notify;
    private bool interactable = true;
    private bool enable = false;
    private float distance;
    
    private void Awake() => notify = inGameProgress;
    private void Update()
    {
        if(interactable && Input.GetMouseButtonDown(0)) SelectObjective();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(enable && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entrou na area do pet!");
            Invoke ("Pet", detectionDelay);
        }
    }
    private void SelectObjective()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            enable = true;
            Debug.Log("Indo olhar o pet!");
        }
        else if(enable)
        {
            enable = false;
            Debug.Log("cancelou a ação");
        }
    }
    public void CompleteTask()
    {
        Debug.Log("Pronto!");
        interactable = true;
        notify.AddSliderValue(petValue, 2);
    }
    private void Pet()
    {
        pettingUI.SetActive(true);
        enable = false;
        interactable = false;
    }
}
