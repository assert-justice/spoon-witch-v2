namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
	private void BindStateUsingItem()
	{
		BindState(new()
        {
            State = SwPlayerState.UsingItem, 
            OnEnterState = OnEnterUsingItem, 
            OnExitState = OnExitUsingItem, 
            OnTick = OnTickUsingItem
        });
	}
	private void OnEnterUsingItem(SwPlayerState lastState){}
	private void OnExitUsingItem(SwPlayerState lastState){}
	private void OnTickUsingItem(float dt){}
}
