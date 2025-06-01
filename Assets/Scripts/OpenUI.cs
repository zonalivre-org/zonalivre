using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private LayerMask clicklableLayers;
    [SerializeField] private float delay;
    [SerializeField] private bool interactable;
    [SerializeField] private bool enable;

    private PlayerController playerMovement;

    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerController>();
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) Interact();
    }

    private void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                if (interactable)
                {
                    enable = true;
                }
            }
            else if (enable)
            {
                enable = false;
                CancelInvoke(nameof(EnableUI)); 
            }
        }
    }

    private void EnableUI()
    {
        ui.SetActive(true);
        playerMovement.ToggleMovement(false);
    }

    public void toggleUI(bool toggle)
    {
        ui.SetActive(toggle);
        playerMovement.ToggleMovement(!toggle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Invoke("EnableUI", delay);
        }
    }
}
