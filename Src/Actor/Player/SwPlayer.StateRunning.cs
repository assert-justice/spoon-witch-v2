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
		Velocity = InputManager.Move.GetValue() * Speed;
		if(Velocity.LengthSquared() < SwConstants.EPSILON) StateMachine.QueueState(SwState.Idle);
	}
}
