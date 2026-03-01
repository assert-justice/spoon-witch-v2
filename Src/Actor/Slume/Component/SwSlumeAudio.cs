using Godot;
using SW.Src.Entity;

namespace SW.Src.Actor.Slume.Component;

public class SwSlumeAudio
{
    private readonly SwSlume Parent;
    private readonly AudioStreamPlayer2D WalkSound;
    private readonly AudioStreamPlayer2D DeathSound;
    private readonly AudioStreamPlayer2D HitSound;
    private readonly SwMultiSound AttackSounds;
    public SwSlumeAudio(SwSlume parent)
    {
        Parent = parent;
        WalkSound = Parent.GetNode<AudioStreamPlayer2D>("Audio/WalkSound");
        DeathSound = Parent.GetNode<AudioStreamPlayer2D>("Audio/DeathSound");
        HitSound = Parent.GetNode<AudioStreamPlayer2D>("Audio/HitSound");
        AttackSounds = Parent.GetNode<SwMultiSound>("Audio/AttackSounds");
    }
    public void Tick(float dt)
    {
        if(Parent.StateMachine.IsInState(SwSlume.SwState.KnockedBack))WalkSound.Stop();
        else if(Parent.IsMoving() && !WalkSound.Playing) WalkSound.Play();
        else if(!Parent.IsMoving() && WalkSound.Playing) WalkSound.Stop();
    }
    public void PlayDeathSound()
    {
        DeathSound.Play();
    }
    public void PlayHitSound()
    {
        HitSound.Play();
    }
    public void PlayAttackSound()
    {
        AttackSounds.PlayRandom();
    }
}
