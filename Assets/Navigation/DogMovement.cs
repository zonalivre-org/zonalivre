using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public List<Transform> destinationTransform;
    private Transform currentDestination;
    [SerializeField]
    private float timeBetweenMove = 2f;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }
    void Start()
    {
        MoveToDestination();
    }

  
    void MoveToDestination()
    {
        currentDestination = destinationTransform[Random.Range(0, destinationTransform.Count)];
        agent.SetDestination(currentDestination.position);
        Invoke("MoveToDestination", timeBetweenMove);
    }
    
}
