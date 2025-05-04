using UnityEngine;

// TODO: Este script precisa ser capaz de ser interagido (clicado) pelo player.
// A lógica de clique virá do PlayerController que fará um Raycast.
public class WorldItem : MonoBehaviour
{
    [Header("Configuração do Item")]
    [SerializeField] private ItemData itemData; // Qual ItemData este objeto representa

    // Para itens que podem ser retornados, precisamos saber sua posição original
    // ou ter uma referência para o GameObject original se ele for apenas desativado
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    // private GameObject originalGameObject; // Alternativa: referência ao original

    private PlayerInventory playerInventory; // Referência obtida no Start/Awake

    private void Awake()
    {
        // Encontrar o PlayerInventory no Start ou Awake (método mais robusto depende da estrutura)
        playerInventory = FindObjectOfType<PlayerInventory>(); // Encontra o primeiro na cena

        if (playerInventory == null) Debug.LogError("PlayerInventory não encontrado na cena!", this);

        // Guarda a posição original se o item puder ser retornado
        if (itemData != null)
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            // originalGameObject = this.gameObject; // Se estiver desativando/reativando
        }
    }

    // Este método será chamado pelo PlayerController quando o jogador clicar neste item
    private void OnTriggerEnter( Collider other )
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Clicou no item!");
        if (playerInventory == null) return;
        
        playerInventory.SetItem(itemData); // Adiciona o item ao inventário();
    }

}