using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateCharging(SwPlayer parent) :
    SwStateMachine<SwPlayer, SwState>.SwStateData(parent, SwState.Charging)
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
        Parent.Velocity = Parent.Controls.Move() * Parent.Speed * Parent.ChargeSpeedMul;
        Parent.Animator.PlayBodyAnimFaced(Parent.Controls.IsMoving() ? "run" : "idle", 1);
        if(Parent.Controls.IsCharging()) Parent.StateManager.SetLockout(0.1f, 0.1f);
    }
}
