using Unity.VisualScripting;
using UnityEngine;

public class JumpObjects : MonoBehaviour
{
    [HideInInspector] public CleanNinja cleanNinja;
    [HideInInspector] public float zPos;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        RandomPosition();
        rb.AddForce(UpwardForce(), ForceMode.Impulse);
    }

    Vector3 UpwardForce()
    {
        float randomSpeed = Random.Range(cleanNinja.minSpeed, cleanNinja.maxSpeed);
        return Vector3.up * randomSpeed;
    }

    private void RandomPosition()
    {
        float randomX = Random.Range(cleanNinja.minXRange, cleanNinja.maxXRange);
        transform.position = new Vector3(randomX, cleanNinja.yPosition);
    }

    void OnMouseOver()
    {
        Destroy(gameObject);
    }
}
