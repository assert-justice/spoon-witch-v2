using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Knight.SwKnight;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateSeeking : SwStateMachine<SwKnight, SwState>.SwStateData
{
    //
    private readonly SwClock GiveUpClock;
    public SwKnightStateSeeking(SwKnight parent) : base(parent, SwState.Seeking)
    {
        GiveUpClock = new(new(){Duration=parent.GiveUpTime});
    }
    public override void EnterState(SwState lastState)
    {
        GiveUpClock.Restart();
    }
    public override void Tick(float dt)
    {
        Parent.Animator.PlayBodyDefault();
        if(Parent.CanSeePlayer()) Parent.StateMachine.QueueState(SwState.Chasing);
        else if(Parent.IsTargetPointInRadius(Parent.CloseEnough)) Parent.StateMachine.QueueState(SwState.Wandering);
        else if(!GiveUpClock.IsRunning()) Parent.StateMachine.QueueState(SwState.Wandering);
        else
        {
            Parent.Velocity = Parent.DirectionToTargetPoint() * Parent.Speed;
        }
    }
}
