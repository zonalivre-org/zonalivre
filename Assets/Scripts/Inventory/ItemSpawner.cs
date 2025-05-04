using UnityEngine;

// TODO: Este GameObject precisa ter um Collider configurado como IsTrigger = true
// E estar em uma Layer que interaja com a Layer do Player.
public class ItemSpawner : MonoBehaviour
{
    [Header("Configuração do Item Spawner")]
    [SerializeField] private ItemData itemToGive; // Qual ItemData este spawner dá

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou no trigger é o jogador
        if (other.CompareTag("Player")) // Use a tag correta do player
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

            if (playerInventory != null)
            {
                // Define o item no inventário do player (substitui ou adiciona)
                playerInventory.SetItem(itemToGive);
                // O spawner não some, ele continua lá
            }
            else
            {
                Debug.LogWarning("Objeto com tag 'Player' não tem PlayerInventory!", other.gameObject);
            }
        }
    }

    // TODO: Opcional: Feedback visual ou sonoro ao coletar (pode ser na colisão ou no SetItem)
}