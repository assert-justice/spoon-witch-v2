using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateAttacking(SwPlayer parent) : 
    SwStateMachine<SwPlayer, SwState>.SwStateData(parent, SwState.Attacking)
{
    public override void EnterState(SwState lastState)
    {
        Parent.Evoker.StartSpoonAttack();
        Parent.Animator.PlaySpoonAnim();
        Parent.StateManager.SetLockout(0.25f, 0.25f);
    }
    public override void ExitState(SwState lastState)
    {
        Parent.Animator.HideSpoon();
        Parent.Evoker.EndSpoonAttack();
    }
    public override void Tick(float dt)
    {
        Parent.Velocity = Parent.Controls.Move() * Parent.Speed;
        Parent.Animator.PlayBodyAnimDefault(0);
    }
}
