namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
    protected override void BindStates()
	{
		BindStateDefault();
		BindStateAttacking();
		BindStateCharging();
		BindStateDodging();
		BindStateInSubmenu();
		BindStateUsingItem();
	}
}
