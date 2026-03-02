using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Effect;
using SW.Src.Global;
using SW.Src.Timer;
using SW.Src.Ui;
using SW.Src.Ui.Menu;
using SW.Src.Utils;

namespace SW.Src.Actor;
public abstract partial class SwActor : CharacterBody2D, ISwDamageable
{
	[Export] public float InvulnerableTime = 0.5f;
	[Export] public float FlickersPerSecond = 8;
	[Export] private bool DebugDrawEnabled = false;
	public SwClock InvulnerableClock;
	public SwClock FlickerClock;
	public float Health;
	private readonly List<ISwTick> Tickers = [];
	protected readonly Dictionary<SwDamageType, float> DamageMultipliers = [];
	public readonly List<SwDamage> IncomingDamageQueue = [];
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
		InvulnerableClock = new(new(){Duration=InvulnerableTime,IsPaused=true});
		FlickerClock = new(new(){Duration=1/FlickersPerSecond,Repeats=true});
	}
	public override void _PhysicsProcess(double delta)
	{
		if(!IsAwake) return;
		float dt = (float)delta;
		HandleInvulnerability(dt);
		if(IsMoving()) LastVelocity = Velocity;
		foreach (var item in Tickers)
		{
			item.Tick(dt);
		}
		Tick(dt);
		MoveAndSlide();
		// LateTick(dt);
		ApplyDamage();
	}
	public bool CanDebugDraw(){return DebugDrawEnabled && SwGlobal.GetSettings().DebugDraw;}
	public void DebugDrawRect(Rect2 rect, Color color)
    {
		if(!CanDebugDraw() || !Main.TryGetHud(out var hud)) return;
		var trans = GetViewportTransform().Inverse();
        void fn(SwHud hud){
            hud.DrawRect(rect * trans, color);
        }
        hud.AddDrawCommand(fn);
    }
    public void DebugDrawLine(Vector2 from, Vector2 to, Color color)
    {
		if(!CanDebugDraw() || !Main.TryGetHud(out var hud)) return;
		var trans = GetViewportTransform().Inverse();
        void fn(SwHud hud){
            hud.DrawLine(from * trans, to * trans, color);
        }
        hud.AddDrawCommand(fn);
    }
    public void DebugDrawText(Vector2 position, string text, Color color)
    {
		if(!CanDebugDraw() || !Main.TryGetHud(out var hud)) return;
		var trans = GetViewportTransform().Inverse();
        void fn(SwHud hud){
            hud.DrawString(ThemeDB.FallbackFont, position * trans, text, HorizontalAlignment.Left, -1, 16, color);
        }
        hud.AddDrawCommand(fn);
    }
	private void HandleInvulnerability(float dt)
	{
		if(!InvulnerableClock.IsRunning()) return;
		InvulnerableClock.Tick(dt);
		if (!InvulnerableClock.IsRunning())
		{
			Visible = true;
			return;
		}
		FlickerClock.Tick(dt);
		Visible = FlickerClock.GetProgress() > 0.5f;
	}
	public bool IsInvulnerable(){return InvulnerableClock.IsRunning();}
	protected virtual void Sleep()
	{
		Velocity = Vector2.Zero;
	}
	protected virtual void Wake(){}
	public bool IsAlive(){return Health > 0;}
	public virtual void Cleanup()
	{
		QueueFree();
	}
	protected virtual void Tick(float dt){}
	// protected virtual void LateTick(float dt){}
	// protected virtual void DebugDraw(DebugDrawCallbacks drawCallbacks){}
	protected T AddTicker<T>(T ticker) where T : ISwTick
	{
		Tickers.Add(ticker);
		return ticker;
	}
	protected SwClock AddClock(SwClockData clockData)
	{
		return AddTicker(new SwClock(clockData));
	}
	public float GetHealth(){return Health;}
	public Vector2 GetLastVelocity(){return LastVelocity;}
	public float GetLastAngle()
	{
		float angle = LastVelocity.Angle();
		return angle >= 0 ? angle : angle + Mathf.Tau;
	}
	public float GetLastAngleRounded()
	{
		const float mul = 4 / Mathf.Tau;
		return Mathf.Round(GetLastAngle() * mul) * SwConstants.HALF_PI;
	}
	public int GetLastFacing4()
	{
		const float mul = 4 / Mathf.Tau;
		return Mathf.RoundToInt(GetLastAngle() * mul) % 4;
	}
	public bool IsMoving(){return Velocity.LengthSquared() > SwConstants.EPSILON;}
	public virtual float Damage(SwDamage damage, Node2D source)
	{
		// Note, damage effects can *heal* you as well as hurt you depending on what your multiplier is.
		if(IsInvulnerable()) return 0;
		float value = damage.Value;
		if(DamageMultipliers.TryGetValue(damage.Type, out float mul)) value *= mul;
		if(value != 0) IncomingDamageQueue.Add(new(damage.Type, value));
		return value;
	}
	public virtual void Die(){}
	protected abstract float GetMaxHealth();
	protected virtual float ApplyDamage()
	{
		if(IncomingDamageQueue.Count == 0) return 0;
		float totalDamage = 0;
		foreach (var damage in IncomingDamageQueue)
		{
			totalDamage += damage.Value;
			Health -= damage.Value;
			float maxHealth = GetMaxHealth();
			if(Health > maxHealth)
			{
				Health = maxHealth;
			}
			else if(!IsAlive()) Die();
		}
		IncomingDamageQueue.Clear();
		InvulnerableClock.Restart();
		return totalDamage;
	}
}
