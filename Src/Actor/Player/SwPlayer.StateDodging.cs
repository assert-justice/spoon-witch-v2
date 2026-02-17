namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
	private void BindStateDodging()
	{
		BindState(new()
        {
            State = SwPlayerState.Dodging, 
            OnEnterState = OnEnterDodging, 
            OnExitState = OnExitDodging, 
            OnTick = OnTickDodging
        });
	}
	private void OnEnterDodging(SwPlayerState lastState){}
	private void OnExitDodging(SwPlayerState lastState){}
	private void OnTickDodging(float dt){}
}
