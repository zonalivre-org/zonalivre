using UnityEngine;
using System.Collections; // Para IEnumerator

// TODO: Este script deve estar anexado a um GameObject filho do Player, posicionado acima da cabeça.
// Ele precisa de um SpriteRenderer.
[RequireComponent(typeof(SpriteRenderer))]
public class ItemIndicatorUI : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerInventory playerInventory; // Referência ao inventário do player

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false; // Esconde inicialmente
    }

    private void Start()
    {
        // Encontrar o PlayerInventory no Start
        playerInventory = GetComponentInParent<PlayerInventory>(); // Assumindo que PlayerInventory está no pai
        if (playerInventory == null)
        {
            // Tenta encontrar na cena se não estiver no pai (menos ideal, mas funciona)
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        if (playerInventory == null) Debug.LogError("PlayerInventory não encontrado (no pai ou na cena) para ItemIndicatorUI!", this);
        else
        {
            // Assina o evento de mudança de item
            playerInventory.OnItemChanged += HandleItemChanged;
            // Atualiza o indicador com o estado inicial do inventário
            HandleItemChanged(playerInventory.HeldItem);
        }
    }

    private void OnDestroy()
    {
        // Remove a assinatura do evento ao destruir para evitar erros
        if (playerInventory != null)
        {
            playerInventory.OnItemChanged -= HandleItemChanged;
        }
    }

    // Método chamado quando o item no inventário do player muda
    private void HandleItemChanged(ItemData newItem)
    {
        if (spriteRenderer == null) return;

        if (newItem != null)
        {
            Debug.Log("Item removido do inventário. Limpando o indicador.");
            spriteRenderer.sprite = newItem.icon;
            spriteRenderer.enabled = true; // Mostra o sprite
            // TODO: Opcional: Adicionar uma pequena animação de popup/fade aqui
        }
        else
        {
            spriteRenderer.sprite = null; // Limpa o sprite
            spriteRenderer.enabled = false; // Esconde o sprite
             // TODO: Opcional: Adicionar uma pequena animação de fade-out aqui
        }
    }

    // TODO: Adicionar lógica para LookAtCamera se necessário para o SpriteRenderer
}