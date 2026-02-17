using Godot;
using SW.Src.Global;

namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwActor
{
	private void BindStateRunning()
	{
		StateMachine.AddState(new(SwState.Running, OnEnterRunning, null, OnTickRunning));
	}
	private void OnEnterRunning(SwState lastState)
	{
		BodySprite.Play("run2_" + GetFacing());
	}
	private void OnTickRunning(float dt)
	{
		if(FacingIdx.IsDirty()) BodySprite.Play("run2_" + GetFacing());
		// GD.Print(FacingIdx.IsDirty());
		Velocity = InputManager.Move.GetValue() * Speed;
		if(Velocity.LengthSquared() > SwConstants.EPSILON) IdleTime.SetDuration(0.1f);
		// if(Velocity.LengthSquared() < SwConstants.EPSILON) StateMachine.QueueState(SwState.Idle);
	}
}
