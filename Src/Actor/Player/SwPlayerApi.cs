using Godot;
using SW.Src.Global;
using SW.Src.Input;
using SW.Src.Timer;
using SW.Src.Utils;

namespace SW.Src.Actor.Player;

public abstract partial class SwPlayerApi : SwActor
{
	// Exports
    [Export] protected float Speed = 300;
	[Export] protected float MaxHealth = 100;
	[Export] protected float DeathDelay = 1;
	// State management
	private SwStateMachine<SwPlayerState> StateMachine;
	private SwClock InputLockoutClock;
	private SwClock IdleClock;
	protected abstract void BindStates();
    protected void BindState(SwStateData<SwPlayerState> stateData)
    {
        StateMachine.AddState(stateData);
    }
    protected void QueueState(SwPlayerState state)
    {
        StateMachine.QueueState(state);
    }
	// Animation
	private string[] Facing = ["right", "down", "left", "up"];
	private SwDelta<int> FacingIdx = new();
	protected SwInputManager InputManager{get;private set;}
	private AnimatedSprite2D BodySprite;
	private AnimatedSprite2D SpoonSprite;
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
    protected void PlayBodyAnim(string animName)
    {
        BodySprite.Play(animName);
    }
    protected void PlayBodyAnimFaced(string animName)
    {
        PlayBodyAnim(animName + "_" + GetFacing());
    }
    protected void PlayBodyAnimFaced(string animName, int hands)
    {
        PlayBodyAnimFaced($"{animName}{hands}");
    }
	// Input
	private SwInputBuffer InputBuffer;
	protected abstract string[] InitInputBuffer();
	public override void _Ready()
	{
		BodySprite = GetNode<AnimatedSprite2D>("BodySprite");
		SpoonSprite = GetNode<AnimatedSprite2D>("SpoonSprite");
		InputManager = SwGlobal.GetInputManager();
		StateMachine = new(SwPlayerState.Default);
        BindStates();
		InputLockoutClock = AddClock(new(){Duration = 0, IsPaused = true});
		IdleClock = AddClock(new(){Duration = 0, IsPaused = true, OnFinish = ()=>StateMachine.QueueState(SwPlayerState.Default)});
		InputBuffer = AddPoller(new SwInputBuffer(InitInputBuffer()));
		base._Ready();
	}
	protected override void Tick(float dt)
	{
		int facingIdx = Mathf.RoundToInt(GetLastVelocity().Angle() / (Mathf.Pi * 0.5f));
		if (facingIdx < 0) facingIdx += 4;
		FacingIdx.Value = facingIdx;
		StateMachine.Tick(dt);
	}
	
	protected override float GetDeathDelay()
    {
        return DeathDelay;
    }

    protected override float GetMaxHealth()
    {
        return MaxHealth;
    }
}
