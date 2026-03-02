using SW.Src.Utils;
using static SW.Src.Actor.Knight.SwKnight;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateChasing : SwStateMachine<SwKnight, SwState>.SwStateData
{
    //
    public SwKnightStateChasing(SwKnight parent) : base(parent, SwState.Chasing){}
    public override void EnterState(SwState lastState){}
    public override void ExitState(SwState lastState){}
    public override void Tick(float dt)
    {
        Parent.Animator.PlayBodyDefault();
        if (Parent.CanSeePlayer(out var player))
        {
            if(Parent.IsPlayerInRadius(Parent.AttackRange)) Parent.StateMachine.QueueState(SwState.Attacking);
            Parent.TargetPoint = player.Position;
            Parent.Velocity = Parent.DirectionToPlayer() * Parent.Speed;
        }
        else
        {
            Parent.StateMachine.QueueState(SwState.Seeking);
        }
    }
}
