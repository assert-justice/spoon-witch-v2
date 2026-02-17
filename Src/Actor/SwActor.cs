using System.Collections.Generic;
using Godot;
using SW.Src.Effect;
using SW.Src.Global;
using SW.Src.Timer;
using SW.Src.Utils;

namespace SW.Src.Actor;
public abstract partial class SwActor : CharacterBody2D
{
	// protected float MaxHealth = 100;
	protected float Health;
	// private readonly List<SwTimer> Timers = [];
	private readonly List<ISwTick> Tickers = [];
	// private readonly List<ISwPoll> Pollers = [];
	protected readonly Dictionary<SwDamageType, float> DamageMultipliers = [];
	// protected readonly Dictionary<SwDamage, float> DamageThresholds = [];
	private Vector2 LastVelocity = Vector2.Down;
	protected bool IsAlive(){return Health > 0;}
	public override void _Ready()
	{
		// MotionMode = MotionModeEnum.Floating;
		Health = GetMaxHealth();
		// AddTicker(new SwClock(GetDeathDelay(), false, null, Cleanup));
		AddClock(new(){Duration = GetDeathDelay(), IsPaused = true, OnFinish = Cleanup});
		// Init();
	}
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		// Maybe have system where tickers and pollers can set their own priority interleaved?
		// In that system actor itself could be a ticker
		foreach (var item in Tickers)
		{
			item.Tick(dt);
		}
		// foreach (var item in Pollers)
		// {
		// 	item.Poll();
		// }
		Tick(dt);
		MoveAndSlide();
		if(Velocity.LengthSquared() > SwConstants.EPSILON) LastVelocity = Velocity;
	}
	// protected virtual void Init(){}
	protected virtual void Cleanup()
	{
		QueueFree();
	}
	protected virtual void Tick(float dt){}
	protected T AddTicker<T>(T ticker) where T : ISwTick
	{
		Tickers.Add(ticker);
		return ticker;
	}
	protected SwClock AddClock(SwClockData clockData)
	{
		return AddTicker(new SwClock(clockData));
	}
	// protected T AddPoller<T>(T poller) where T : ISwPoll
	// {
	// 	Pollers.Add(poller);
	// 	return poller;
	// }
	public Vector2 GetLastVelocity(){return LastVelocity;}
	public virtual float Damage(SwDamage damage)
	{
		// Note, damage effects can *heal* you as well as hurt you depending on what your multiplier is.
		float value = damage.Value;
		if(DamageMultipliers.TryGetValue(damage.Type, out float mul)) value *= mul;
		Health -= value;
		float maxHealth = GetMaxHealth();
		if(Health > maxHealth)
		{
			value = maxHealth - Health;
			Health = maxHealth;
		}
		else if(!IsAlive()) Die();
		return value;
	}
	protected virtual void Die(){}
	protected abstract float GetMaxHealth();
	protected abstract float GetDeathDelay();
}
