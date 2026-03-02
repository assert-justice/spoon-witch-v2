using System;
using Godot;
using SW.Src.Global;

namespace SW.Src.Actor.Knight.Component;

public class SwKnightAnimator
{
    private readonly SwKnight Parent;
    private readonly AnimatedSprite2D BodySprite;
    private readonly AnimatedSprite2D SwordSprite;
    public SwKnightAnimator(SwKnight parent)
    {
        Parent = parent;
        BodySprite = Parent.GetNode<AnimatedSprite2D>("BodySprite");
        SwordSprite = Parent.GetNode<AnimatedSprite2D>("SwordPivot/SwordSprite");
    }
    public void PlayBodyDefault(int hands = 2)
    {
        if (Parent.IsMoving())
        {
            BodySprite.Play("walk" + hands.ToString());
            BodySprite.FlipH = Parent.Velocity.X < 0;
        }
        else BodySprite.Stop();
    }
    public void PlaySwordSwing()
    {
        SwordSprite.Visible = true;
        SwordSprite.Play();
    }
    public void EndSwordSwing()
    {
        SwordSprite.Visible = false;
    }
    public float GetSwordSwingDuration()
    {
        int frameCount = SwordSprite.SpriteFrames.GetFrameCount("default");
        // Todo: replace 1 here with game speed
        return frameCount * 1 / GetSwordSwingFps();
    }
    public float GetSwordSwingFps()
    {
        return (float)SwordSprite.SpriteFrames.GetAnimationSpeed("default");
    }
    public void PlayBodyAnim(string animName)
    {
        BodySprite.Play(animName);
    }
    public void PlayDeathAnim()
    {
        BodySprite.Rotation = SwConstants.HALF_PI;
    }
}
