using Godot;

namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwPlayerApi
{
    private void BindStateAttacking()
    {
        BindState(new()
        {
            State = SwPlayerState.Attacking, 
            OnEnterState = OnEnterAttacking, 
            OnExitState = OnExitAttacking, 
            OnTick = OnTickAttacking
        });
    }
    private void OnEnterAttacking(SwPlayerState lastState)
    {
        // BodySprite.Play("idle0_" + GetFacing());
        // IdleTime.SetDuration(0.5f);
        // SpoonSprite.Visible = true;
        // SpoonSprite.Rotation = GetLastVelocity().Angle() - Mathf.Pi * 0.5f;
        // SpoonSprite.Play();
        // Todo: enable and place hurtbox
    }
    private void OnExitAttacking(SwPlayerState nextState)
    {
        // SpoonSprite.Visible = false;
        // Todo: disable hurtbox
    }
    private void OnTickAttacking(float dt){}
}
