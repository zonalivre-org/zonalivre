using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public enum ItemType { TrashBag, Tela };
    public string id; // Identificador único do item (ex: "trashBag", "flySwatter")
    public string displayName; // Nome visível no jogo
    public Sprite icon; // Sprite para o balão do indicador

   
}