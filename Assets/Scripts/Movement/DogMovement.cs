using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class DogMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> destinationNodes;

    [SerializeField] private float waitAtNodeDuration = 1.5f;

    [SerializeField] private float followSafeDistance = 3.0f;
    [SerializeField] private float followStopThreshold = 0.5f;

    [SerializeField] private float fleeDistance = 10.0f;

    public bool canAutoMove = true;

    private NavMeshAgent agent;
    private Animator animator;

    private enum DogState
    {
        Idle,
        MovingToDestination,
        WaitingAtDestination,
        FollowingPlayer,
        Fleeing
    }
    [SerializeField]
    private DogState currentState = DogState.Idle;
    private Coroutine waitCoroutine = null;
    private Vector3 currentDestination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null) Debug.LogError("NavMeshAgent não encontrado!", this);

        if (agent != null) agent.updateRotation = false;
    }

    private void Start()
    {
        if (canAutoMove)
        {
            ChangeState(DogState.MovingToDestination);
        }
        else
        {
            ChangeState(DogState.Idle);
        }
    }

    private void Update()
    {
        UpdateCurrentStateLogic();
    }

    private void ChangeState(DogState newState)
    {
        if (currentState == newState) return;

        ExitCurrentStateLogic();

        currentState = newState;

        EnterCurrentStateLogic();
    }

    private void EnterCurrentStateLogic()
    {
        switch (currentState)
        {
            case DogState.Idle:
                StopAgentMovement();
                break;

            case DogState.MovingToDestination:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; }
                currentDestination = GetRandomNavMeshNodePosition();
                SetAgentDestination(currentDestination);
                break;

            case DogState.WaitingAtDestination:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; }
                StopAgentMovement();
                StartWaitTimer(waitAtNodeDuration);
                break;

            case DogState.FollowingPlayer:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; }
                break;

            case DogState.Fleeing:
                if (!canAutoMove) { ChangeState(DogState.Idle); return; }
                StartFleeing();
                break;
        }
    }

    private void UpdateCurrentStateLogic()
    {
        if (!canAutoMove && currentState != DogState.Idle)
        {
            ChangeState(DogState.Idle);
            return;
        }

        switch (currentState)
        {
            case DogState.Idle:
                if (canAutoMove)
                {
                }
                break;

            case DogState.MovingToDestination:
                if (HasAgentArrived())
                {
                    ChangeState(DogState.WaitingAtDestination);
                }
                break;

            case DogState.WaitingAtDestination:
                break;

            case DogState.FollowingPlayer:
                UpdateFollowPlayerPosition();
                break;

            case DogState.Fleeing:
                if (HasAgentArrived())
                {
                    ChangeState(DogState.MovingToDestination);
                }
                break;
        }
    }

    private void ExitCurrentStateLogic()
    {
        switch (currentState)
        {
            case DogState.WaitingAtDestination:
                StopWaitTimer();
                break;
        }
    }

    private void StartWaitTimer(float duration)
    {
        StopWaitTimer();
        waitCoroutine = StartCoroutine(WaitCoroutine(duration));
    }

    private void StopWaitTimer()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }

    private IEnumerator WaitCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        waitCoroutine = null;

        if (currentState == DogState.WaitingAtDestination && canAutoMove)
        {
            ChangeState(DogState.MovingToDestination);
        }
    }

    private void UpdateFollowPlayerPosition()
    {
        if (playerTransform == null || !canAutoMove)
        {
            ChangeState(DogState.Idle);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 directionToPlayer = transform.position - playerTransform.position;

        bool needsToMove = false;
        Vector3 targetPosition = transform.position;

        if (distanceToPlayer > followSafeDistance)
        {
            targetPosition = playerTransform.position;
            needsToMove = true;
        }
        else if (distanceToPlayer < (followSafeDistance - followStopThreshold))
        {
            targetPosition = playerTransform.position + directionToPlayer.normalized * followSafeDistance;
            needsToMove = true;
        }

        if (needsToMove)
        {
            if (!agent.hasPath || Vector3.Distance(agent.destination, targetPosition) > 0.1f)
            {
                SetAgentDestination(targetPosition);
            }
        }
        else if(agent.hasPath)
        {
            StopAgentMovement();
        }
    }

    private void StartFleeing()
    {
        if (playerTransform == null || !canAutoMove)
        {
            ChangeState(DogState.Idle);
            return;
        }

        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        Vector3 fleeTargetAttempt = transform.position + directionAwayFromPlayer * (fleeDistance * 1.2f);

        if (NavMesh.SamplePosition(fleeTargetAttempt, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
        {
            SetAgentDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Não foi possível encontrar ponto de fuga válido no NavMesh.", this);
            ChangeState(DogState.Idle);
        }
    }

    private void SetAgentDestination(Vector3 destination)
    {
        if (agent != null && agent.enabled)
        {
            currentDestination = destination;
            agent.SetDestination(destination);
        }
    }

    private void StopAgentMovement()
    {
        if (agent != null && agent.enabled && agent.hasPath)
        {
            agent.ResetPath();
        }
        StopWaitTimer();
    }

    private bool HasAgentArrived()
    {
        if (agent == null || !agent.enabled || agent.pathPending || !agent.hasPath)
        {
            return false;
        }
        return agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.01f;
    }

    private Vector3 GetRandomNavMeshNodePosition()
    {
        if (destinationNodes == null || destinationNodes.Count == 0)
        {
            return transform.position;
        }

        Vector3 randomPos = destinationNodes[Random.Range(0, destinationNodes.Count)].position;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return transform.position;
        }
    }

    public void RequestFollowPlayer()
    {
        if (!canAutoMove) SetAutonomousMovement(true);
        ChangeState(DogState.FollowingPlayer);
    }

    public void RequestFlee()
    {
        if (!canAutoMove) SetAutonomousMovement(true);
        ChangeState(DogState.Fleeing);
    }

    public void RequestStopAndIdle()
    {
        SetAutonomousMovement(false);
    }

    public void RequestResumeWander()
    {
        SetAutonomousMovement(true);
        if (currentState == DogState.Idle)
        {
            ChangeState(DogState.MovingToDestination);
        }
    }

    public void SetAutonomousMovement(bool enabled)
    {
        if (canAutoMove == enabled) return;

        canAutoMove = enabled;

        if (!enabled)
        {
            ChangeState(DogState.Idle);
        }
        else if (currentState == DogState.Idle)
        {
            ChangeState(DogState.MovingToDestination);
        }
    }
}