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
    public void Poll()
    {
        int facingIdx = Mathf.RoundToInt(Parent.GetLastVelocity().Angle() / (Mathf.Pi * 0.5f));
		if (facingIdx < 0) facingIdx += 4;
		FacingIdx = facingIdx;
    }
    private string GetFacing()
	{
		int facingIdx = FacingIdx;
		// handle image flipping
		if(facingIdx == 2)
		{
			facingIdx = 0;
			Sprite.FlipH = true;
		}
		else
		{
			Sprite.FlipH = false;
		}
		return Facing[facingIdx];
	}
    public void PlayBodyAnim(string animName)
    {
        Sprite.Play(animName);
    }
    public void PlayBodyAnimFaced(string animName)
    {
        PlayBodyAnim(animName + "_" + GetFacing());
    }
}
