using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateUsingItem(SwPlayer parent) :
    SwStateMachine<SwPlayer, SwState>.SwStateData(parent, SwState.UsingItem)
{
    public override void EnterState(SwState lastState)
    {
        Parent.StateManager.SetLockout(Parent.HealTime);
        Parent.Animator.PlayItemAnim();
    }
    public override void ExitState(SwState lastState)
    {
        Parent.Animator.StopItemAnim();
    }
    public override void Tick(float dt)
    {
        Parent.Animator.PlayBodyAnimDefault(1);
        Parent.Velocity = Parent.Controls.Move() * Parent.Speed * Parent.HealMovementSpeedMul;
    }
}
