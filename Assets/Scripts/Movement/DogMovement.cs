using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DogMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> destinationTransform;
    [SerializeField] private string firstOrder = "FollowNode";
    [SerializeField] private float timeBetweenMove = 1.5f;
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private int followQuota = 5;
    private NavMeshAgent agent;
    private float detectRadius;
    private float distance;
    private Vector3 destination;
    private float waitTime;
    private string funcName;
    private int currentQuota;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentQuota = followQuota;
        detectRadius = (this.transform.localScale.x / 2) + detectionRange;
    }
    private void Start() => Invoke(firstOrder, timeBetweenMove);
    private void Arrival()
    {
        distance = Vector3.Distance(destination, this.transform.position);
        if(distance <= detectRadius) Invoke(funcName, waitTime);
        else Invoke("Arrival", 0.7f);
    }
    private void MoveToDestination(Vector3 currentDestination, float waitTimeMultiplier, string nextFunctionCall)
    {
        destination = currentDestination;
        waitTime = timeBetweenMove * waitTimeMultiplier;
        funcName = nextFunctionCall;
        agent.SetDestination(currentDestination);
        Arrival();
    }
    private void RandomizeMovement()
    {
        int roll = Random.Range(1, 128);
        if(roll >= 24) FollowNode();
        else WaitInPlace(2);
    }
    private void FollowNode() => MoveToDestination(destinationTransform[Random.Range(0, destinationTransform.Count)].position, 1f, "RandomizeMovement");
    public void WaitInPlace(float waitMultiplier) => MoveToDestination(this.transform.position, waitMultiplier, "RandomizeMovement");
    public void FollowPlayer()
    {
        if(currentQuota > 0)
        {
            currentQuota--;
            MoveToDestination(playerTransform.position, 0.5f, "FollowPlayer");
        }
        else
        {
            currentQuota = followQuota;
            MoveToDestination(playerTransform.position, 0.5f, "RandomizeMovement");
        }
    }
    //public void Panic(){}
    //public void Flee(){}
}
