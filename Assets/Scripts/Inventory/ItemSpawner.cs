using UnityEngine;

// Este script representa um ponto no mundo que dá um item ao player quando ele entra no trigger.
// A coleta é ativada por um clique do player no spawner que habilita o trigger.
[RequireComponent(typeof(Collider))] // Garante que há um Collider (para o trigger)
public class ItemSpawner : MonoBehaviour
{
    [Header("Configuração do Item Spawner")]
    [Tooltip("O ItemData que este spawner dá ao jogador.")]
    [SerializeField] private ItemData itemToGive; // Qual ItemData este spawner dá

    // --- Controle de Interação (Baseado em Clique) ---
    [Tooltip("Indica se a interação por trigger está habilitada (ativada por clique).")]
    public bool enable = false; // Flag: Habilita o trigger check em OnTriggerEnter após clique.

    // Usaremos a Layer do próprio GameObject para o Raycast
    private LayerMask thisObjectLayerMask; // NOVO: LayerMask gerada a partir da layer deste objeto.

    // --- Componentes Essenciais (Encontrados em Awake/Start) ---
    // Referência ao inventário do player (encontrada no trigger)
    // private PlayerInventory playerInventory; // Não precisa ser SerializedField, será encontrado no trigger.

    // --- Inicialização ---
    void Awake() // Use Awake para garantir que a layer seja obtida antes de Start de outros scripts
    {
        // Obtém o índice da layer deste GameObject
        int thisLayerIndex = gameObject.layer;

        // Converte o índice da layer para uma LayerMask
        // Uma LayerMask é um bitmask. O bit correspondente à layer é 1.
        thisObjectLayerMask = 1 << thisLayerIndex;

        // Removido: layerMask = LayerMask.; // Linha incompleta e desnecessária agora.

        // Debug.Log($"ItemSpawner '{gameObject.name}' na Layer {LayerMask.LayerToName(thisLayerIndex)} (Mask: {thisObjectLayerMask.value})"); // Para Debug
    }

    // --- Loops de Update (Gerenciamento de Input) ---
    void Update()
    {
      // Verifica por cliques do mouse no Update
      if (Input.GetMouseButtonDown(0))
      {
        SelectObjective(); // Chama a lógica de seleção/habilitação por clique
      }
    }

    // --- Lógica de Trigger (Habilitada por Clique) ---
    // Este método é chamado uma vez quando outro Collider entra no trigger.
    private void OnTriggerEnter(Collider other)
    {
        // A lógica de trigger só funciona se 'enable' for true (definido por SelectObjective)
        // e se o objeto que entrou no trigger for o Player.
        if (enable && other.CompareTag("Player")) // Use a tag correta do player
        {
            // Debug.Log("Player entrou na área do ItemSpawner habilitado!"); // Para Debug

            // Desabilita a flag 'enable' imediatamente após o trigger ser ativado pelo player.
            // Isso garante que o item seja pego apenas uma vez por "habilitação" de clique.
            enable = false; // Desabilita o trigger check para futuras colisões

            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

            if (playerInventory != null)
            {
                // Define o item no inventário do player (substitui qualquer item anterior)
                playerInventory.SetItem(itemToGive);
                // Debug.Log($"Item '{itemToGive?.displayName}' coletado."); // Para Debug

                // O spawner não some, ele continua lá no mundo.
                // TODO: Opcional: Adicionar um efeito visual/sonoro aqui para feedback de coleta.
            }
            else
            {
                Debug.LogWarning("Objeto com tag 'Player' não tem PlayerInventory!", other.gameObject);
            }
        }
        // else { Debug.Log($"OnTriggerEnter: enable={enable}, tag={other.tag}"); } // Debug para entender o trigger
    }


    // --- Mecanismo de Seleção por Clique (Habilita o Trigger) ---
    // Chamado por Update para verificar se o player clicou neste spawner.
    private void SelectObjective()
    {
        RaycastHit hit;
        // Lança um raio a partir da posição do mouse nas camadas definidas por thisObjectLayerMask.
        // Isso garantirá que o Raycast acerte APENAS objetos na mesma layer deste spawner.
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, thisObjectLayerMask))
        {
            // Verifica se o collider que o Raycast acertou pertence a ESTE GameObject.
            // É importante verificar o GameObject da hit.collider.
            if (hit.collider.gameObject == this.gameObject)
            {
                // Se clicou NESTE ItemSpawner, habilita a interação por trigger.
                enable = true;
                // Debug.Log($"ItemSpawner {gameObject.name} selecionado por clique. Trigger habilitado!"); // Para Debug
                // TODO: Opcional: Mostrar feedback visual de seleção/habilitação.
            }

        }

    }

}