using Godot;
using SW.Src.Actor.Player;
using SW.Src.Timer;

namespace SW.Src.Actor;
public abstract partial class SwEnemy : SwActor
{
    [Export] private float SleepRadius = 10000;
    [Export] private float SleepTickDelay = 10;
    private float SleepRadiusSquared;
    private SwPlayer Player;
    private float TimeScale = 1;
    private SwClock SleepClock;
    public override void _Ready()
    {
        SleepRadiusSquared = SleepRadius * SleepRadius;
        SleepClock = new(new(){});
        if(!IsAwake) SleepClock.GetTime();
        base._Ready();
    }
    public override void _PhysicsProcess(double delta)
    {
        HandleSleep((float)delta);
        base._PhysicsProcess(delta);
    }
    private void HandleSleep(float dt)
    {
        if (SleepClock.IsRunning())
        {
            SleepClock.Tick(dt);
        }
        else if(IsAwake && !InSleepRadius())
        {
            IsAwake = false;
            // Randomize the delay to spread out checks
            SleepClock.SetDuration(SleepTickDelay * GD.Randf());
        }
        else if(!IsAwake && InSleepRadius()) IsAwake = true;
        else
        {
            SleepClock.SetDuration(SleepTickDelay);
            SleepClock.Restart();
        }
    }
    protected bool TryGetPlayer(out SwPlayer player)
    {
        player = Player;
        if(Player is not null) return true;
        var nodes = GetTree().GetNodesInGroup("Player");
        if(nodes.Count != 1) return false;
        if(nodes[0] is not SwPlayer p) return false;
        Player = p;
        player = p;
        return true;
    }
    public float DistanceToPlayer()
    {
        if(!TryGetPlayer(out var player)) return Mathf.Inf;
        return (Position - player.Position).Length();
    }
    public float DistanceToPlayerSquared()
    {
        if(!TryGetPlayer(out var player)) return Mathf.Inf;
        return (Position - player.Position).LengthSquared();
    }
    public bool InSleepRadius()
    {
        return DistanceToPlayerSquared() < SleepRadiusSquared;
    }
}
