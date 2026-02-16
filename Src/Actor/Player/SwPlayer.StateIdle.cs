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
	}
}
