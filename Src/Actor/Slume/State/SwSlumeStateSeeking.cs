using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateSeeking(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Seeking)
{
    private readonly SwClock GiveUpClock = new(new(){Duration=parent.GiveUpTime});
    public override void EnterState(SwState lastState)
    {
        GiveUpClock.Restart();
    }
    public override void Tick(float dt)
    {
        Parent.Animator.PlayDefault();
        if(Parent.CanSeePlayer()) Parent.StateMachine.QueueState(SwState.Chasing);
        else if(Parent.IsTargetPointInRadius(Parent.CloseEnough)) Parent.StateMachine.QueueState(SwState.Wandering);
        else if(!GiveUpClock.IsRunning()) Parent.StateMachine.QueueState(SwState.Wandering);
        else
        {
            Parent.Velocity = Parent.DirectionToTargetPoint() * Parent.Speed;
        }
    }
}
