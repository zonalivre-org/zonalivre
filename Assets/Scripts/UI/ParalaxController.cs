using UnityEngine;

public class ParalaxController : MonoBehaviour
{
    [SerializeField] private RectTransform layer1;
    [SerializeField] private RectTransform layer2;
    [SerializeField] private RectTransform layer3;
    [SerializeField] private float layer1Speed = 10f;
    [SerializeField] private float layer2Speed = 20f;
    [SerializeField] private float layer3Speed = 30f;

    private float layer1Width, layer2Width, layer3Width;
    private Vector2 layer1Start, layer2Start, layer3Start;

    void Start()
    {
        layer1Width = layer1.rect.width;
        layer2Width = layer2.rect.width;
        layer3Width = layer3.rect.width;

        layer1Start = layer1.anchoredPosition;
        layer2Start = layer2.anchoredPosition;
        layer3Start = layer3.anchoredPosition;

        layer1.sizeDelta = new Vector2(layer1Width * 2, layer1.sizeDelta.y);
        layer2.sizeDelta = new Vector2(layer2Width * 2, layer2.sizeDelta.y);
        layer3.sizeDelta = new Vector2(layer3Width * 2, layer3.sizeDelta.y);

    }

    void Update()
    {
        MoveLayer(layer1, layer1Speed, layer1Width, layer1Start);
        MoveLayer(layer2, layer2Speed, layer2Width, layer2Start);
        MoveLayer(layer3, layer3Speed, layer3Width, layer3Start);
    }

    private void MoveLayer(RectTransform layer, float speed, float width, Vector2 startPos)
    {
        float move = speed * Time.deltaTime;
        Vector2 pos = layer.anchoredPosition;
        pos.x -= move;

        // Repeat logic: when the layer moves a full width to the left, reset to start
        if (pos.x <= (startPos.x - width))
            pos.x += width;

        layer.anchoredPosition = pos;
    }
}
