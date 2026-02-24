using Godot;
using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateWandering(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Wandering)
{
    private void SetRandomTargetPoint()
    {
        float distance = GD.Randf() * (Parent.MaxWanderDistance - Parent.MinWanderDistance) + Parent.MinWanderDistance;
        float angle = GD.Randf() * Mathf.Tau;
        Parent.TargetPoint = Vector2.FromAngle(angle) * distance + Parent.Position;
    }
    private bool ShouldReset()
    {
        // Todo: It thinks the velocity is always speed, even when it's colliding with things. Fix this
        if(Parent.GetLastSlideCollision() is not null) return true;
        return Parent.IsTargetPointInRadius(Parent.CloseEnough);
    }
    public override void EnterState(SwState lastState)
    {
        SetRandomTargetPoint();
    }
    public override void Tick(float dt)
    {
        Parent.Animator.PlayDefault();
        if(Parent.ShouldFlee()) Parent.StateMachine.QueueState(SwState.Fleeing);
        else if(Parent.CanSeePlayer()) Parent.StateMachine.QueueState(SwState.Chasing);
        if(ShouldReset()) SetRandomTargetPoint();
        Parent.Velocity = Parent.DirectionToTargetPoint() * Parent.Speed * Parent.WanderSpeedMul;
    }
}
