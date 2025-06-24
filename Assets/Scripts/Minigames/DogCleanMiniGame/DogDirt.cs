using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DogDirt : MonoBehaviour
{
    private Image dirtImage;
    public float cleanSpeed = 0.1f;

    [SerializeField] private DogCleanMiniGame dogCleanMiniGame;

    void Start()
    {
        dirtImage = GetComponent<Image>();
    }

    public void CleanDirt()
    {
        if (gameObject.activeSelf)
        {
            if (dirtImage.color.a <= 0.3f)
            {
                dogCleanMiniGame.DecreaseDirtCount();
                gameObject.SetActive(false);
            }
            else
            {
                // Otherwise, reduce the alpha value to simulate cleaning
                Color currentColor = dirtImage.color;
                currentColor.a -= cleanSpeed * Time.deltaTime; // Adjust this value to control the cleaning speed
                dirtImage.color = currentColor;
            }
        }
    }
}
