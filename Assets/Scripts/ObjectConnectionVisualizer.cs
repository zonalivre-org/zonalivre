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
    public SpriteRenderer arrowRenderer;
    public Vector3 arrowRotationOffsetY;

    [Header("Line Visuals")]
    public LineRenderer lineRenderer;
    public float animationDuration = 0.5f;
    public float displayDuration = 2f;

    private Tweener lineTween;
    private Tween arrowFadeTween;

    public void ShowConnector()
    {
        Vector3 worldStart = startPoint.position + startOffset;
        Vector3 worldEnd = endPoint.position + endOffset;

        // Inicializa a linha
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, worldStart);
        lineRenderer.SetPosition(1, worldStart); // Começa com tamanho 0

        // Posiciona a seta no final da linha e aplica rotação
        arrowRenderer.transform.position = worldStart;
        Vector3 direction = (worldEnd - worldStart).normalized;
        arrowRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        arrowRenderer.transform.Rotate(0, arrowRotationOffsetY.y, 0); // Ajuste extra de rotação Y
        arrowRenderer.color = new Color(1, 1, 1, 0); // Começa invisível

        // Anima a linha crescendo até o ponto final
        lineTween?.Kill();
        lineTween = DOTween.To(
            () => worldStart,
            x => {
                lineRenderer.SetPosition(1, x);
                arrowRenderer.transform.position = x;
            },
            worldEnd,
            animationDuration
        ).OnComplete(() => {
            // Mostra a seta com fade
            arrowFadeTween?.Kill();
            arrowFadeTween = arrowRenderer.DOFade(1f, 0.2f);

            // Desliga tudo após um tempo
            DOVirtual.DelayedCall(displayDuration, HideConnector);
        });
    }

    public void HideConnector()
    {
        lineTween?.Kill();
        arrowFadeTween?.Kill();

        // Esconde tudo
        arrowRenderer.DOFade(0f, 0.2f);
        DOTween.To(
            () => lineRenderer.GetPosition(1),
            x => {
                lineRenderer.SetPosition(1, x);
                arrowRenderer.transform.position = x;
            },
            lineRenderer.GetPosition(0),
            0.3f
        ).OnComplete(() => {
            lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));
        });
    }
}
