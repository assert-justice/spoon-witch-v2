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
	protected readonly List<SwDamage> IncomingDamageQueue = [];
	private bool IsAwake_ = true;
	protected bool IsAwake{get=>IsAwake_; set
		{
			if(value) Wake();
			else Sleep();
			IsAwake_ = value;
		}
	}
	private Vector2 LastVelocity = Vector2.Down;
	public override void _Ready()
	{
		Health = GetMaxHealth();
	}
	public override void _PhysicsProcess(double delta)
	{
		if(!IsAwake) return;
		float dt = (float)delta;
		if(Velocity.LengthSquared() > SwConstants.EPSILON) LastVelocity = Velocity;
		foreach (var item in Tickers)
		{
			item.Tick(dt);
		}
		Tick(dt);
		MoveAndSlide();
		ApplyDamage();
	}
	protected virtual void Sleep(){}
	protected virtual void Wake(){}
	protected bool IsAlive(){return Health > 0;}
	public virtual void Cleanup()
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
	public virtual float Damage(SwDamage damage, Node2D source)
	{
		// Note, damage effects can *heal* you as well as hurt you depending on what your multiplier is.
		float value = damage.Value;
		if(DamageMultipliers.TryGetValue(damage.Type, out float mul)) value *= mul;
		if(value != 0) IncomingDamageQueue.Add(new(damage.Type, value));
		return value;
	}
	public virtual void Die(){Cleanup();}
	protected abstract float GetMaxHealth();
	protected virtual void ApplyDamage()
	{
		foreach (var damage in IncomingDamageQueue)
		{
			Health -= damage.Value;
			float maxHealth = GetMaxHealth();
			if(Health > maxHealth)
			{
				Health = maxHealth;
			}
			else if(!IsAlive()) Die();
		}
		IncomingDamageQueue.Clear();
	}
}
