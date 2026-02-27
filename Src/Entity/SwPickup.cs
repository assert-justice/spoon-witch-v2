using Godot;
using SW.Src.Actor.Player;
using SW.Src.Inventory;

namespace SW.Src.Entity;

public partial class SwPickup : Area2D
{
	[Export] public SwItemType ItemType = SwItemType.SlingBullet;
	[Export] public string ItemName;
	[Export] public string PluralItemName;
	[Export] private float StartQuantity = 5;
	public float Quantity;
	private CollisionShape2D CollisionShape2D;
	private bool NeedsCleanup = false;
	public override void _Ready()
	{
		base._Ready();
		BodyEntered += OnBodyEntered;
		CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		Quantity = StartQuantity;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (NeedsCleanup) Cleanup();
		base._PhysicsProcess(delta);
	}

	protected virtual void Cleanup()
	{
		NeedsCleanup = false;
		Visible = false;
		CollisionShape2D.Disabled = true;
	}

	public virtual void OnBodyEntered(Node2D body)
	{
		if(body is not SwPlayer player) return;
		string name = ItemName;
		if(Mathf.Abs(Quantity) != 1) name = PluralItemName; 
		player.TryAddItems(ItemType, ref Quantity, name);
		if(Quantity == 0) NeedsCleanup = true;
	}
}
