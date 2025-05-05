using UnityEngine;

public class Mango : MonoBehaviour
{
    [HideInInspector] public MangoCatchMinigame mangoCatch;
    [HideInInspector] public float fallSpeed;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        rectTransform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
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
