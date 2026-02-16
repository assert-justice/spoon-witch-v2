using Godot;
using SW.Src.Global;
using SW.Src.Input;
using SW.Src.StateMachine;
using SW.Src.Timer;
using SW.Src.Utils;

namespace SW.Src.Actor.Player;

public partial class SwPlayer : SwActor
{
	[Export] private float Speed = 300;
	public enum SwState
	{
		Idle,
		Running,
		Attacking,
		Dodging,
		Charging,
		UsingItem,
		InSubmenu,
	}
	private SwStateMachine<SwState> StateMachine;
	private string[] Facing = ["right", "down", "left", "up"];
	private SwDirty<int> FacingIdx = new(1);
	private SwInputManager InputManager;
	private AnimatedSprite2D BodySprite;
	private AnimatedSprite2D SpoonSprite;
	private SwPlayerControls Controls;
	private SwClock InputLockout;
	public override void _Ready()
	{
		BodySprite = GetNode<AnimatedSprite2D>("BodySprite");
		SpoonSprite = GetNode<AnimatedSprite2D>("SpoonSprite");
		InputManager = SwGlobal.GetInputManager();
		StateMachine = new(SwState.Idle);
		BindStateIdle();
		BindStateRunning();
		InputLockout = AddTimer(new SwClock());
		Controls = new(StateMachine.QueueState, InputManager, InputLockout);
		base._Ready();
	}
	protected override void Tick(float dt)
	{
		int facingIdx = Mathf.RoundToInt(GetLastVelocity().Angle() / (Mathf.Pi * 0.5f));
		if (facingIdx < 0) facingIdx += 4;
		FacingIdx.Value = facingIdx;
		// Player controls are not used or updated while in a submenu
		if(StateMachine.GetState() != SwState.InSubmenu) Controls.Poll(dt);
		StateMachine.Tick(dt);
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
}
