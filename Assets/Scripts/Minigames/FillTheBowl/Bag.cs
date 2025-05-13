using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Bag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private GameObject rationPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FillTheBowlMinigame fillTheBowl;
    RectTransform rectTransform;
    [SerializeField] private float rotationOnClick;
    [SerializeField] private RectTransform moveArea;
    [SerializeField] private float spawnCooldown = 0.5f; // Cooldown time in seconds
    [SerializeField] private float startSpeed = 5f;
    private bool isHolding;
    private bool canSpawn = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void OnEnable()
    {
        rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        canSpawn = true;
    }

    void LateUpdate()
    {
        if (isHolding)
        {
            MiniGameBase.OnMinigameInteract.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        rectTransform.localRotation = Quaternion.Euler(0, 0, rotationOnClick);
        StartCoroutine(SpawnRationWithCooldown()); // Start spawning rations
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private IEnumerator SpawnRationWithCooldown()
    {
        while (isHolding) // Keep spawning while the player is holding
        {
            if (canSpawn)
            {
                SpawnRation();
                canSpawn = false; // Prevent immediate re-spawning
                yield return new WaitForSeconds(spawnCooldown); // Wait for the cooldown
                canSpawn = true; // Allow spawning again
            }
            yield return null; // Wait for the next frame
        }
    }

    private void SpawnRation()
    {
        GameObject ration = Instantiate(rationPrefab, spawnPoint.position, Quaternion.identity);

        //SoundManager.Instance.PlayRandomPitchSFXSound(3);

        Destroy(ration, 3f); 

        float randomRotation = Random.Range(0, 360);

        ration.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, randomRotation);

        Vector3 forceDirection = spawnPoint.up * startSpeed;

        ration.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);

        // Set the parent of the spawned ration to the moveArea
        ration.transform.SetParent(moveArea, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calculate the new position based on the drag delta
        Vector2 newPosition = rectTransform.anchoredPosition + new Vector2(eventData.delta.x, eventData.delta.y);

        // Get the bottom-left and top-right corners of the moveArea in local space
        Vector2 minBounds = moveArea.rect.min + (Vector2)moveArea.anchoredPosition;
        Vector2 maxBounds = moveArea.rect.max + (Vector2)moveArea.anchoredPosition;

        // Get the size of the object (/2 because the pivot is in the center)
        Vector2 objectSize = rectTransform.rect.size / 2;

        // Clamp the new position's X-axis only if the cursor is inside the moveArea
        if (RectTransformUtility.RectangleContainsScreenPoint(moveArea, eventData.position, eventData.pressEventCamera))
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x + objectSize.x, maxBounds.x - objectSize.x);
        }
        else
        {
            newPosition.x = rectTransform.anchoredPosition.x; // Keep the X position unchanged
        }

        // Clamp the new position's Y-axis within the moveArea
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y + objectSize.y, maxBounds.y - objectSize.y);

        // Update the object's position
        rectTransform.anchoredPosition = newPosition;
    }

}
