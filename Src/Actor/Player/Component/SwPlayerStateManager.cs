using SW.Src.Actor.Player.State;
using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerStateManager
{
    private readonly SwPlayer Parent;
    private SwStateMachine<SwPlayer, SwState> StateMachine;
	// When the player performs an action, they enter a state and cannot intentionally leave it for some time.
	// This is the "recovery". So when you make an attack, this is the time until you can perform another action, like attacking again, or drinking a potion, or whatever.
	// This time is not tied directly to animations or anything else. If a recovery time is less than the duration of an animation, the player can cancel that animation at a certain point.
	private readonly SwClock RecoveryClock;
	// This is the time after the player recovers from an action but before they are returned to the default state automatically.
	private readonly SwClock DefaultClock;
    public SwPlayerStateManager(SwPlayer parent)
    {
        Parent = parent;
        StateMachine = new(SwState.Default);
		StateMachine.AddState(new SwPlayerStateDefault(Parent));
		StateMachine.AddState(new SwPlayerStateAttacking(Parent));
		StateMachine.AddState(new SwPlayerStateCharging(Parent));
		StateMachine.AddState(new SwPlayerStateDodging(Parent));
		StateMachine.AddState(new SwPlayerStateInSubmenu(Parent));
		StateMachine.AddState(new SwPlayerStateUsingItem(Parent));
		RecoveryClock = new(new()
		{
			Duration = 0, 
			IsPaused = true, 
			OnFinish = ()=>DefaultClock.Restart(),
		});
		DefaultClock = new(new()
		{
			Duration = 0, 
			IsPaused = true, 
			OnFinish = ()=>StateMachine.QueueState(SwState.Default),
		});
    }
	private void AutoStateChange()
	{
		if(Parent.Controls.JustAttacked()) Parent.StateManager.QueueState(SwState.Attacking);
		else if(Parent.Controls.JustCharged()) Parent.StateManager.QueueState(SwState.Charging);
	}
    public void Tick(float dt)
    {
        RecoveryClock.Tick(dt);
        DefaultClock.Tick(dt);
		if(!IsRecovering()) AutoStateChange();
        StateMachine.Tick(dt);
    }
    public void QueueState(SwState state)
    {
        StateMachine.QueueState(state);
    }
    public void SetLockout(float recoveryTime, float defaultTime = 0)
	{
		RecoveryClock.SetDuration(recoveryTime);
		DefaultClock.SetDuration(defaultTime, true);
	}
    public bool IsRecovering()
	{
		return RecoveryClock.IsRunning();
	}

}