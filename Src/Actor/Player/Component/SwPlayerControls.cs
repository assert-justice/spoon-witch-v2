using Godot;
using SW.Src.Global;
using SW.Src.Input;
using SW.Src.Utils;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerControls : ISwPoll
{
    private readonly SwInputBuffer InputBuffer;
    private readonly SwInputManager InputManager;
    private readonly SwPlayer Parent;
    public SwPlayerControls(SwPlayer parent)
    {
        Parent = parent;
        InputManager = SwGlobal.GetInputManager();
        InputBuffer = new([]);
    }
    public SwInputBuffer GetInputBuffer(){return InputBuffer;}

    public void Poll()
    {
        InputBuffer.Poll();
        Aim = Vector2.Zero;
        var joyAim = InputManager.Aim.GetValue();
        var move = InputManager.Move.GetValue();
        // Aim = InputManager.Aim.GetValue();
        if (SwGlobal.InputMode == SwGlobal.SwInputMode.Kb)
        {
            var viewport = Parent.GetViewport();
            Vector2 center = viewport.GetVisibleRect().Size / 2;
            Aim = Parent.GetViewport().GetMousePosition() - center;
        }
        else Aim = joyAim;
        // else if (joyAim.LengthSquared() > SwConstants.EPSILON)
        // {
        //     // Aim = Parent.GetLastVelocity();//.Normalized();
        // }
        // else if (move.LengthSquared() > SwConstants.EPSILON)
        // {
        //     Aim = move;
        // }
        // float length = Aim.Length(); 
        AimLength = Aim.Length();
        Aim = Aim.Normalized();
        if(AimLength > SwConstants.EPSILON)
        {
            LastAim = Aim;
        }
        else if(move.LengthSquared() > SwConstants.EPSILON) LastAim = move.Normalized();
        // IsMoving_.Value = InputManager.Move.GetValue().LengthSquared() > SwConstants.EPSILON;
    }
    public bool IsMoving(){return InputManager.Move.GetValue().LengthSquared() > SwConstants.EPSILON;}
    public Vector2 Move(){return InputManager.Move.GetValue();}
    public Vector2 Aim{get; private set;} = Vector2.Zero;
    public Vector2 LastAim{get; private set;} = Vector2.Zero;
    public float AimLength{get; private set;} = 0;
    public bool JustAttacked(){return InputManager.SpoonAttack.IsJustPressed();}
    public bool JustCharged(){return InputManager.ChargeSling.IsJustPressed();}
    public bool IsCharging(){return InputManager.ChargeSling.IsPressed();}
    public bool IsChargingJustReleased(){return InputManager.ChargeSling.IsJustReleased();}
    public bool JustDodged(){return InputManager.Dodge.IsJustPressed();}
    public bool JustUsedItem(){return InputManager.UseItem.IsJustPressed();}
    public bool JustHealed(){return InputManager.Heal.IsJustPressed();}
}
