using Godot;
using SW.Src.Global;
using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateKnockedBack(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.KnockedBack)
{
    private readonly SwClock KnockbackClock = new();
    public override void EnterState(SwState lastState)
    {
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
        else Parent.StateMachine.QueueState(Parent.IsAlive() ? SwState.Default : SwState.Dead);
    }
}
