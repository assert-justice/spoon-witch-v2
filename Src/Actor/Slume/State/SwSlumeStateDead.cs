using SW.Src.GameSpace.Dungeon;
using SW.Src.Global;
using SW.Src.Ui;
using SW.Src.Utils;
using static SW.Src.Actor.Slume.SwSlume;

namespace SW.Src.Actor.Slume.State;

public class SwSlumeStateDead(SwSlume parent) : SwStateMachine<SwSlume, SwState>.SwStateData(parent, SwState.Dead)
{
    public override void EnterState(SwState lastState)
    {
        Parent.Animator.PlayBodyAnim("death");
        Parent.Hurtbox.IsEnabled = false;
        Parent.Hitbox.Disabled = true;
        if(Parent.IsPassive() && !SwGlobal.GetSettings().CreativeMode)
        {
            Main.Message("tutorial:kill");
            SwDungeon.Message("clear_rect:3269a9f0-fa90-11f0-9f4c-ad66fec81f90,2");
        }
    }
    public override void Tick(float dt)
    {
        if(Parent.Animator.IsPlaying()) return;
        if(Parent.InSleepRadius()) return;
        Parent.Cleanup();
    }
}
