using System;
using Godot;

namespace SW.Src.Input;

public abstract class SwInput
{
    public static bool GetKey(Key key)
    {
        return Godot.Input.IsKeyPressed(key);
    }
    public static bool GetPhysicalKey(Key key)
    {
        return Godot.Input.IsPhysicalKeyPressed(key);
    }
    public static bool GetMouseButton(MouseButton button)
    {
        return Godot.Input.IsMouseButtonPressed(button);
    }
    public static bool GetJoyButton(JoyButton button, int device = 0)
    {
        // Todo: if device index is negative read from all connected devices
        return Godot.Input.IsJoyButtonPressed(0, button);
    }
    public static float GetJoyAxis(JoyAxis axis, int device = 0)
    {
        return Godot.Input.GetJoyAxis(0, axis);
    }
    public abstract bool TryAddBind(SwInputBind inputBind);
}
