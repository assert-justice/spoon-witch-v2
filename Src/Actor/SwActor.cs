using System.Collections.Generic;
using Godot;
using SW.Src.Global;
using SW.Src.Timer;

namespace SW.Src.Actor;
public abstract partial class SwActor : CharacterBody2D
{
	[Export] private float MaxHealth = 100;
	private float Health;
	private readonly List<SwTimer> Timers = [];
	private Vector2 LastVelocity = Vector2.Down;
	public override void _Ready()
	{
		MotionMode = MotionModeEnum.Floating;
		Health = MaxHealth;
		Init();
	}
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		foreach (var t in Timers)
		{
			t.Tick(dt);
		}
		Tick(dt);
		MoveAndSlide();
		if(Velocity.LengthSquared() > SwConstants.EPSILON) LastVelocity = Velocity;
	}
	protected virtual void Init(){}
	protected virtual void Cleanup(){}
	protected virtual void Tick(float dt){}
	protected T AddTimer<T>(T timer) where T : SwTimer
	{
		Timers.Add(timer);
		return timer;
	}
	public Vector2 GetLastVelocity(){return LastVelocity;}
}
