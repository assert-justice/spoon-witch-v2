using System;
using Godot;
using SW.Src.Global;
using SW.Src.Utils;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerAnimator(SwPlayer parent)
{
    private readonly SwPlayer Parent = parent;
    private readonly AnimatedSprite2D BodySprite = parent.GetNode<AnimatedSprite2D>("BodySprite");
	private readonly AnimatedSprite2D SpoonSprite = parent.GetNode<AnimatedSprite2D>("SpoonPivot/SpoonSprite");
	private readonly AnimatedSprite2D SlingSprite = parent.GetNode<AnimatedSprite2D>("SlingSprite");
	private readonly CpuParticles2D SlingParticles = parent.GetNode<CpuParticles2D>("SlingParticles");
    private readonly CpuParticles2D HealingParticles = parent.GetNode<CpuParticles2D>("HealingParticles");
    private readonly string[] Facing = ["right", "down", "left", "up"];
    private string GetFacing(int facingIdx)
	{
        string dir = Facing[facingIdx];
		if(dir == "left")
		{
			dir = "right";
			BodySprite.FlipH = true;
		}
		else
		{
			BodySprite.FlipH = false;
		}
		return dir;
	}
    public void PlayBodyAnim(string animName)
    {
        BodySprite.Play(animName);
    }
    public void PlayBodyAnimFaced(string animName)
    {
        PlayBodyAnim(animName + "_" + GetFacing(Parent.GetLastFacing4()));
    }
    public void PlayBodyAnimFaced(string animName, int hands)
    {
        PlayBodyAnimFaced($"{animName}{hands}");
    }
    public void PlayBodyAnimFaced(string animName, int hands, int facingIdx)
    {
        PlayBodyAnim($"{animName}{hands}_{GetFacing(facingIdx)}");
    }
    public void PlayBodyAnimDefault(int hands)
    {
        string animName = Parent.Controls.IsMoving() ? "run" : "idle";
        PlayBodyAnimFaced($"{animName}{hands}");
    }
    public void PlayBodyAnimDefault(int hands, int facingIdx)
    {
        string animName = Parent.Controls.IsMoving() ? "run" : "idle";
        PlayBodyAnimFaced(animName, hands, facingIdx);
    }
    public void PlayBodyAnimAiming(int hands)
    {
        string animName = Parent.Controls.IsMoving() ? "run" : "idle";
        Vector2 aim = Parent.Controls.Aim();
        if(aim.LengthSquared() < SwConstants.EPSILON)
        {
            PlayBodyAnimDefault(hands);
            return;
        }
        int facingIdx = SwMath.RoundAngleToInt(aim.Angle(), 4);
        PlayBodyAnimFaced(animName, hands, facingIdx);
    }
	public void PlaySpoonAnim()
	{
		SpoonSprite.Visible = true;
        // Restart the animation if it is already playing
        if(SpoonSprite.IsPlaying()) SpoonSprite.Stop();
        SpoonSprite.Play();
	}
    public void HideSpoon()
    {
        SpoonSprite.Visible = false;
    }
    public void PlaySlingAnim(float speedMul = 1)
    {
        SlingSprite.Visible = true;
        SlingParticles.Emitting = speedMul == 1;
        SlingSprite.Play(null, speedMul);
    }
    public void HideSling()
    {
        SlingSprite.Visible = false;
        SlingParticles.Emitting = false;
    }
    public void PlayItemAnim()
    {
        HealingParticles.Emitting = true;
    }
    public void StopItemAnim()
    {
        HealingParticles.Emitting = false;
    }
    public void PlayDeathAnim()
    {
        BodySprite.Rotation = SwConstants.HALF_PI;
    }
}
