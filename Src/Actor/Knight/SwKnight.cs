using Godot;
using SW.Src.Actor.Knight.Component;
using SW.Src.Actor.Knight.State;
using SW.Src.Effect;
using SW.Src.Global;
using SW.Src.Timer;
using SW.Src.Utils;

namespace SW.Src.Actor.Knight;

public partial class SwKnight : SwEnemy
{
	[ExportGroup("Health")]
	[Export] public float MaxHealth = 300;
	[Export] public float FleeThreshold = 0;
	[ExportGroup("Movement")]
	[Export] public float Speed = 30;
	[Export] public float WanderSpeedMul = 1;
	[Export] public float ChargeSpeedMul = 2;
	[Export] public float ChargeRadius = 200;
	[Export] public float ChargeRecoveryTime = 10;
	[ExportGroup("Wander")]
	[Export] public float KnockBackTime = 0.1f;
	[Export] public float KnockBackBaseSpeed = 1;
	[Export] public float GiveUpTime = 3;
	[Export] public float MinWanderDistance = 300;
	[Export] public float MaxWanderDistance = 500;
	[Export] public float CloseEnough = 8;
	[ExportGroup("Attack")]
	[Export] public float AttackRange = 32;
	[Export] public float AttackDelayFrames = 3;
	[Export] public float AttackCooldown = 1.5f;
	[ExportGroup("State")]
	[Export] private SwState InitialState = SwState.Default;
	public Vector2 DamageSourcePosition = Vector2.Zero;
	public SwClock ChargeRecoveryClock;
	public SwClock AttackCooldownClock;
	public enum SwState
	{
		Default,
		Attacking,
		KnockedBack,
		Wandering,
		Chasing,
		Seeking,
		Fleeing,
		Dead,
	}
	public SwStateMachine<SwKnight, SwState> StateMachine;
	public SwKnightAnimator Animator;
	public SwKnightAudio AudioManager;
	public SwKnightEvoker Evoker;
	public override void _Ready()
	{
		base._Ready();
		ChargeRecoveryClock = new(new(){Duration=ChargeRecoveryTime, IsPaused = true});
		AttackCooldownClock = new(new(){Duration=AttackCooldown,IsPaused=true});
		Animator = new(this);
		AudioManager = new(this);
		StateMachine = new(InitialState);
		StateMachine.AddState(new SwKnightStateDefault(this));
		StateMachine.AddState(new SwKnightStateAttacking(this));
		StateMachine.AddState(new SwKnightStateKnockedBack(this));
		StateMachine.AddState(new SwKnightStateWandering(this));
		StateMachine.AddState(new SwKnightStateChasing(this));
		StateMachine.AddState(new SwKnightStateSeeking(this));
		StateMachine.AddState(new SwKnightStateFleeing(this));
		StateMachine.AddState(new SwKnightStateDead(this));
		// StateMachine.LogStates = true;
		Evoker = new(this);
	}
	protected override void Tick(float dt)
	{
		// if(IsPassive()) StateMachine.QueueState(SwState.Default);
		// else if(StateMachine.IsInState(SwState.Default)) StateMachine.QueueState(SwState.Wandering);
		base.Tick(dt);
		ChargeRecoveryClock.Tick(dt);
		AttackCooldownClock.Tick(dt);
		AudioManager.Poll();
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
