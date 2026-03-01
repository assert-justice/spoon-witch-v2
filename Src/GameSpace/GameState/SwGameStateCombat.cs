using SW.Src.Utils;

namespace SW.Src.GameSpace.GameState;

public class SwGameStateCombat : SwStateMachine<SwGame, SwGame.GameState>.SwStateData
{
    public SwGameStateCombat(SwGame parent) : base(parent, SwGame.GameState.Combat)
    {
    }
    public override void EnterState(SwGame.GameState lastState)
    {
        Parent.QueueMusic(SwGame.COMBAT_THEME);
    }
    public override void Tick(float dt)
    {
        if(!Parent.InCombat){Parent.StateMachine.QueueState(SwGame.GameState.Default);}
    }
}
