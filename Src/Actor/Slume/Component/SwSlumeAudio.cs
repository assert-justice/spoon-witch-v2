using Godot;
using SW.Src.Entity;

namespace SW.Src.Actor.Slume.Component;

public class SwSlumeAudio
{
    private readonly SwSlume Parent;
    private readonly SwMultiSound WalkSound;
    private readonly SwMultiSound DeathSound;
    private readonly SwMultiSound HitSound;
    private readonly SwMultiSound AttackSounds;
    private bool WasPlayerVisible = false;
    public SwSlumeAudio(SwSlume parent)
    {
        Parent = parent;
        WalkSound = Parent.GetNode<SwMultiSound>("Audio/WalkSound");
        DeathSound = Parent.GetNode<SwMultiSound>("Audio/DeathSound");
        HitSound = Parent.GetNode<SwMultiSound>("Audio/HitSound");
        AttackSounds = Parent.GetNode<SwMultiSound>("Audio/AttackSounds");
    }
    private void SetVolume(float volume)
    {
        WalkSound.SwSetVolumeDb(volume);
        DeathSound.SwSetVolumeDb(volume);
        HitSound.SwSetVolumeDb(volume);
        AttackSounds.SwSetVolumeDb(volume);
    }
    public void Tick(float dt)
    {
        if(Parent.StateMachine.IsInState(SwSlume.SwState.KnockedBack))WalkSound.Stop();
        else if(Parent.IsMoving() && !WalkSound.Playing) WalkSound.Play();
        else if(!Parent.IsMoving() && WalkSound.Playing) WalkSound.Stop();
        bool playerVisible = Parent.CanSeePlayer();
        if(playerVisible && !WasPlayerVisible) SetVolume(0);
        else if(!playerVisible && WasPlayerVisible) SetVolume(-5);
        WasPlayerVisible = playerVisible;
    }
    public void PlayDeathSound()
    {
        DeathSound.Play(0);
    }
    public void PlayHitSound()
    {
        HitSound.Play(0);
    }
    public void PlayAttackSound()
    {
        AttackSounds.PlayRandom();
    }
}
