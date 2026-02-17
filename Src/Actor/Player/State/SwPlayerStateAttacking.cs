using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateAttacking(SwPlayer parent) : 
    SwStateMachine<SwPlayer, SwState>.SwStateData(parent, SwState.Attacking)
{
    public override void EnterState(SwState lastState)
    {
        // Parent.Animator.PlayBodyAnimFaced(Parent.Controls.IsMoving() ? "run" : "idle", 0);
        Parent.Animator.PlaySpoonAnim();
        Parent.StateManager.SetLockout(0.25f, 0.25f);
        // Todo: enable and place hurtbox
        // Todo: finish implementation
        // We can use the animator here
    }
    public override void ExitState(SwState lastState)
    {
        Parent.Animator.HideSpoon();
    }
    public override void Tick(float dt)
    {
        Parent.Velocity = Parent.Controls.Move() * Parent.Speed;
        Parent.Animator.PlayBodyAnimFaced(Parent.Controls.IsMoving() ? "run" : "idle", 0);
    }
}
