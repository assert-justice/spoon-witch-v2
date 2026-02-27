using Godot;

namespace SW.Src.Inventory;

[GlobalClass]
public partial class SwInventoryRes : Resource
{
    [Export] public SwItemType Type = SwItemType.SlingBullet;
    [Export] public float Quantity = 0;
    [Export] public float Capacity = Mathf.Inf;
    [Export] public float Minimum = 0;
}
