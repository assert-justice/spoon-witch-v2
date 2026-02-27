using System.Collections.Generic;

namespace SW.Src.Inventory;

public class SwInventory
{
    public readonly Dictionary<SwItemType, SwInventorySlot> Slots = [];
    public void SetSlots(SwInventoryRes[] slots)
    {
        Slots.Clear();
        foreach (var item in slots)
        {
            SwInventorySlot slot = new(item.Type, item.Quantity, item.Capacity, item.Minimum);
            Slots.Add(item.Type, slot);
        }
    }
    public void AddSlot(SwInventorySlot slot)
    {
        Slots.Add(slot.Type, slot);
    }
    public float CountItems(SwItemType type)
    {
        if(!Slots.TryGetValue(type, out var slot)) return 0;
        return slot.Quantity;
    }
    // public void AddItems(SwItemType type, float count){Inventory[type] = Inventory.GetValueOrDefault(type) + count;}
    // public void RemoveItems(SwItemType type, float count){Inventory[type] = Inventory.GetValueOrDefault(type) - count;}
    // public float CountItems(SwItemType type){return Inventory.GetValueOrDefault(type);}
    // private readonly Dictionary<SwItemType, SwInventorSlot> Inventory = [];
    // public bool TryGetSlot(SwItemType type, out SwInventorSlot slot)
    // {
    //     return Inventory.TryGetValue(type, out slot);
    // }
    // public void AddSlot(SwInventorSlot slot){Inventory.Add(slot);}
}
