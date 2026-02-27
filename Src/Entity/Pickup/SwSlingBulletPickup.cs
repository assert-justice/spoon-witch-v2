using Godot;

namespace SW.Src.Entity.Pickup;

public partial class SwSlingBulletPickup : SwPickup
{
	public override void OnBodyEntered(Node2D body)
	{
		base.OnBodyEntered(body);
		for (int idx = 1 + (int)Quantity; idx < GetChildCount(); idx++)
		{
			GetChild<Node2D>(idx).Visible = false;
		}
	}
}
