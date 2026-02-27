using Godot;

namespace SW.Src.Inventory;

public class SwInventorySlot
{
    public readonly SwItemType Type;
    public float Capacity;
    public float Minimum;
    public float Quantity_;
    public float Quantity{get=>Quantity_; set{Quantity_ = Mathf.Clamp(value, Minimum, Capacity);}}
    public SwInventorySlot(SwItemType type, float quantity = 0, float capacity = Mathf.Inf, float minimum = 0)
    {
        Type = type;
        Capacity = capacity;
        Minimum = minimum;
        Quantity = quantity;
    }
    public bool TryAddItems(ref float count)
    {
        float startCount = count;
        float val = Quantity + count;
        Quantity += count;
        count = val - Quantity;
        return Mathf.Abs(count) < Mathf.Abs(startCount);
    }
    public bool TryRemoveItems(ref float count)
    {
        float startCount = count;
        float val = Quantity - count;
        Quantity -= count;
        count = val - Quantity;
        return Mathf.Abs(count) < Mathf.Abs(startCount);
    }
    public void Fill()
    {
        Quantity = Capacity;
    }
    public void Empty()
    {
        Quantity = Minimum;
    }
    public bool IsFull(){return Quantity >= Capacity;}
    public bool IsEmpty(){return Quantity <= Minimum;}
}
