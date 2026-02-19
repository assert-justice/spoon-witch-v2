using Godot;
using SW.Src.Global;
using SW.Src.Input;
using SW.Src.Utils;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerControls : ISwPoll
{
    private readonly SwInputBuffer InputBuffer;
    private readonly SwInputManager InputManager;
    // private SwDelta<bool> IsMoving_ = new();
    public SwPlayerControls()
    {
        InputManager = SwGlobal.GetInputManager();
        InputBuffer = new([]);
    }
    public SwInputBuffer GetInputBuffer(){return InputBuffer;}

    public void Poll()
    {
        InputBuffer.Poll();
        // IsMoving_.Value = InputManager.Move.GetValue().LengthSquared() > SwConstants.EPSILON;
    }
    public bool IsMoving(){return InputManager.Move.GetValue().LengthSquared() > SwConstants.EPSILON;}
    public Vector2 Move(){return InputManager.Move.GetValue();}
    public Vector2 Aim(){return InputManager.Aim.GetValue();}
    public bool JustAttacked(){return InputManager.SpoonAttack.IsJustPressed();}
    public bool JustCharged(){return InputManager.ChargeSling.IsJustPressed();}
    public bool IsCharging(){return InputManager.ChargeSling.IsPressed();}
    public bool JustDodged(){return InputManager.Dodge.IsJustPressed();}
    public bool JustUsedItem(){return InputManager.UseItem.IsJustPressed();}
}
