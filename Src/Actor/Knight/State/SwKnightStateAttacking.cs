using Godot;
using SW.Src.Timer;
using SW.Src.Utils;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateAttacking : SwStateMachine<SwKnight, SwKnight.SwState>.SwStateData
{
	private readonly SwClock SwingClock;
	private readonly SwClock DelayClock;
	private SwKnight.SwState LastState;
	private int FacingIdx = 0;
	public SwKnightStateAttacking(SwKnight parent) : base(parent, SwKnight.SwState.Attacking)
	{
        float duration = Parent.Animator.GetSwordSwingDuration();
		float delay = Parent.AttackDelayFrames / Parent.Animator.GetSwordSwingFps();
		SwingClock = new(new(){Duration = duration});
		DelayClock = new(new(){Duration = delay, OnFinish = ()=> Parent.Evoker.HurtboxEnable(true)});
	}
	public override void EnterState(SwKnight.SwState lastState)
	{
		LastState = lastState;
		Parent.Evoker.StartSwordAttack();
		Parent.Animator.PlaySwordSwing();
		Parent.AudioManager.PlayAttackSound();
		FacingIdx = Parent.GetLastFacing4();
		SwingClock.Restart();
		DelayClock.Restart();
	}
	public override void ExitState(SwKnight.SwState nextState)
	{
		Parent.Evoker.HurtboxEnable(false);
        Parent.Animator.EndSwordSwing();
	}
	public override void Tick(float dt)
	{
		DelayClock.Tick(dt);
		SwingClock.Tick(dt);
		if(!SwingClock.IsRunning()) Parent.StateMachine.QueueState(LastState);
	}
}
