using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateChasing(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Chasing)
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
        if (Parent.CanSeePlayer(out var player))
        {
            Parent.TargetPoint = player.Position;
            Parent.Velocity = Parent.DirectionToPlayer() * Parent.Speed;
        }
        else
        {
            Parent.StateMachine.QueueState(SwState.Seeking);
        }
    }
}
