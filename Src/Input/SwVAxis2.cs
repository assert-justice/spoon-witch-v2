using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Utils;

namespace SW.Src.Input;

public partial class SwVAxis2 : SwInput, ISwInput<Vector2>
{
    private readonly List<Func<Vector2>> Fns = [];
    private Vector2 Value = Vector2.Zero;
    private Func<Vector2,Vector2> Filter;
    private SwMaxAbs AvgX = new();
    private SwMaxAbs AvgY = new();
    public SwVAxis2(float deadzone = 0.25f)
    {
        SetDeadzone(deadzone);
    }
    public void AddFn(Func<Vector2> fn)
    {
        Fns.Add(fn);
    }
    public void AddPhysicalKeys(Key negX, Key posX, Key negY, Key posY)
    {
        AddFn(() =>
        {
            float x = (GetPhysicalKey(posX)?1:0) - (GetPhysicalKey(negX)?1:0);
            float y = (GetPhysicalKey(posY)?1:0) - (GetPhysicalKey(negY)?1:0);
            return new(x,y);
        });
    }
    public void AddJoyButtons(JoyButton negX, JoyButton posX, JoyButton negY, JoyButton posY, int device = -1)
    {
        AddFn(() =>
        {
            float x = (GetJoyButton(posX,device)?1:0) - (GetJoyButton(negX,device)?1:0);
            float y = (GetJoyButton(posY,device)?1:0) - (GetJoyButton(negY,device)?1:0);
            return new(x,y);
        });    }
    public void AddJoyAxes(JoyAxis x, JoyAxis y, int device = -1)
    {
        AddFn(()=>new(GetJoyAxis(x, device), GetJoyAxis(y, device)));
    }
    public void Clear()
    {
        Fns.Clear();
    }
    public void Poll()
    {
        AvgX.Reset();
        AvgY.Reset();
        foreach (var fn in Fns)
        {
            Vector2 vec = fn();
            AvgX.AddValue(vec.X);
            AvgY.AddValue(vec.Y);
        }
        Value = Filter(new(AvgX.GetValue(), AvgY.GetValue()));
    }
    public void SetFilter(Func<Vector2,Vector2> filter){Filter = filter;}
    public void SetDeadzone(float deadzone)
    {
        Vector2 fn(Vector2 vec)
        {
            float len = vec.Length();
            // If len is within deadzone, round it to zero
            if(len < deadzone) return Vector2.Zero;
            // If len is greater than 1, clamp it to 1
            if(len > 1) return vec.Normalized();
            // Otherwise we want a new length between 0 and 1. So if it's a tiny bit outside of the deadzone it's a low value
            // If we didn't do that the value would "jump" from zero to the deadzone, we don't want that.
            // Different games and genres need different deadzones but this is simple and works across most applications.
            len -= deadzone;
            len /= 1 - deadzone;
            return vec.Normalized() * len;
        }
        Filter = fn;
    }
    public Vector2 GetValue(){return Value;}

    public override bool TryAddBind(SwInputBind inputBind)
    {
        throw new NotImplementedException();
    }
}
