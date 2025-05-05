using UnityEngine;

public class Bowl : MonoBehaviour
{
    [SerializeField] private FillTheBowlMinigame fillTheBowl;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ration"))
        {
            fillTheBowl.FillBowl();
            Destroy(other.gameObject);
        }
    }
}
