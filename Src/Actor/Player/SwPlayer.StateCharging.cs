namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
	private void BindStateCharging()
	{
		BindState(new()
        {
            State = SwPlayerState.Charging, 
            OnEnterState = OnEnterCharging, 
            OnExitState = OnExitCharging, 
            OnTick = OnTickCharging
        });
	}
	private void OnEnterCharging(SwPlayerState lastState)
    {
        //
    }
	private void OnExitCharging(SwPlayerState lastState)
    {
        //
    }
	private void OnTickCharging(float dt)
    {
        //
    }
}
