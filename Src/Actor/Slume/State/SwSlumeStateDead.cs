using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateDead(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Dead)
{
    public override void EnterState(SwState lastState)
    {
        Parent.Animator.PlayBodyAnim("death");
        Parent.Hurtbox.IsEnabled = false;
        Parent.Hitbox.Disabled = true;
    }
    public override void Tick(float dt)
    {
        if(Parent.Animator.IsPlaying()) return;
        if(Parent.InSleepRadius()) return;
        Parent.Cleanup();
    }
}
