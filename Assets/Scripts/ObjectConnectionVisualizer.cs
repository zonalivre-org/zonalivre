using UnityEngine;

[RequireComponent(typeof(LineRenderer))] // Garante que o GameObject tenha um LineRenderer
public class ObjectConnectionVisualizer : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O primeiro GameObject (ponto inicial da linha).")]
    [SerializeField] private GameObject startObject;
    [Tooltip("O segundo GameObject (ponto final da linha).")]
    [SerializeField] private GameObject endObject;
    [Tooltip("O GameObject que representa a seta (opcional). Será posicionado no endObject.")]
    [SerializeField] private GameObject arrowObject; // Objeto da seta (pode ser um cone, etc.)

    [Header("Configuração")]
    [Tooltip("Offset local para o ponto inicial da linha (em relação ao startObject).")]
    [SerializeField] private Vector3 startOffset = Vector3.zero;
    [Tooltip("Offset local para o ponto final da linha (em relação ao endObject).")]
    [SerializeField] private Vector3 endOffset = Vector3.zero;
    [Tooltip("Offset local para a seta (em relação ao endObject).")]
    [SerializeField] private Vector3 arrowOffset = Vector3.zero;
    [Tooltip("Ajuste da rotação da seta em torno do eixo Y (em graus).")]
    [SerializeField] private float arrowYRotationAdjust = 0f; // Para corrigir orientação da seta

    // Componentes
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // A linha sempre terá 2 pontos
        lineRenderer.enabled = false; // Começa invisível

        SetConnection(startObject, endObject); // Inicializa com os objetos definidos
        // Garante que a seta esteja desativada no início se existir
        if (arrowObject != null)
        {
            arrowObject.SetActive(false);
        }
    }

    void Update()
    {
        // Atualiza a linha e a seta APENAS se o visualizador estiver ativo (enabled)
        // e se os objetos de início/fim estiverem definidos.
        if (this.enabled && startObject != null && endObject != null)
        {
            UpdateConnection();
        }
        else
        {
             // Se este script está desativado, garante que a linha e a seta também sumam.
             if (lineRenderer.enabled) lineRenderer.enabled = false;
             if (arrowObject != null && arrowObject.activeSelf) arrowObject.SetActive(false);
        }
    }

    /// <summary>
    /// Define os objetos a serem conectados e mostra o visualizador.
    /// </summary>
    public void SetConnection(GameObject startObj, GameObject endObj)
    {
        startObject = startObj;
        endObject = endObj;

        // Só mostra se ambos os objetos são válidos
        if (startObject != null && endObject != null)
        {
            this.enabled = true; // Ativa este script para que o Update funcione
            lineRenderer.enabled = true; // Mostra a linha
            if (arrowObject != null) arrowObject.SetActive(true); // Mostra a seta

            // Atualiza a conexão imediatamente
            UpdateConnection();
        }
        else
        {
            // Se algum objeto for nulo, esconde tudo
            HideConnection();
        }
    }

    /// <summary>
    /// Esconde o visualizador de conexão.
    /// </summary>
    public void HideConnection()
    {
        startObject = null;
        endObject = null;
        this.enabled = false; // Desativa este script (para parar o Update)
        lineRenderer.enabled = false; // Esconde a linha
        if (arrowObject != null) arrowObject.SetActive(false); // Esconde a seta
    }

    // Atualiza a posição da linha e da seta
    private void UpdateConnection()
    {
        // Garante que os objetos ainda são válidos no Update
        if (startObject == null || endObject == null)
        {
            HideConnection(); // Esconde se um deles sumiu
            return;
        }

        // Calcula as posições dos pontos da linha no espaço do mundo, aplicando os offsets
        Vector3 startPointWorld = startObject.transform.position + startOffset;
        Vector3 endPointWorld = endObject.transform.position + endOffset;

        // Define os pontos da linha
        lineRenderer.SetPosition(0, startPointWorld);
        lineRenderer.SetPosition(1, endPointWorld);

        // Posiciona e rotaciona a seta (se existir)
        if (arrowObject != null)
        {
            // Posiciona a seta no ponto final da linha, aplicando o offset da seta
            arrowObject.transform.position = endPointWorld + arrowOffset;

            // Calcula a direção do ponto inicial para o ponto final
            Vector3 direction = endPointWorld - startPointWorld;

            if (direction.sqrMagnitude > 0.0001f) // Evita erro de LookRotation se os pontos forem muito próximos
            {
                 // Cria uma rotação que olha na direção, mantendo o Up do mundo
                 Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

                 // Aplica a rotação calculada
                 arrowObject.transform.rotation = lookRotation;

                 // Aplica o ajuste de rotação Y se necessário (para corrigir a orientação padrão do seu modelo de seta)
                 arrowObject.transform.Rotate(Vector3.up, arrowYRotationAdjust);
            }
             // else { seta fica na rotação padrão ou última conhecida }
        }
    }
}