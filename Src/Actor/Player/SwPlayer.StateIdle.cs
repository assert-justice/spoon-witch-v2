namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwActor
{
	private void BindStateIdle()
	{
		StateMachine.AddState(new(SwState.Idle, OnEnterIdle, null, OnTickIdle));
	}
	private void OnEnterIdle(SwState lastState)
	{
		BodySprite.Play("idle_" + GetFacing());
	}
	private void OnTickIdle(float dt)
	{
		if(FacingIdx.IsDirty()) BodySprite.Play("idle_" + GetFacing());
		if(InputManager.SpoonAttack.IsPressed()) StateMachine.QueueState(SwState.Attacking);
		else if(InputManager.ChargeSling.IsPressed()) StateMachine.QueueState(SwState.Charging);
		else if(InputManager.Dodge.IsPressed()) StateMachine.QueueState(SwState.Dodging);
		else if(InputManager.Move.GetValue().LengthSquared() > 0.1f) StateMachine.QueueState(SwState.Running);
	}
}
