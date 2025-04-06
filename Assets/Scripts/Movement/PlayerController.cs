using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    private PlayerActions input;
    private NavMeshAgent agent;
    private Animator animator;
    public bool canMove = true;

    [Header("Movement")]
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private LayerMask clicklableLayers;
    [SerializeField] private float lookRotationSpeed = 8f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        input = new PlayerActions();
        AssignInputs();

    }

    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        if (canMove)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clicklableLayers))
            {
                agent.SetDestination(hit.point);
                // animator.SetTrigger(WALK); // This will be needed when we have a walk animation
                ParticleSystem clickEffectInstance = (Instantiate(clickEffect, hit.point + Vector3.up * 0.1f, clickEffect.transform.rotation));
                Destroy(clickEffectInstance.gameObject, 1f);
            }
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

    void Update()
    {

    }

    void FaceTarget()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lookRotationSpeed * Time.deltaTime);
    }

    void SetAnimations()
    {
        // if (agent.velocity != Vector3.zero)
        // {
        //     animator.SetTrigger(WALK);
        // }
        // else
        // {
        //     animator.SetTrigger(IDLE);
        // }

    }
    public void ToggleMovement(bool toggle) => canMove = toggle;
}
