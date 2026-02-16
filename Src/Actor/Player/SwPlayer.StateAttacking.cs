using Godot;

namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwActor
{
    private void BindStateAttacking()
    {
        StateMachine.AddState(new(SwState.Attacking, OnEnterAttacking, OnExitAttacking, OnTickAttacking));
    }
    private void OnEnterAttacking(SwState lastState)
    {
        BodySprite.Play("idle0_" + GetFacing());
        IdleTime.SetDuration(0.5f);
        SpoonSprite.Visible = true;
        SpoonSprite.Rotation = GetLastVelocity().Angle() - Mathf.Pi * 0.5f;
        SpoonSprite.Play();
        // Todo: enable and place hurtbox
    }
    private void OnExitAttacking(SwState nextState)
    {
        SpoonSprite.Visible = false;
        // Todo: disable hurtbox
    }
    private void OnTickAttacking(float dt){}
}