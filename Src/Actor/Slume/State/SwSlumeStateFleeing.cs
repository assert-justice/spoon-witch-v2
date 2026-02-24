using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateFleeing(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Fleeing)
{
    public override void EnterState(SwState lastState)
    {
    }
    public override void ExitState(SwState lastState)
    {
    }
    public override void Tick(float dt)
    {
        Parent.Animator.PlayDefault();
        if(!Parent.CanSeePlayer()) Parent.StateMachine.QueueState(SwState.Wandering);
        else Parent.Velocity = Parent.DirectionToPlayer() * - Parent.Speed;
    }
}
