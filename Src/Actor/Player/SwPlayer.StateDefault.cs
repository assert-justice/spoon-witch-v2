namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
	private void BindStateDefault()
	{
        BindState(new()
        {
            State = SwPlayerState.Default, 
            OnEnterState = OnEnterDefault, 
            OnTick = OnTickDefault
        });
	}
	private void OnEnterDefault(SwPlayerState lastState)
    {
        PlayBodyAnimFaced(IsMoving.Value ? "run" : "idle", 2);
    }
	private void OnTickDefault(float dt)
    {
        Velocity = InputManager.Move.GetValue() * Speed;
        if(IsMoving.IsDirty()) PlayBodyAnimFaced(IsMoving.Value ? "run" : "idle", 2);
    }
}
