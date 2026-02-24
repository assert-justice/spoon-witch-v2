using Godot;
using SW.Src.Timer;
using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateWandering(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Wandering)
{
    private Vector2 TargetPoint;
    private readonly SwClock MoveTimeout = new(new(){Duration=parent.GiveUpTime});
    private void SetRandomTargetPoint()
    {
        float distance = GD.Randf() * (Parent.MaxWanderDistance - Parent.MinWanderDistance) + Parent.MinWanderDistance;
        float angle = GD.Randf() * Mathf.Tau;
        TargetPoint = Vector2.FromAngle(angle) * distance + Parent.Position;
        MoveTimeout.Restart();
    }
    private bool ShouldReset(Vector2 distance)
    {
        if(!Parent.IsMoving() || !MoveTimeout.IsRunning()) return true;
        return distance.LengthSquared() < 64;
    }
    public override void EnterState(SwState lastState)
    {
        SetRandomTargetPoint();
    }
    public override void ExitState(SwState lastState)
    {
    }
    public override void Tick(float dt)
    {
        Parent.Animator.PlayDefault();
        MoveTimeout.Tick(dt);
        Vector2 distance = TargetPoint - Parent.Position;
        if(ShouldReset(distance)) SetRandomTargetPoint();
        Parent.Velocity = (TargetPoint - Parent.Position).Normalized() * Parent.Speed;
    }
}
