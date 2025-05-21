using UnityEngine;

public class Dirt : MonoBehaviour
{
    [SerializeField] private PlantTheCitronela plantTheCitronela;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") && plantTheCitronela.isPlanted)
        {
            plantTheCitronela.WaterPlant();
            Destroy(other.gameObject);
        }
    }
}
