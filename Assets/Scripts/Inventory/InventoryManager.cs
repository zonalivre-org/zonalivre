using System;
using UnityEngine;

[Serializable]
public struct Item
{
    public string name;
    public int id;
    public Texture image;
}
public class InventoryManager : MonoBehaviour
{
    public Item[] itemsList;
    [HideInInspector] public Item currentItem;
    public static InventoryManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        Search(0, true);
    }
    
    public int Search(int id, bool changeItem)
    {
        int i;
        for(i = 0; i <= itemsList.Length && id != itemsList[i].id; i++);  
        if(changeItem) SetItem(itemsList[i]);
        return Array.IndexOf(itemsList, i);
    }
    public void SetItem(Item item)
    {
        currentItem = item;
        Debug.Log("Item atual: " + currentItem.name);
    }
}
