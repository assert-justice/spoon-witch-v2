using Godot;
using SW.Src.Entity;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerAudio
{
    private readonly SwPlayer Parent;
    private readonly AudioStreamPlayer2D WalkSound;
    private readonly SwMultiSound SpoonSounds;
    private readonly SwMultiSound HitSounds;
    private readonly SwMultiSound SlingSounds;
    public SwPlayerAudio(SwPlayer parent)
    {
        Parent = parent;
        WalkSound = Parent.GetNode<AudioStreamPlayer2D>("Audio/WalkSound");
        SpoonSounds = Parent.GetNode<SwMultiSound>("Audio/SpoonSounds");
        HitSounds = Parent.GetNode<SwMultiSound>("Audio/HitSounds");
        SlingSounds = Parent.GetNode<SwMultiSound>("Audio/SlingSounds");
    }
    public void Tick(float dt)
    {
        if(Parent.IsMoving() && !WalkSound.Playing) WalkSound.Play();
        else if(!Parent.IsMoving() && WalkSound.Playing) WalkSound.Stop();
    }
    public void PlaySpoonSound()
    {
        SpoonSounds.PlayRandom();
    }
    public void PlayHitSound()
    {
        if(HitSounds.Playing) return;
        HitSounds.PlayShuffled();
    }
    public void PlaySlingChargingSound()
    {
        SlingSounds.Play(0);
    }
    public void PlaySlingChargedSound()
    {
        SlingSounds.Play(1);
    }
    public void PlaySlingFireSound()
    {
        SlingSounds.Play(2);
    }
}
