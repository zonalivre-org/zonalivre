using UnityEngine;
using System; // Para Action

public class PlayerInventory : MonoBehaviour
{
    // Evento que outros scripts podem ouvir quando o item no inventário muda
    public event Action<ItemData> OnItemChanged;

    private ItemData _heldItem; // O item que o jogador está segurando atualmente

    // --- Propriedades ---
    public ItemData HeldItem => _heldItem;
    public bool HasItem => _heldItem != null;

    // --- Métodos de Inventário ---

    /// <summary>
    /// Define o item atual no inventário, substituindo qualquer item anterior.
    /// </summary>
    /// <param name="item">O ItemData a ser adicionado/substituído. Use null para remover o item.</param>
    public void SetItem(ItemData item)
    {
        ItemData previousItem = _heldItem;
        _heldItem = item;

        if (_heldItem != null)
        {
            Debug.Log($"Item definido: {_heldItem.displayName}. Substituiu: {previousItem?.displayName ?? "Nada"}"); // Para Debug
        }
        else
        {
            Debug.Log($"Item removido. Anterior: {previousItem?.displayName ?? "Nada"}"); // Para Debug
        }

        // Dispara o evento para notificar outros componentes (como o UI Indicator)
        OnItemChanged?.Invoke(_heldItem);
    }

    public ItemData GetItem()
    {
        if (_heldItem != null)
        {
            return _heldItem;
        }
        return null;
    }

    /// <summary>
    /// Remove o item atual do inventário.
    /// </summary>
    public void RemoveItem()
    {
        SetItem(null); // Definir para null remove o item
    }

    // --- Métodos de Controle (Chamados por outros componentes) ---

    // Não precisamos mais de UseHeldItem ou DropHeldItem no inventário,
    // pois a lógica de "uso" está na interação (ObjectiveInteract) e não há mais "drop" no mundo.
}