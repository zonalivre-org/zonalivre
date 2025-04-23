using System;
using System.Collections;
using System.Collections.Generic;
using FSMC.Runtime;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class WanderingBehavior : FSMC_Behaviour
{
    private Transform playerTransform; // Reference to the player's transform
    private DogMovement dogMovement;
    private NavMeshAgent agent;
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        agent = executer.GetComponent<NavMeshAgent>();
        dogMovement = executer.GetComponent<DogMovement>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
    }
    
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        Debug.Log("Wandering state entered");
    }
    
    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        base.OnStateUpdate(stateMachine, executer);
        
        // Implement the logic for following the player here
        // For example, you can use NavMeshAgent to move towards the player's position
        // and check for distance to trigger other states if needed.
    }
    
    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        base.OnStateExit(stateMachine, executer);
    }

}
