namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
	private void BindStateDefault()
	{
        BindState(new()
        {
            State = SwPlayerState.Default, 
            OnEnterState = OnEnterDefault, 
            OnExitState = OnExitDefault, 
            OnTick = OnTickDefault
        });
	}
	private void OnEnterDefault(SwPlayerState lastState){}
	private void OnExitDefault(SwPlayerState lastState){}
	private void OnTickDefault(float dt){}
}
