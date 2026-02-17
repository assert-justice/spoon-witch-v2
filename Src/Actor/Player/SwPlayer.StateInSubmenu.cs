namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
	private void BindStateInSubmenu()
	{
		BindState(new()
        {
            State = SwPlayerState.InSubmenu, 
            OnEnterState = OnEnterInSubmenu, 
            OnExitState = OnExitInSubmenu, 
            OnTick = OnTickInSubmenu
        });
	}
	private void OnEnterInSubmenu(SwPlayerState lastState)
    {
        // Player controls are not used or updated while in a submenu
    }
	private void OnExitInSubmenu(SwPlayerState lastState){}
	private void OnTickInSubmenu(float dt)
    {
    }
}
