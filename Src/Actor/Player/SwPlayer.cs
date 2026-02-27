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
	// Misc nodes
	private Label HudLabel;
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
		Inventory.AddItems(SwItemType.SlingBullet, 5);
		// var hud = GetTree().GetNodesInGroup("Hud");
		// if(hud.Count == 1) HudLabel = hud[0].GetChild<Label>(0);
		base._Ready();
	}
	protected override void Tick(float dt)
	{
		StateManager.Tick(dt);
		Controls.Poll();
		Hud.Tick(dt);
		if(SwGlobal.GetInputManager().Pause.IsJustPressed()) Main.Message("pause");
	}
	protected override float GetMaxHealth()
	{
		return MaxHealth;
	}
	// private void UpdateHud()
	// {
	// 	if(HudLabel is null) return;
	// 	string text = $"Health: {Health} Ammo: {Inventory.CountItems(SwItemType.SlingBullet)}";
	// 	HudLabel.Text = text;
	// }
	public void AddItems(SwItemType itemType, float quantity, string itemName = null)
	{
		Inventory.AddItems(itemType, quantity);
		// Send pickup message to hud
		itemName ??= itemType.ToString();
		string verb = quantity > 0 ? "Added" : "Removed";
		string message = $"{verb} {quantity} {itemName}";
		// SwStatic.Log(message);
		Hud.AddMessage(message);
	}
}
