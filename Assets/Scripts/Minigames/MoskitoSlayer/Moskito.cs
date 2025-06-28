using UnityEngine;
using UnityEngine.EventSystems;

public class Moskito : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject hitEffectPrefab;
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public MoskitoSlayer moskitoSlayer;
    private RectTransform rectTransform;
    [HideInInspector] public float speed = 2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Move();
    }

    public void Initialize(Vector2 initialDirection, float initialSpeed)
    {
        if (initialDirection.x > 0)
        {
            rectTransform.localRotation = Quaternion.Euler(0, 0, 0); // Facing right
        }

        else if (initialDirection.x < 0)
        {
            rectTransform.localScale = new Vector3(-1, 1, 1); // Facing left
        }

        else if (initialDirection.y > 0)
        {
            rectTransform.localRotation = Quaternion.Euler(0, 0, 90); // Facing up
        }

        direction = initialDirection.normalized;

        speed = initialSpeed;
    }

    private void Move()
    {
        Vector2 position = transform.position;
        position += direction * speed * Time.deltaTime;
        transform.position = position;

        Destroy(gameObject, 10f); // Destroy after 10 seconds to prevent memory leaks
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity, transform);

        Destroy(hitEffect, 0.12f); // Destroy hit effect after 0.5 seconds
        speed = 0f; // Stop the Moskito movement
        Invoke(nameof(SlainMoskito), 0.12f);
        Destroy(gameObject, 0.12f); // Destroy the Moskito]
    }

    private void SlainMoskito()
    {
        moskitoSlayer.MoskitoSlain();
    }
}
