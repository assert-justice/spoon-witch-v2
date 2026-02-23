using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateDefault(SwPlayer parent) :
    SwStateMachine<SwPlayer, SwState>.SwStateData(parent, SwState.Default)
{
    // public override void EnterState(SwState lastState){}
    // public override void ExitState(SwState lastState){}
    public override void Tick(float dt)
    {
        Parent.Velocity = Parent.Controls.Move() * Parent.Speed;
        Parent.Animator.PlayBodyAnimDefault(2);
    }
}
