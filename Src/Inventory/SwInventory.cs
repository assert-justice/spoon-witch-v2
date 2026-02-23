using System.Collections.Generic;

namespace SW.Src.Inventory;

public class SwInventory
{
    private readonly Dictionary<SwItemType, float> Inventory = [];
    public void AddItems(SwItemType type, float count){Inventory[type] = Inventory.GetValueOrDefault(type) + count;}
    public void RemoveItems(SwItemType type, float count){Inventory[type] = Inventory.GetValueOrDefault(type) - count;}
    public float CountItems(SwItemType type){return Inventory.GetValueOrDefault(type);}
}
