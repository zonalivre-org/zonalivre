using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System;
public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    private PlayerActions input;
    private NavMeshAgent agent;
    private Animator animator;
    public bool canMove = true;
    private float agentOriginalSpeed;

    [Header("Movement")]
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private LayerMask clicklableLayers;
    [SerializeField] private float lookRotationSpeed = 8f;

    [Header("Actions")]
    public Action OnDestinationReached; // Action triggered when the player reaches the destination

    private bool hasReachedDestination = false; // Flag to track if the destination has been reached

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        input = new PlayerActions();
        AssignInputs();
        agentOriginalSpeed = agent.speed;

        agent.updateRotation = false; // Disable automatic rotation
    }

    void LateUpdate()
    {
        SetAnimations();
        CheckIfDestinationReached();
    }

    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        if (canMove && Time.timeScale != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
            {
                agent.SetDestination(hit.point);

                // Play click effect
                ParticleSystem clickEffectInstance = Instantiate(clickEffect, hit.point + Vector3.up * 0.1f, clickEffect.transform.rotation);
                Destroy(clickEffectInstance.gameObject, 1f);
            }
        }
    }

    void CheckIfDestinationReached()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                if (!hasReachedDestination) // Only invoke the action if the destination hasn't been reached yet
                {
                    hasReachedDestination = true;
                    OnDestinationReached?.Invoke(); // Trigger the action when the destination is reached
                }
            }
        }
        else
        {
            hasReachedDestination = false; // Reset the flag if the player starts moving again
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

    void FaceTarget()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lookRotationSpeed * Time.deltaTime);
    }

    void SetAnimations()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 movementDirection = agent.velocity.normalized;

            // Define directional vectors
            Vector3 forward = transform.forward;
            Vector3 backward = -transform.forward;
            Vector3 right = transform.right;
            Vector3 left = -transform.right;

            // Calculate dot products
            float dotForward = Vector3.Dot(movementDirection, forward);
            float dotBackward = Vector3.Dot(movementDirection, backward);
            float dotRight = Vector3.Dot(movementDirection, right);
            float dotLeft = Vector3.Dot(movementDirection, left);

            // Find the direction with the highest dot product
            if (dotForward > dotBackward && dotForward > dotRight && dotForward > dotLeft)
            {
                animator.Play("Walking_Forward");
            }
            else if (dotBackward > dotForward && dotBackward > dotRight && dotBackward > dotLeft)
            {
                animator.Play("Walking_Back");
            }
            else if (dotRight > dotForward && dotRight > dotBackward && dotRight > dotLeft)
            {
                animator.Play("Walking_Right");
            }
            else if (dotLeft > dotForward && dotLeft > dotBackward && dotLeft > dotRight)
            {
                animator.Play("Walking_Left");
            }
        }
        else
        {
            animator.Play("Idle");
        }
    }

    public void ToggleMovement(bool toggle)
    {
        canMove = toggle;
        if (!canMove)
        {
            agent.SetDestination(this.transform.position);
            agent.speed = 0;
        }
        else agent.speed = agentOriginalSpeed;
    }
}
