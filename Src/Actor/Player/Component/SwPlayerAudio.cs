using Godot;
using SW.Src.Entity;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerAudio
{
    private readonly SwPlayer Parent;
    private readonly SwMultiSound WalkSound;
    private readonly SwMultiSound DeathSound;
    private readonly SwMultiSound SpoonSounds;
    private readonly SwMultiSound HitSounds;
    private readonly SwMultiSound SlingSounds;
    private readonly SwMultiSound SlingFireSound;
    public SwPlayerAudio(SwPlayer parent)
    {
        Parent = parent;
        WalkSound = Parent.GetNode<SwMultiSound>("Audio/WalkSound");
        DeathSound = Parent.GetNode<SwMultiSound>("Audio/DeathSound");
        SpoonSounds = Parent.GetNode<SwMultiSound>("Audio/SpoonSounds");
        HitSounds = Parent.GetNode<SwMultiSound>("Audio/HitSounds");
        SlingSounds = Parent.GetNode<SwMultiSound>("Audio/SlingSounds");
        SlingFireSound = Parent.GetNode<SwMultiSound>("Audio/SlingFireSound");
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
    public void StopSlingSound()
    {
        SlingSounds.Stop();
    }
    public void PlaySlingFireSound()
    {
        SlingFireSound.Play(0);
    }
    public void PlayDeathSound()
    {
        DeathSound.Play(0);
    }
    public bool IsDeathSoundPlaying()
    {
        return DeathSound.Playing;
    }
}
