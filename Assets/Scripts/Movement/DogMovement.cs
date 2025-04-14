using System.Collections.Generic;
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
    private bool canAutoMove = true;
    private Animator animator;
    private int direction = 1; // 1 for right, -1 for left

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentQuota = followQuota;
        detectRadius = (this.transform.localScale.x / 2) + detectionRange;

        agent.updateRotation = false; // Disable automatic rotation
    }

    void LateUpdate()
    {
        SetAnimations();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        Invoke(firstOrder, timeBetweenMove);
    }

    private void Arrival()
    {
        distance = Vector3.Distance(destination, this.transform.position);
        if (distance <= detectRadius) Invoke(funcName, waitTime);
        else Invoke("Arrival", 0.7f);
    }
    private void MoveToDestination(Vector3 currentDestination, float waitTimeMultiplier, string nextFunctionCall)
    {
        canAutoMove = true;
        destination = currentDestination;
        waitTime = timeBetweenMove * waitTimeMultiplier;
        funcName = nextFunctionCall;
        agent.SetDestination(currentDestination);

        if (currentDestination.x > this.transform.position.x) direction = 1;
        else if (currentDestination.x < this.transform.position.x) direction = -1;

        Arrival();
    }
    private void RandomizeMovement()
    {
        if (canAutoMove)
        {
            int roll = Random.Range(1, 128);
            if (roll >= 24) FollowNode();
            else WaitInPlace(2);
        }
    }
    public void FollowNode()
    {
        canAutoMove = false;
        MoveToDestination(destinationTransform[Random.Range(0, destinationTransform.Count)].position, 1f, "RandomizeMovement");
    }
    public void WaitInPlace(float waitMultiplier)
    {
        canAutoMove = false;
        MoveToDestination(this.transform.position, waitMultiplier, "RandomizeMovement");
    }
    public void FollowPlayer()
    {
        canAutoMove = false;
        if (currentQuota > 0)
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
        canAutoMove = toggle;
        if (!canAutoMove) agent.SetDestination(this.transform.position);
        else RandomizeMovement();
    }

    void SetAnimations()
    {
        Vector3 movementDirection = agent.velocity.normalized;

        if (agent.velocity.sqrMagnitude > 0.1f) // Check if the dog is moving
        {

            if (direction == 1)
            {
                animator.Play("Walking_Right");
            }
            else if (direction == -1)
            {
                animator.Play("Walking_Left");
            }
        }
        else
        {
            if (direction == 1)
            {
                animator.Play("Idle_Right");
            }
            else if (direction == -1)
            {
                animator.Play("Idle_Left");
            }
        }
    }
}
