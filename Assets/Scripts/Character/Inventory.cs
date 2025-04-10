using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> items = new List<Item>();
    
    public IReadOnlyList<Item> Items => items.AsReadOnly();

    public void AddItem(Item item)
    {
        if (item != null)
        {
            items.Add(item);
            Debug.Log("Добавлен предмет: " + item.itemName);
        }
    }

    public bool RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            Debug.Log("Удалён предмет: " + item.itemName);
            return true;
        }
        return false;
    }
}