using SW.Src.Utils;

namespace SW.Src.GameSpace.GameState;

public class SwGameStateDefault : SwStateMachine<SwGame, SwGame.GameState>.SwStateData
{
    public SwGameStateDefault(SwGame parent) : base(parent, SwGame.GameState.Default)
    {
    }
    public override void EnterState(SwGame.GameState lastState)
    {
        // Chose music track
        Parent.QueueMusic(SwGame.BEACH_THEME);
    }
    public override void Tick(float dt)
    {
        if(Parent.InCombat){Parent.StateMachine.QueueState(SwGame.GameState.Combat);}
    }
}
