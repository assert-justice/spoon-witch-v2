namespace SW.Src.Actor.Player;

public enum SwPlayerState
{
    Default,
    // Idle,
    // Running,
    Attacking,
    Dodging,
    Charging,
    UsingItem,
    InSubmenu,
}

/*
namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerAbstract
{
	private void BindState[StateName]()
	{
		BindState(new()
        {
            State = SwPlayerState.[StateName], 
            OnEnterState = OnEnter[StateName], 
            OnExitState = OnExit[StateName], 
            OnTick = OnTick[StateName]
        });
	}
	private void OnEnter[StateName](SwPlayerState lastState){}
	private void OnExit[StateName](SwPlayerState lastState){}
	private void OnTick[StateName](float dt){}
}

*/
