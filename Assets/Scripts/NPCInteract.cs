using UnityEngine;
using UnityEngine.UI;

public class NPCInteract : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelIndex;
    [SerializeField] private string cutSceneClipName;
    [SerializeField] private string cutSceneName;
    [SerializeField] private GameObject spawnPoint;

    [Header("Other things")]
    [SerializeField] private LayerMask clicklableLayers;
    [SerializeField] private float delay;
    [SerializeField] private bool interactable;
    [SerializeField] private bool enable;

    private void Interact()
    {
        HUBManager.Instance.StartLevelSelection(levelIndex, cutSceneClipName, cutSceneName);

        SaveManager.Instance.SetSpawnPosition(spawnPoint.transform.position);
        Debug.Log("Spawn position set to: " + spawnPoint.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(Interact), delay);
        }
    }
}
