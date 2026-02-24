using Godot;

namespace SW.Src.Actor.Slume.Component;

public class SwSlumeAnimator
{
    private readonly SwSlume Parent;
    private readonly AnimatedSprite2D Sprite;
    private readonly string[] Facing = ["right", "down", "left", "up"];
	private int FacingIdx = 0;
    public SwSlumeAnimator(SwSlume parent)
    {
        Parent = parent;
        Sprite = parent.GetNode<AnimatedSprite2D>("Sprite");
    }
    private string GetFacing()
	{
        string dir = Facing[Parent.GetLastFacing4()];
		if(dir == "left")
		{
			dir = "right";
			Sprite.FlipH = true;
		}
		else
		{
			Sprite.FlipH = false;
		}
		return dir;
	}
    public void PlayBodyAnim(string animName)
    {
        Sprite.Play(animName);
    }
    public void PlayBodyAnimFaced(string animName)
    {
        PlayBodyAnim(animName + "_" + GetFacing());
    }
    public bool IsPlaying(){return Sprite.IsPlaying();}
}
