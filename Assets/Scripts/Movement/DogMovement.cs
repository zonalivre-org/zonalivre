using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using FSMC.Runtime;
using Random = UnityEngine.Random;

public class DogMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> destinationTransform;
    [SerializeField] private FSMC_Executer stateMachine;
    [SerializeField] public string currentStateName;
    
    private NavMeshAgent agent;
    private float detectRadius;
    public bool canAutoMove = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<FSMC_Executer>();
    }

    private void Update()
    {
        
        currentStateName = stateMachine.GetCurrentState().Name;
        
    }

    // Core movement methods (used by states)
    public void MoveToDestination(Vector3 currentDestination)
    {
        if (!canAutoMove) return;
        agent.SetDestination(currentDestination);
    }

    public void StopMovement()
    {
        if (agent != null)
        {
            agent.ResetPath();
        }
    }

    public Vector3 GetRandomDestination()
    {
        if (destinationTransform == null || destinationTransform.Count == 0)
            return transform.position;
            
        return destinationTransform[Random.Range(0, destinationTransform.Count)].position;
    }

    public Vector3 GetPlayerPosition() => playerTransform.position;

    // State transition helpers
    public void TriggerFollowPlayer()
    {
        stateMachine.SetTrigger("FollowPlayer");
    }

    public void TriggerIdle()
    {
        stateMachine.SetTrigger("Idle");
    }
    
    public void TriggerFlee()
    {
        stateMachine.SetTrigger("Flee");
    }

    public void TriggerRandomMovement()
    {
        stateMachine.SetTrigger("Wandering");
        SetAutonomousMovement(true); 
    }

    // Clean version of SetAutonomousMovement
    public void SetAutonomousMovement(bool enabled)
    {
        canAutoMove = enabled;
        if (!enabled) StopMovement();
    }
}
