using System.Collections.Generic;
using Godot;
using SW.Src.Input;

namespace SW.Src.Global;

public partial class SwGlobal : Node
{
    private static SwGlobal Global;
    private SwInputManager InputManager;
    public static SwInputManager GetInputManager(){return Global.InputManager;}
    private SwSettings Settings = new();
    public static SwSettings GetSettings(){return Global.Settings;}
    private float Delta;
    public static float GetDelta(){return Global.Delta;}
    private static InputEvent LastInputEvent;
    public static InputEvent GetLastInputEvent(){return LastInputEvent;}
    public static bool WasLastInputKbm()
    {
        return LastInputEvent is InputEventKey || LastInputEvent is InputEventMouse;
    }
    public static bool WasLastInputJoy()
    {
        return LastInputEvent is InputEventJoypadButton || LastInputEvent is InputEventJoypadMotion;
    }
    public override void _Ready()
    {
        Global = this;
        InputManager = new();
        InputManager.BindDefaults();
    }
    public override void _PhysicsProcess(double delta)
    {
        Delta = (float) delta;
        InputManager.Tick(Delta);
    }
    public override void _Input(InputEvent inputEvent)
    {
        LastInputEvent = inputEvent;
    }
    // I don't know where else to put this
    public static bool IsEqual<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }
}
