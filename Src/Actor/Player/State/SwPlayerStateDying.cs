using SW.Src.Utils;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateDying : SwStateMachine<SwPlayer, SwPlayer.SwState>.SwStateData
{
    //
    public SwPlayerStateDying(SwPlayer parent) : base(parent, SwPlayer.SwState.Dying){}
    public override void EnterState(SwPlayer.SwState lastState)
    {
        // Disable stuff
        Parent.AudioManager.PlayDeathSound();
    }
    public override void Tick(float dt)
    {
        if(!Parent.AudioManager.IsDeathSoundPlaying()) Parent.IsDeadDead = true;
    }
}
