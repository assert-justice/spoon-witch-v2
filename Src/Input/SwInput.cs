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
    public static bool GetJoyButton(JoyButton button, int device = -1)
    {
        return Godot.Input.IsJoyButtonPressed(device, button);
    }
    public static float GetJoyAxis(JoyAxis axis, int device = -1)
    {
        return Godot.Input.GetJoyAxis(device, axis);
    }
    public abstract bool TryAddBind(SwInputBind inputBind);
}
