using Godot;
using SW.Src.Actor.Player.Component;

namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwActor
{
    [Export] public float Speed = 300;
    [Export] public float ChargeSpeedMul = 0.5f;
	[Export] public float MaxHealth = 100;
	[Export] public float DeathDelay = 1;
	// State management
    public enum SwState
    {
        Default,
        Attacking,
        Dodging,
        Charging,
        UsingItem,
        InSubmenu,
    }
	public SwPlayerStateManager StateManager{get; private set;}
	public SwPlayerControls Controls{get; private set;}
	public SwPlayerAnimator Animator{get; private set;}
	// Overrides
	public override void _Ready()
	{
		Animator = new(this);
		StateManager = new(this);
		Controls = new();
		base._Ready();
	}
	protected override void Tick(float dt)
	{
		Animator.Poll();
		StateManager.Tick(dt);
		Controls.Poll();
	}
	protected override float GetDeathDelay()
    {
        return DeathDelay;
    }
    protected override float GetMaxHealth()
    {
        return MaxHealth;
    }
}
