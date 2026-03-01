using SW.Src.Utils;

namespace SW.Src.GameSpace.GameState;

public class SwGameStateGameOver : SwStateMachine<SwGame, SwGame.GameState>.SwStateData
{
    public SwGameStateGameOver(SwGame parent) : base(parent, SwGame.GameState.GameOver)
    {
    }
    public override void EnterState(SwGame.GameState lastState)
    {
    }
    public override void ExitState(SwGame.GameState lastState)
    {
    }
    public override void Tick(float dt)
    {
    }
}
