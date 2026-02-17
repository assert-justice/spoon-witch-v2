using SW.Src.Global;
using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateDefault(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Default)
{
    public override void EnterState(SwState lastState)
    {
    }
    public override void ExitState(SwState lastState)
    {
    }
    public override void Tick(float dt)
    {
        Parent.Animator.PlayBodyAnimFaced(Parent.Velocity.LengthSquared() > SwConstants.EPSILON ? "move" : "idle");
    }
}
