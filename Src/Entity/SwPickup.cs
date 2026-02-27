using Godot;
using SW.Src.Actor.Player;
using SW.Src.Global;
using SW.Src.Inventory;

namespace SW.Src.Entity;

public partial class SwPickup : Area2D
{
	[Export] public SwItemType ItemType = SwItemType.SlingBullet;
	[Export] public string ItemName;
	[Export] public string PluralItemName;
	[Export] public float Quantity = 5;
	private CollisionShape2D CollisionShape2D;
	private bool NeedsCleanup = false;
	public override void _Ready()
	{
		base._Ready();
		BodyEntered += OnBodyEntered;
		CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
	}
	public override void _PhysicsProcess(double delta)
	{
		if (NeedsCleanup) Cleanup();
		base._PhysicsProcess(delta);
	}

	private void Cleanup()
	{
		NeedsCleanup = false;
		Visible = false;
		CollisionShape2D.Disabled = true;
		// Todo: finish cleanup
	}

	public void OnBodyEntered(Node2D body)
	{
		if(body is not SwPlayer player) return;
		string name = ItemName;
		if(Mathf.Abs(Quantity) != 1) name = PluralItemName; 
		player.AddItems(ItemType, Quantity, name);
		NeedsCleanup = true;
	}
}
