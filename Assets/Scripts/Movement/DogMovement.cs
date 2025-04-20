using FSMC.Runtime;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public interface IDogAnimator
{
    void SetMovementAnimation(Vector3 velocity, int direction);
}

public class DogAnimator : IDogAnimator
{
    private readonly Animator animator;

    public DogAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void SetMovementAnimation(Vector3 velocity, int direction)
    {
        string animationState = (velocity.sqrMagnitude > 0.1f) ? "Walking" : "Idle";
        animationState += (direction == 1) ? "_Right" : "_Left";
        animator.Play(animationState);
    }
}

public class DogMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> destinationPoints;
    [SerializeField] private float timeBetweenMoves = 1.5f;
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private int followQuota = 5;

    private NavMeshAgent agent;
    private float detectRadius;
    private int currentQuota;
    private bool canAutoMove = true;
    private int direction = 1; // 1 for right, -1 for left
    private IDogAnimator dogAnimator;
    private string funcName;
    [HideInInspector]
    public FSMC_Executer stateMachine;
    public FSMC_Transition transitions;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<FSMC_Executer>();
        agent.updateRotation = false;
        currentQuota = followQuota;
        detectRadius = (transform.localScale.x / 2) + detectionRange;
        dogAnimator = new DogAnimator(GetComponent<Animator>());
        SetAutonomousMovement(true);
    }

    private void LateUpdate()
    {
        dogAnimator.SetMovementAnimation(agent.velocity, direction);
    }

    public void SetAutonomousMovement(bool enabled)
    {
        canAutoMove = enabled;
        if (!enabled) 
        {
            agent.SetDestination(transform.position);
        }
        else 
        {
            MoveToRandomDestination();
        }
    }

    public void MoveToRandomDestination()
    {
        if (!canAutoMove) return;

        int roll = Random.Range(1, 128);
        if (roll >= 24) 
        {
            MoveToPoint(destinationPoints[Random.Range(0, destinationPoints.Count)].position);
        }
        else 
        {
            StopMovementTemporarily(2);
        }
    }

    public void MoveToPoint(Vector3 point)
    {
        SetDestination(point, 1f, nameof(MoveToRandomDestination));
    }

    public void StopMovementTemporarily(float waitMultiplier)
    {
        SetDestination(transform.position, waitMultiplier, nameof(MoveToRandomDestination));
    }

    public void MoveToPlayer()
    {
        if (currentQuota > 0)
        {
            currentQuota--;
            SetDestination(playerTransform.position, 0.5f, nameof(MoveToPlayer));
        }
        else
        {
            currentQuota = followQuota;
            SetDestination(playerTransform.position, 0.5f, nameof(MoveToRandomDestination));
        }
    }

    private void SetDestination(Vector3 target, float waitMultiplier, string nextAction)
    {
        direction = target.x > transform.position.x ? 1 : -1;
        agent.SetDestination(target);
        funcName = nextAction;
        Invoke(nameof(CheckArrival), 0.7f);
    }

    private void CheckArrival()
    {
        float distance = Vector3.Distance(agent.destination, transform.position);
        if (distance <= detectRadius)
        {
            Invoke(funcName, timeBetweenMoves);
        }
        else
        {
            Invoke(nameof(CheckArrival), 0.7f);
        }
    }
}
