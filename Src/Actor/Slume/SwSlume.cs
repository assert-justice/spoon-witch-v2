using Godot;
using SW.Src.Actor.Slume.Component;
using SW.Src.Actor.Slume.State;
using SW.Src.Effect;
using SW.Src.Entity;
using SW.Src.Utils;

namespace SW.Src.Actor.Slume;

public partial class SwSlume : SwEnemy
{
	[Export] public float MaxHealth = 100;
	[Export] public float Speed = 75;
	[Export] public float WanderSpeedMul = 0.5f;
	[Export] public float KnockBackTime = 0.25f;
	[Export] public float KnockBackBaseSpeed = 3;
	[Export] public float GiveUpTime = 3;
	[Export] public float MinWanderDistance = 300;
	[Export] public float MaxWanderDistance = 500;
	[Export] public float CloseEnough = 8;
	[Export] private SwState InitialState = SwState.Default;
	public Vector2 DamageSourcePosition = Vector2.Zero;
	public SwHurtbox Hurtbox;
	public CollisionShape2D Hitbox;
	public enum SwState
	{
		Default,
		KnockedBack,
		Wandering,
		Chasing,
		Dead,
	}
	public SwStateMachine<SwSlume, SwState> StateMachine;
	public SwSlumeAnimator Animator{get; private set;}
	public override void _Ready()
	{
		base._Ready();
		Animator = new(this);
		StateMachine = new(InitialState);
		StateMachine.AddState(new SwSlumeStateDefault(this));
		StateMachine.AddState(new SwSlumeStateKnockedBack(this));
		StateMachine.AddState(new SwSlumeStateWandering(this));
		StateMachine.AddState(new SwSlumeStateChasing(this));
		StateMachine.AddState(new SwSlumeStateDead(this));
		Hurtbox = GetNode<SwHurtbox>("Hurtbox");
		Hitbox = GetNode<CollisionShape2D>("Hitbox");
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
		// Slume cannot be harmed in knockback state
		// if(StateMachine.IsInState(SwState.KnockedBack)) return 0;
		float damageValue = base.Damage(damage, source);
		if(damageValue == 0) return 0;
		StateMachine.QueueStateUnchecked(SwState.KnockedBack);
		DamageSourcePosition = source.Position;
		return damageValue;
	}
	public override void Die()
	{
		// We're handling dying in the knockback state
	}
}
