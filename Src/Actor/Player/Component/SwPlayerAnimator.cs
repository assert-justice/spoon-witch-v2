using Godot;
using SW.Src.Utils;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerAnimator: ISwPoll
{
    private readonly SwPlayer Parent;
    private readonly AnimatedSprite2D BodySprite;
	private readonly AnimatedSprite2D SpoonSprite;
	private readonly AnimatedSprite2D SlingSprite;
    private readonly string[] Facing = ["right", "down", "left", "up"];
	private SwDelta<int> FacingIdx = new();
    public SwPlayerAnimator(SwPlayer parent)
    {
        Parent = parent;
        BodySprite = parent.GetNode<AnimatedSprite2D>("BodySprite");
        SpoonSprite = parent.GetNode<AnimatedSprite2D>("SpoonSprite");
        SlingSprite = parent.GetNode<AnimatedSprite2D>("SlingSprite");
    }
    public void Poll()
    {
        int facingIdx = Mathf.RoundToInt(Parent.GetLastVelocity().Angle() / (Mathf.Pi * 0.5f));
		if (facingIdx < 0) facingIdx += 4;
		FacingIdx.Value = facingIdx;
    }
    private string GetFacing()
	{
		int facingIdx = FacingIdx.Value;
		// handle image flipping
		if(facingIdx == 2)
		{
			facingIdx = 0;
			BodySprite.FlipH = true;
		}
		else
		{
			BodySprite.FlipH = false;
		}
		return Facing[facingIdx];
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
	public void PlaySpoonAnim()
	{
		SpoonSprite.Visible = true;
        SpoonSprite.Rotation = Parent.GetLastVelocity().Angle() - Mathf.Pi * 0.5f;
        // Restart the animation if it is already playing
        if(SpoonSprite.IsPlaying()) SpoonSprite.Stop();
        SpoonSprite.Play();
	}
    public void HideSpoon()
    {
        SpoonSprite.Visible = false;
    }
    public void PlaySlingAnim()
    {
        SlingSprite.Visible = true;
        SlingSprite.Play();
    }
        public void HideSling()
    {
        SlingSprite.Visible = false;
    }
}
