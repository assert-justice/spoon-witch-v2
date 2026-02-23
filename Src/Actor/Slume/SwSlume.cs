using Godot;
using SW.Src.Actor.Slume.Component;
using SW.Src.Actor.Slume.State;
using SW.Src.Utils;

namespace SW.Src.Actor.Slume;

public partial class SwSlume : SwActor
{
    [Export] public float MaxHealth = 100;
    [Export] public float Speed = 100;
    [Export] public float DeathDelay = 1;
    public enum SwState
    {
        Default,
        Dead,
    }
    private SwStateMachine<SwSlume, SwState> StateMachine;
    public SwSlumeAnimator Animator{get; private set;}
    public override void _Ready()
    {
        Animator = new(this);
        StateMachine = new(SwState.Default);
        StateMachine.AddState(new SwSlumeStateDefault(this));
        StateMachine.AddState(new SwSlumeStateDead(this));
        base._Ready();
    }
    public override void _PhysicsProcess(double delta)
    {
        float left = Godot.Input.IsPhysicalKeyPressed(Key.A)?-1:0;
        float right = Godot.Input.IsPhysicalKeyPressed(Key.D)?1:0;
        float up = Godot.Input.IsPhysicalKeyPressed(Key.W)?-1:0;
        float down = Godot.Input.IsPhysicalKeyPressed(Key.S)?1:0;
        if(Godot.Input.IsMouseButtonPressed(MouseButton.Right)) Die();

        Velocity = new(left + right, up + down);

        float dt = (float)delta;
        StateMachine.Tick(dt);
        Animator.Poll();
        base._PhysicsProcess(delta);
    }
    protected override float GetMaxHealth()
    {
        return MaxHealth;
    }
    protected override void Die()
    {
        StateMachine.QueueState(SwState.Dead);
    }
}
