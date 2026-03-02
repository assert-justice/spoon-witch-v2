using Godot;
using SW.Src.Entity;

namespace SW.Src.Actor.Knight.Component;

public class SwKnightAudio
{
    private readonly SwKnight Parent;
    private readonly SwMultiSound WalkSound;
    private readonly SwMultiSound DeathSound;
    private readonly SwMultiSound HitSound;
    private readonly SwMultiSound AttackSounds;
    public SwKnightAudio(SwKnight parent)
    {
        Parent = parent;
        WalkSound = Parent.GetNode<SwMultiSound>("Audio/WalkSound");
        DeathSound = Parent.GetNode<SwMultiSound>("Audio/DeathSound");
        HitSound = Parent.GetNode<SwMultiSound>("Audio/HitSound");
        AttackSounds = Parent.GetNode<SwMultiSound>("Audio/AttackSounds");
    }
    public void Poll()
    {
        if(Parent.IsMoving() && !WalkSound.Playing) WalkSound.Play();
        else if(!Parent.IsMoving() && WalkSound.Playing) WalkSound.Stop();
    }
    public void PlayAttackSound(){AttackSounds.PlayRandom();}
    public void PlayDeathSound(){DeathSound.Play(0);}
    public void PlayHitSound(){HitSound.Play(0);}
}
