using Godot;
using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateSlingCharging : SwStateMachine<SwPlayer, SwState>.SwStateData
{
    private readonly SwClock ChargeClock;
    public SwPlayerStateSlingCharging(SwPlayer parent) : base(parent, SwState.SlingCharging)
    {
        ChargeClock = new(new(){Duration = parent.SlingChargeTime, OnFinish = this.OnCharge});
    }
    public override void EnterState(SwState lastState)
    {
        ChargeClock.Restart();
    }
    public override void ExitState(SwState lastState)
    {
        Parent.Animator.HideSling();
    }
    public override void Tick(float dt)
    {
        Parent.Velocity = Parent.Controls.Move() * Parent.Speed * Parent.SlingMovementSpeedMul;
        Parent.Animator.PlayBodyAnimDefault(1);
        if (Parent.Controls.IsChargingJustReleased())
        {
            // Cancel charge
            Parent.StateManager.QueueState(SwState.Default);
        }
        else if(Parent.Controls.IsCharging()) Parent.StateManager.SetLockout(0.1f, 0.1f);
        ChargeClock.Tick(dt);
        Parent.Animator.PlaySlingAnim(ChargeClock.GetProgress() * 0.5f + 0.5f);
    }
    private void OnCharge()
    {
        Parent.StateManager.QueueState(SwState.SlingCharged);
    }
}
