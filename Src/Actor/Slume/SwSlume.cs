using System;
using Godot;
using SW.Src.Actor.Player;
using SW.Src.Actor.Slume.Component;
using SW.Src.Actor.Slume.State;
using SW.Src.Effect;
using SW.Src.Entity;
using SW.Src.Global;
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
	[Export] public float FleeThreshold = 0.5f;
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
		Seeking,
		Fleeing,
		Dead,
	}
	public SwStateMachine<SwSlume, SwState> StateMachine;
	public SwSlumeAnimator Animator{get; private set;}
	public SwSlumeAudio AudioManager{get; private set;}
	public override void _Ready()
	{
		base._Ready();
		Animator = new(this);
		AudioManager = new(this);
		StateMachine = new(InitialState);
		StateMachine.AddState(new SwSlumeStateDefault(this));
		StateMachine.AddState(new SwSlumeStateKnockedBack(this));
		StateMachine.AddState(new SwSlumeStateWandering(this));
		StateMachine.AddState(new SwSlumeStateChasing(this));
		StateMachine.AddState(new SwSlumeStateSeeking(this));
		StateMachine.AddState(new SwSlumeStateFleeing(this));
		StateMachine.AddState(new SwSlumeStateDead(this));
		Hurtbox = GetNode<SwHurtbox>("Hurtbox");
		Hurtbox.OnDamageFn = AudioManager.PlayAttackSound;
		Hitbox = GetNode<CollisionShape2D>("Hitbox");
	}
	protected override void Tick(float dt)
	{
		if(SwGlobal.GetSettings().CreativeMode) StateMachine.QueueState(SwState.Default);
		else if(StateMachine.IsInState(SwState.Default)) StateMachine.QueueState(SwState.Wandering);
		base.Tick(dt);
		AudioManager.Tick(dt);
		StateMachine.Tick(dt);
		DebugDraw();
	}
	protected override float GetMaxHealth()
	{
		return MaxHealth;
	}
	private void DebugDraw()
	{
		if(!CanDebugDraw() || !TryGetPlayer(out var player)) return;
		bool canSeePlayer = CanSeePlayer();
		Color color = canSeePlayer ? Colors.Red : Colors.Green;
		color.A = 0.5f;
		if(canSeePlayer && IsAlive()) DebugDrawLine(Position, player.Position, color);
		if(StateMachine.TryGetState(out var state)) DebugDrawText(Position, state.ToString(), color);
	}

	public override float Damage(SwDamage damage, Node2D source)
	{
		float damageValue = base.Damage(damage, source);
		if(damageValue == 0) return 0;
		StateMachine.QueueStateUnchecked(SwState.KnockedBack);
		DamageSourcePosition = source.Position;
		return damageValue;
	}
	protected override float ApplyDamage()
	{
		float damageValue = base.ApplyDamage();
		if(damageValue == 0) return 0;
		if(IsAlive()) AudioManager.PlayHitSound();
		else AudioManager.PlayDeathSound();
		return damageValue;
	}

	public bool ShouldFlee()
	{
		return CanSeePlayer() && (Health / MaxHealth < FleeThreshold);
	}
}
