using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaterCan : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PlantTheCitronela plantTheCitronela;
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
        StartCoroutine(SpawnWaterWithCooldown()); // Start spawning rations
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private IEnumerator SpawnWaterWithCooldown()
    {
        while (isHolding) // Keep spawning while the player is holding
        {
            if (canSpawn)
            {
                SpawnWater();
                canSpawn = false; // Prevent immediate re-spawning
                yield return new WaitForSeconds(spawnCooldown); // Wait for the cooldown
                canSpawn = true; // Allow spawning again
            }
            yield return null; // Wait for the next frame
        }
    }

    private void SpawnWater()
    {
        GameObject ration = Instantiate(waterPrefab, spawnPoint.position, Quaternion.identity);

        //SoundManager.Instance.PlayRandomPitchSFXSound(3);

        Destroy(ration, 3f); 

        // float randomRotation = Random.Range(0, 360);

        // ration.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, randomRotation);

        Vector3 forceDirection = spawnPoint.up * startSpeed;

        ration.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);

        // Set the parent of the spawned ration to the moveArea
        ration.transform.SetParent(moveArea, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(moveArea, eventData.position, eventData.pressEventCamera))
        {
            transform.position = Input.mousePosition;
        }
    }
}
