using SW.Src.Utils;
using static SW.Src.Actor.Knight.SwKnight;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateChasing : SwStateMachine<SwKnight, SwState>.SwStateData
{
    //
    public SwKnightStateChasing(SwKnight parent) : base(parent, SwState.Chasing){}
    private bool CanAttack()
    {
        return Parent.IsPlayerInRadius(Parent.AttackRange) && !Parent.AttackCooldownClock.IsRunning();
    }
    public override void EnterState(SwState lastState){}
    public override void ExitState(SwState lastState){}
    public override void Tick(float dt)
    {
        float animSpeed = 1;
        if (Parent.IsPlayerInRadius(Parent.ChargeRadius) && !Parent.ChargeRecoveryClock.IsRunning())
        {
            // Parent.ChargeRecoveryClock.Restart();
            animSpeed = Parent.ChargeSpeedMul;
        }
        Parent.Animator.PlayBodyDefault(2, animSpeed);
        if (Parent.CanSeePlayer(out var player))
        {
            if(CanAttack()) Parent.StateMachine.QueueState(SwState.Attacking);
            Parent.TargetPoint = player.Position;
            Parent.Velocity = Parent.DirectionToPlayer() * Parent.Speed * animSpeed;
        }
        else
        {
            Parent.StateMachine.QueueState(SwState.Seeking);
        }
    }
}
