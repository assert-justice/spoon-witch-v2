using Godot;
using SW.Src.Utils;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateDefault : SwStateMachine<SwKnight, SwKnight.SwState>.SwStateData
{
    public SwKnightStateDefault(SwKnight parent) : base(parent, SwKnight.SwState.Default){}
    public override void EnterState(SwKnight.SwState lastState){Parent.Velocity = Vector2.Zero;}
    public override void ExitState(SwKnight.SwState lastState){}
    public override void Tick(float dt){Parent.Animator.PlayBodyDefault();}
}
