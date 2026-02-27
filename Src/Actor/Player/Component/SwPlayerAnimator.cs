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
    private string GetFacing()
	{
        string dir = Facing[Parent.GetLastFacing4()];
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
        PlayBodyAnim(animName + "_" + GetFacing());
    }
    public void PlayBodyAnimFaced(string animName, int hands)
    {
        PlayBodyAnimFaced($"{animName}{hands}");
    }
    public void PlayBodyAnimDefault(int hands)
    {
        string animName = Parent.Controls.IsMoving() ? "run" : "idle";
        PlayBodyAnimFaced($"{animName}{hands}");
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
}
