using Godot;
using SW.Src.Entity;

namespace SW.Src.Actor.Knight.Component;

public class SwKnightAudio
{
    private readonly SwKnight Parent;
    private readonly AudioStreamPlayer2D WalkSound;
    private readonly AudioStreamPlayer2D DeathSound;
    private readonly AudioStreamPlayer2D HitSound;
    private readonly SwMultiSound AttackSounds;
    public SwKnightAudio(SwKnight parent)
    {
        Parent = parent;
        WalkSound = Parent.GetNode<AudioStreamPlayer2D>("Audio/WalkSound");
        DeathSound = Parent.GetNode<AudioStreamPlayer2D>("Audio/DeathSound");
        HitSound = Parent.GetNode<AudioStreamPlayer2D>("Audio/HitSound");
        AttackSounds = Parent.GetNode<SwMultiSound>("Audio/AttackSounds");
    }
    public void Poll()
    {
        if(Parent.IsMoving() && !WalkSound.Playing) WalkSound.Play();
        else if(!Parent.IsMoving() && WalkSound.Playing) WalkSound.Stop();
    }
    public void PlayAttackSound(){AttackSounds.PlayRandom();}
    public void PlayDeathSound(){DeathSound.Play();}
    public void PlayHitSound(){HitSound.Play();}
}
