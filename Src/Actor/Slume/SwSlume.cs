using Godot;
using SW.Src.Actor.Slume.Component;
using SW.Src.Actor.Slume.State;
using SW.Src.Effect;
using SW.Src.Utils;

namespace SW.Src.Actor.Slume;

public partial class SwSlume : SwEnemy
{
	[Export] public float MaxHealth = 100;
	[Export] public float Speed = 100;
	[Export] public float DeathDelay = 1;
	public enum SwState
	{
		Default,
		KnockedBack,
		Dead,
	}
	private SwStateMachine<SwSlume, SwState> StateMachine;
	public SwSlumeAnimator Animator{get; private set;}
	public override void _Ready()
	{
		base._Ready();
		Animator = new(this);
		StateMachine = new(SwState.Default);
		StateMachine.AddState(new SwSlumeStateDefault(this));
		StateMachine.AddState(new SwSlumeStateDead(this));
	}
	protected override void Tick(float dt)
	{
		base.Tick(dt);
		StateMachine.Tick(dt);
	}
	protected override float GetMaxHealth()
	{
		return MaxHealth;
	}
	public override float Damage(SwDamage damage, Node2D source)
	{
		GD.Print(damage.Type, " ", damage.Value);
		return base.Damage(damage, source);
	}
	public override void Die()
	{
		StateMachine.QueueState(SwState.Dead);
	}
}
