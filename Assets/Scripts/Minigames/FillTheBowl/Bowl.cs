using UnityEngine;

public class Bowl : MonoBehaviour
{
    [SerializeField] private FillTheBowl fillTheBowl;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ration"))
        {
            fillTheBowl.FillBowl();
            Destroy(other.gameObject);
        }
    }
}
