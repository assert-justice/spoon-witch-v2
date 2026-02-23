using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateSlingCharged(SwPlayer parent) :
    SwStateMachine<SwPlayer, SwState>.SwStateData(parent, SwState.SlingCharged)
{
    public override void EnterState(SwState lastState)
    {
        Parent.Animator.PlaySlingAnim();
    }
    public override void ExitState(SwState lastState)
    {
        Parent.Animator.HideSling();
    }
    public override void Tick(float dt)
    {
        Parent.Velocity = Parent.Controls.Move() * Parent.Speed * Parent.SlingMovementSpeedMul;
        Parent.Animator.PlayBodyAnimDefault(1);
        if (Parent.Controls.JustAttacked())
        {
            // Fire!
            Parent.Evoker.FireSling();
            Parent.StateManager.QueueState(SwState.Default);
        }
        else if (Parent.Controls.IsChargingJustReleased())
        {
            // Cancel attack
            Parent.StateManager.QueueState(SwState.Default);
        }
        else if(Parent.Controls.IsCharging()) Parent.StateManager.SetLockout(0.1f, 0.1f);
    }
}
