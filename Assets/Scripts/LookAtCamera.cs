using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            Vector3 direction = transform.position - Camera.main.transform.position; // Invert the direction
            direction.y = 0; // Ignore the Y-axis to constrain rotation to the Y-axis only
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}