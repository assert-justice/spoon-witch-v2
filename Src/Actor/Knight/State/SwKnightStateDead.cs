using SW.Src.Utils;
using static SW.Src.Actor.Knight.SwKnight;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateDead : SwStateMachine<SwKnight, SwState>.SwStateData
{
    public SwKnightStateDead(SwKnight parent) : base(parent, SwState.Dead){}
    public override void EnterState(SwState lastState)
    {
        Parent.Evoker.Die();
        Parent.Animator.PlayDeathAnim();
    }
    public override void Tick(float dt)
    {
        if(Parent.InSleepRadius()) return;
        Parent.Cleanup();
    }
}
