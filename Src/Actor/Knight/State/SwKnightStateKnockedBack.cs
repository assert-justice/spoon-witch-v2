using Godot;
using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Knight.SwKnight;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateKnockedBack : SwStateMachine<SwKnight, SwState>.SwStateData
{
    //
    public SwKnightStateKnockedBack(SwKnight parent) : base(parent, SwState.KnockedBack){}
    private readonly SwClock KnockbackClock = new();
    private SwState LastState = SwState.Default;
    public override void EnterState(SwState lastState)
    {
        LastState = lastState == SwState.Attacking ? SwState.Wandering : lastState;
        KnockbackClock.SetDuration(Parent.KnockBackTime);
        Parent.Animator.PlayBodyAnim("knockback");
        float damageValue = 0;
        foreach (var damage in Parent.IncomingDamageQueue)
        {
            if(damage.Value > damageValue) damageValue = damage.Value;
        }
        Vector2 dir = (Parent.Position - Parent.DamageSourcePosition).Normalized();
		Parent.Velocity = dir * Parent.KnockBackBaseSpeed * damageValue;
    }
    public override void ExitState(SwState lastState)
    {
        Parent.Velocity = Vector2.Zero;
    }
    public override void Tick(float dt)
    {
        if(KnockbackClock.IsRunning()) KnockbackClock.Tick(dt);
        else if(!Parent.IsAlive()) Parent.StateMachine.QueueState(SwState.Dead);
        else if(Parent.ShouldFlee()) Parent.StateMachine.QueueState(SwState.Fleeing);
        else Parent.StateMachine.QueueState(LastState);
    }
}
