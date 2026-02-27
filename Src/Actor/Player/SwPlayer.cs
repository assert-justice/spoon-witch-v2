using System;
using Godot;
using SW.Src.Actor.Player.Component;
using SW.Src.Effect;
using SW.Src.Global;
using SW.Src.Inventory;
using SW.Src.Ui;

namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwActor
{
	[ExportGroup("Health")]
	[Export] public float MaxHealth = 100;
	[Export] public float HealAmount = 30;
	[Export] public float HealTime = 1;
	[Export] public float HealMovementSpeedMul = 0.25f;
	[Export] public SwDamage[] DamageResistances = [];
	[ExportGroup("Movement")]
	[Export] public float Speed = 300;
	[ExportGroup("Dodge")]
	[Export] public float DodgeSpeedMul = 1.5f;
	[Export] public float DodgeRecoveryTime = 0.25f;
	[Export] public float DodgeDefaultTime = 0.25f;
	[ExportGroup("Spoon")]
	[Export] public SwDamage[] SpoonDamages = [];
	[Export] public float SpoonDamageMul = 1;
	[Export] public float SpoonRecoveryTime = 0.25f;
	[Export] public float SpoonDefaultTime = 0.25f;
	[ExportGroup("Sling")]
	[Export] public PackedScene SlingBulletScene;
	[Export] public float SlingBulletSpeed = 100;
	[Export] public SwDamage[] SlingDamages = [];
	[Export] public float SlingDamageMul = 1;
	[Export] public float SlingMovementSpeedMul = 0.5f;
	[Export] public float SlingChargeTime = 0.75f;
	[ExportGroup("Inventory")]
	[Export] private SwInventoryRes[] StartingInventory = [];
	// [Export] public float SlingRecoveryTime = 0.25f;
	// [Export] public float SlingDefaultTime = 0.25f;
	// Data
	public SwInventory Inventory = new();
	// State management
	public enum SwState
	{
		Default,
		Attacking,
		Dodging,
		SlingCharging,
		SlingCharged,
		UsingItem,
		InSubmenu,
	}
	// Components
	public SwPlayerStateManager StateManager{get; private set;}
	public SwPlayerControls Controls{get; private set;}
	public SwPlayerAnimator Animator{get; private set;}
	public SwPlayerEvoker Evoker{get; private set;}
	public SwPlayerHud Hud{get; private set;}
	// Overrides
	public override void _Ready()
	{
		Animator = new(this);
		StateManager = new(this);
		Controls = new(this);
		Evoker = new(this);
		BindComponents();
		Inventory.SetSlots(StartingInventory);
		base._Ready();
	}
	protected override void Tick(float dt)
	{
		StateManager.Tick(dt);
		Controls.Poll();
		Hud.Tick(dt);
		if(SwGlobal.GetInputManager().Pause.IsJustPressed()) Main.Message("pause");
	}
	// protected override void DebugDraw(DebugDrawCallbacks drawCallbacks)
	// {
	// 	Color color = Colors.Red;
	// 	color.A = 0.5f;
	// 	Vector2 boxSize = new(32, 32);
	// 	drawCallbacks.DrawRect(new(Position - boxSize * 0.5f, boxSize), color);
	// }

	protected override float GetMaxHealth()
	{
		return MaxHealth;
	}
	public bool TryAddItems(SwItemType itemType, ref float count, string itemName = null)
	{
		float startCount = count;
		if(!Inventory.Slots.TryGetValue(itemType, out var slot)) return false;
		if(!slot.TryAddItems(ref count)) return false;
		// Send pickup message to hud
		itemName ??= itemType.ToString();
		string verb = startCount > 0 ? "Added" : "Removed";
		string message = $"{verb} {startCount - count} {itemName}";
		Hud.AddMessage(message);
		return true;
	}
}
