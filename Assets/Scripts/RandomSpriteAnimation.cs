
using UnityEngine;
using UnityEngine.UI;

public class RandomSpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private bool onEnable = false;
    [SerializeField] private bool usingSpriteRenderer = false;
    [SerializeField] private bool usingImageComponent = false;
    private SpriteRenderer spriteRenderer;
    private Image spriteImage;

    void Start()
    {
        if (usingSpriteRenderer && usingImageComponent)
        {
            Debug.LogError("Both SpriteRenderer and Image components cannot be used at the same time.");
            return;
        }
        else if (usingSpriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component not found on this GameObject.");
                return;
            }
        }
        else if (usingImageComponent)
        {
            spriteImage = GetComponent<Image>();
            if (spriteImage == null)
            {
                Debug.LogError("Image component not found on this GameObject.");
                return;
            }
        }
        else
        {
            Debug.LogError("Either SpriteRenderer or Image component must be used.");
            return;
        }

        if (sprites.Length == 0)
        {
            Debug.LogError(gameObject.name + ": No sprites assigned to the RandomSpriteAnimation script.");
            return;
        }

        if (!onEnable)
        {
            InvokeRepeating(nameof(UpdateSprite), 0f, frameRate);
        }
    }

    void OnEnable()
    {
        if (onEnable)
        {
            InvokeRepeating(nameof(UpdateSprite), 0f, frameRate);
        }
    }

    private void UpdateSprite()
    {
        if (sprites.Length == 0) return;

        int randomIndex = Random.Range(0, sprites.Length);
        if (usingImageComponent)
            spriteImage.sprite = sprites[randomIndex];
        else if (usingSpriteRenderer)
        spriteRenderer.sprite = sprites[randomIndex];
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(UpdateSprite));
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(UpdateSprite));
    }
}
