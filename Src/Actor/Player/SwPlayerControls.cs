using System;
using SW.Src.Global;
using SW.Src.Input;
using SW.Src.Timer;

namespace SW.Src.Actor.Player;

public class SwPlayerControls
{
    private SwInputManager InputManager;
    private SwInputBuffer InputBuffer;
    private SwClock InputLockout;
    private Action<SwPlayer.SwState> QueueState;
    private float IdleTime = 0;
    private enum SwAction
	{
		SpoonAttack,
		ChargeSling,
		Dodge,
		UseItem,
		Heal,
	}
    public bool SpoonAttack{get=>InputBuffer.TryGetValue(4);}
    public bool ChargeSling{get=>InputBuffer.TryGetValue(3);}
    public bool Dodge{get=>InputBuffer.TryGetValue(2);}
    public bool UseItem{get=>InputBuffer.TryGetValue(0);}
    public bool Heal{get=>InputBuffer.TryGetValue(1);}
    public SwPlayerControls(Action<SwPlayer.SwState> queueState, SwInputManager inputManager, SwClock inputLockout)
    {
        QueueState = queueState;
        InputLockout = inputLockout;
        InputManager = inputManager;
        // Names listed in reverse order of priority
        InputBuffer = new(["UseItem","Heal","Dodge","ChargeSling","SpoonAttack"]);
    }
    public void Poll(float dt)
    {
        InputBuffer.Poll();
        if(InputLockout.IsRunning()) return;
        bool idle = false;
        if(SpoonAttack) QueueState(SwPlayer.SwState.Attacking);
        else if(ChargeSling) QueueState(SwPlayer.SwState.Charging);
        else if(Dodge) QueueState(SwPlayer.SwState.Dodging);
        else if(Heal || UseItem) QueueState(SwPlayer.SwState.UsingItem);
        else if(InputManager.Move.GetValue().LengthSquared() > SwConstants.EPSILON) QueueState(SwPlayer.SwState.Running);
        else idle = true;
        if(idle) IdleTime += dt;
        else IdleTime = 0;
    }
    public float GetIdleTime(){return IdleTime;}
}
