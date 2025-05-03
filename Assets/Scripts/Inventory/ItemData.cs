using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public enum ItemType { TrashBag, Tela };
    public string id; // Identificador único do item (ex: "trashBag", "flySwatter")
    public string displayName; // Nome visível no jogo
    public Sprite icon; // Sprite para o balão do indicador

    [Tooltip("O script (MonoBehaviour) da interação/minigame que este item 'desbloqueia'.")]
    public GameObject associatedInteractionPrefab; // Referência ao Prefab do GameObject que tem o script de interação
    // Usamos Prefab aqui porque a interação pode ser desativada/ativada no mundo.
}