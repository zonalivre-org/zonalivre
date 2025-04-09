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
    public bool canAutoMove = true;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentQuota = followQuota;
        detectRadius = (this.transform.localScale.x / 2) + detectionRange;
    }
    private void Start()
    {
        FollowNode();
    }

    private void Arrival()
    {
        distance = Vector3.Distance(destination, this.transform.position);
        if(distance <= detectRadius) Invoke(funcName, waitTime);
        else Invoke(nameof(Arrival), 0.7f);
    }
    private void MoveToDestination(Vector3 currentDestination, float waitTimeMultiplier, string nextFunctionCall)
    {
        if (!canAutoMove) return;
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
    public void FollowNode()
    {
        MoveToDestination(destinationTransform[Random.Range(0, destinationTransform.Count)].position, 1f, "RandomizeMovement");
    }
    public void WaitInPlace(float waitMultiplier)
    {
        MoveToDestination(this.transform.position, waitMultiplier, "RandomizeMovement");
    }
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
    public void ToggleMovement(bool toggle)
    {
        RandomizeMovement();
    }

    public void StopMovement()
    {
        if (agent != null)
        {
            agent.ResetPath(); // Limpa o caminho atual
        }
    }
    public void FollowPlayerAtSafeDistance(float safeDistance)
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = transform.position - playerTransform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Se estiver muito longe, aproxima-se
        if (distanceToPlayer > safeDistance * 1.2f)
        {
            MoveToDestination(playerTransform.position, 0.5f, nameof(FollowPlayerAtSafeDistance));
        }
        // Se estiver muito perto, afasta-se um pouco
        else if (distanceToPlayer < safeDistance)
        {
            Vector3 retreatPosition = transform.position + directionToPlayer.normalized * safeDistance;
            MoveToDestination(retreatPosition, 0.5f, nameof(FollowPlayerAtSafeDistance));
        }
        // Se estiver na distância ideal, espera
        else
        {
            WaitInPlace(1f);
        }
    }
    
    public void FleeFromPlayer(float fleeDistance)
    {
        if (playerTransform == null) return;

        Vector3 directionAwayFromPlayer = transform.position - playerTransform.position;
        Vector3 fleeTarget = transform.position + directionAwayFromPlayer.normalized * fleeDistance;

        // Verifica se o destino de fuga é válido no NavMesh
        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
        {
            MoveToDestination(hit.position, 0.3f, nameof(FleeFromPlayer));
        }
        else
        {
            // Se não encontrar um local válido, espera
            WaitInPlace(0.5f);
        }
    }


    
}
