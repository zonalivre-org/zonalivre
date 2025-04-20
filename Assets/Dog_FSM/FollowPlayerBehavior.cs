using System;
using System.Collections;
using System.Collections.Generic;
using FSMC.Runtime;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class FollowPlayerBehavior : FSMC_Behaviour
{
    private DogMovement dogMovement;
    
    public override void StateInit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        dogMovement = executer.GetComponent<DogMovement>();
    }
    
    public override void OnStateEnter(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        dogMovement.MoveToDestination(dogMovement.GetPlayerPosition());
    }
    
    public override void OnStateUpdate(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        dogMovement.MoveToDestination(dogMovement.GetPlayerPosition());
    }
    
    public override void OnStateExit(FSMC_Controller stateMachine, FSMC_Executer executer)
    {
        base.OnStateExit(stateMachine, executer);
    }
}
