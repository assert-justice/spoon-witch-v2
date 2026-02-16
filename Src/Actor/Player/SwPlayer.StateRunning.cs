namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwActor
{
	private void BindStateRunning()
	{
		StateMachine.AddState(new(SwState.Running, OnEnterRunning, null, OnTickRunning));
	}
	private void OnEnterRunning(SwState lastState)
	{
		BodySprite.Play("run_" + GetFacing());
	}
	private void OnTickRunning(float dt)
	{
		if(FacingIdx.IsDirty()) BodySprite.Play("run_" + GetFacing());
		if(InputManager.SpoonAttack.IsPressed()) StateMachine.QueueState(SwState.Attacking);
		else if(InputManager.ChargeSling.IsPressed()) StateMachine.QueueState(SwState.Charging);
		else if(InputManager.Dodge.IsPressed()) StateMachine.QueueState(SwState.Dodging);
		else if(InputManager.Move.GetValue().LengthSquared() > 0.1f) StateMachine.QueueState(SwState.Running);
	}
}
