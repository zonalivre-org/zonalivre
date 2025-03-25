using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{

    const string IDLE = "Idle";
    const string WALK = "Walk";

    private PlayerActions input;
    
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Movement")] 
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private LayerMask clicklableLayers;
    [SerializeField] private float lookRotationSpeed = 8f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        input = new PlayerActions();
        AssignInputs();

    }

    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
        {
            agent.SetDestination(hit.point);
            animator.SetTrigger(WALK);
            Instantiate(clickEffect, hit.point + Vector3.up*0.1f, clickEffect.transform.rotation);
            
        }
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
