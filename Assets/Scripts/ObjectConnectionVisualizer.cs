using UnityEngine;
using DG.Tweening;

public class ObjectConnectionVisualizer : MonoBehaviour
{
    [Header("Line Settings")]
    public Transform startPoint;
    public Transform endPoint;
    public Vector3 startOffset;
    public Vector3 endOffset;

    [Header("Arrow Settings")]
    public GameObject arrowPrefab;
    public Vector3 arrowRotationOffsetY;

    [Header("Line Visuals")]
    public LineRenderer lineRenderer;
    public float animationDuration = 0.5f;
    public float displayDuration = 2f;

    private GameObject arrowInstance;
    private SpriteRenderer arrowSprite;
    private Tweener lineTween;
    private Tween arrowFadeTween;

    public void ShowConnector()
    {
        gameObject.SetActive(true);
        lineRenderer.enabled = true;
        Vector3 worldStart = startPoint.position + startOffset;
        Vector3 worldEnd = endPoint.position + endOffset;

        // Instancia a seta se ainda não existir
        if (arrowInstance == null)
        {
            arrowInstance = Instantiate(arrowPrefab, worldStart, Quaternion.identity, transform);
            arrowSprite = arrowInstance.GetComponentInChildren<SpriteRenderer>();

            if (arrowSprite == null)
            {
                Debug.LogError("Prefab da seta precisa conter um SpriteRenderer.");
                return;
            }
        }

        // Posiciona a seta no início (vai ser animada até o fim)
        arrowInstance.transform.position = worldStart;
        arrowSprite.color = new Color(1, 1, 1, 0); // Invisível inicialmente

        // Inicializa a linha
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, worldStart);
        lineRenderer.SetPosition(1, worldStart);

        // Anima a linha até o fim
        lineTween?.Kill();
        lineTween = DOTween.To(
    () => worldStart,
    x =>
    {
        lineRenderer.SetPosition(1, x);
        arrowInstance.transform.position = x;

        Vector3 dir = (x - worldStart).normalized;
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle + arrowRotationOffsetY.y);
        }
    },
    worldEnd,
    animationDuration
)
.OnComplete(() =>
{
    // Fade in da seta
    arrowFadeTween?.Kill();
    arrowFadeTween = arrowSprite.DOFade(1f, 0.2f);

    // Esconde depois de um tempo
    DOVirtual.DelayedCall(displayDuration, HideConnector);
});
    }

    public void HideConnector()
    {
        lineTween?.Kill();
        arrowFadeTween?.Kill();

        if (arrowSprite == null)
            return;

        // Fade out da seta
        arrowSprite.DOFade(0f, 0.2f);

        // Anima linha de volta
        DOTween.To(
            () => lineRenderer.GetPosition(1),
            x =>
            {
                lineRenderer.SetPosition(1, x);
                arrowInstance.transform.position = x;
            },
            lineRenderer.GetPosition(0),
            0.3f
        ).OnComplete(() =>
        {
            lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));
        });
    }
}
