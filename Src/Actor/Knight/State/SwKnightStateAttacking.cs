using Godot;
using SW.Src.Timer;
using SW.Src.Utils;

namespace SW.Src.Actor.Knight.State;

public class SwKnightStateAttacking : SwStateMachine<SwKnight, SwKnight.SwState>.SwStateData
{
	private readonly SwClock SwingClock;
	private readonly SwClock DelayClock;
	private SwKnight.SwState LastState;
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
		SwingClock.Restart();
		DelayClock.Restart();
		Parent.ChargeRecoveryClock.Restart();
		Parent.AttackCooldownClock.Restart();
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
