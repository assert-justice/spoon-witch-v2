using System.Collections.Generic;
using Godot;
using SW.Src.Effect;
using SW.Src.Global;
using SW.Src.Timer;
using SW.Src.Utils;

namespace SW.Src.Actor;
public abstract partial class SwActor : CharacterBody2D, ISwDamageable
{
	protected float Health;
	private readonly List<ISwTick> Tickers = [];
	protected readonly Dictionary<SwDamageType, float> DamageMultipliers = [];
	private Vector2 LastVelocity = Vector2.Down;
	protected bool IsAlive(){return Health > 0;}
	public override void _Ready()
	{
		Health = GetMaxHealth();
		// AddClock(new(){Duration = GetDeathDelay(), IsPaused = true, OnFinish = Cleanup});
	}
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		foreach (var item in Tickers)
		{
			item.Tick(dt);
		}
		Tick(dt);
		MoveAndSlide();
		if(Velocity.LengthSquared() > SwConstants.EPSILON) LastVelocity = Velocity;
	}
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
	public Vector2 GetLastVelocity(){return LastVelocity;}
	public float GetLastAngle()
	{
		float angle = LastVelocity.Angle();
		return angle >= 0 ? angle : angle + Mathf.Tau;
	}
	public float GetLastAngleRounded()
	{
		const float mul = 4 / Mathf.Tau;
		return Mathf.Round(GetLastAngle() * mul) * SwStatic.HALF_PI;
	}
	public int GetLastFacing4()
	{
		const float mul = 4 / Mathf.Tau;
		return Mathf.RoundToInt(GetLastAngle() * mul) % 4;
	}
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
}
