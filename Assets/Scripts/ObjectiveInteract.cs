using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectiveInteract : MonoBehaviour
{
    [SerializeField] private GameObject triggerObj;
    private ObjectivePlayerCheck trigger;
    [SerializeField] private LayerMask clicklableLayers;
    private void Awake()
    {
        if(triggerObj.GetComponent<ObjectivePlayerCheck>())
            trigger = triggerObj.GetComponent<ObjectivePlayerCheck>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) SelectObjective();
    }
    private void SelectObjective()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            trigger.ToggleState(true);
            Debug.Log("Ind fazer esta tarefa!");
            Debug.Log("Trigger area = " + trigger.enable);
        }
        else if(trigger.enable)
        {
            trigger.ToggleState(false);
            Debug.Log("desativou o local!");
        }
    }
}
