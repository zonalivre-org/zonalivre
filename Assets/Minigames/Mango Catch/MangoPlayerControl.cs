using UnityEngine;

public class MangoPlayerControl : MonoBehaviour
{
    [SerializeField] Camera miniGameCamera;
    [SerializeField] GameObject outline;
    [SerializeField] MangoCatch mangoCatch;

    void OnMouseDown()
    {
        if (mangoCatch.canGenerate)
        {
            outline.SetActive(true);
        }
    }

    void OnMouseUp()
    {
        if (mangoCatch.canGenerate)
        {
            outline.SetActive(false);
        }
    }

    void OnMouseDrag()
    {
        if (mangoCatch.canGenerate)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

            Vector3 playerPosition = miniGameCamera.ScreenToWorldPoint(mousePosition);

            playerPosition.z = transform.position.z;
            playerPosition.y = transform.position.y;

            transform.position = playerPosition;
        }
    }
}
