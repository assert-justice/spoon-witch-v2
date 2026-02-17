using SW.Src.Utils;
using static SW.Src.Actor.Player.SwPlayer;

namespace SW.Src.Actor.Player.State;

public class SwPlayerStateDodging(SwPlayer parent) :
    SwStateMachine<SwPlayer, SwState>.SwStateData(parent, SwState.Dodging)
{
    public override void EnterState(SwState lastState)
    {
    }
    public override void ExitState(SwState lastState)
    {
    }
    public override void Tick(float dt)
    {
    }
}
