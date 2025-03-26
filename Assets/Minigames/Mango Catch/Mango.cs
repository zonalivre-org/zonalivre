using UnityEngine;

public class Mango : MonoBehaviour
{
    [HideInInspector] public MangoCatch mangoCatch;
    [HideInInspector] public float fallSpeed;

    void LateUpdate()
    {
        transform.Translate(Vector3.back * fallSpeed * Time.deltaTime);
    }
    

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MangoPlayer") 
        {
            mangoCatch.AddPoint(1);
            Destroy(gameObject);
        }

        else if (other.tag == "MangoGround")
        {
            Destroy(gameObject);
        }
    }
}
