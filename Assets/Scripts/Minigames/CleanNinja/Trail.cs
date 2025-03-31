using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Camera miniGameCamera;

    void LateUpdate()
    {
        Vector2 pos = miniGameCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}
