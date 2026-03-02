using SW.Src.Utils;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateFleeing : SwStateMachine<SwKnight, SwKnight.SwState>.SwStateData
{
    //
    public SwKnightStateFleeing(SwKnight parent) : base(parent, SwKnight.SwState.Fleeing){}
    public override void EnterState(SwKnight.SwState lastState){}
    public override void ExitState(SwKnight.SwState lastState){}
    public override void Tick(float dt)
    {
        Parent.Animator.PlayBodyDefault();
        if(!Parent.CanSeePlayer()) Parent.StateMachine.QueueState(SwKnight.SwState.Wandering);
        else Parent.Velocity = Parent.DirectionToPlayer() * - Parent.Speed;
    }
}
