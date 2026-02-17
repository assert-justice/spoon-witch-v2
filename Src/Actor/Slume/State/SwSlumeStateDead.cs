using SW.Src.Global;
using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateDead(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Dead)
{
    public override void EnterState(SwState lastState)
    {
        Parent.Animator.PlayBodyAnim("death");
    }
    public override void ExitState(SwState lastState)
    {
    }
    public override void Tick(float dt)
    {
    }
}
