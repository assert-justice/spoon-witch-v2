using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SW.Src.Global;

namespace SW.Src.Input;

public class SwVButton(float pulseCooldown = 0, float pulseDelay = Mathf.Inf) : SwInput, ISwInput<bool>
{
    private readonly List<Func<bool>> Fns = [];
    private readonly float PulseCooldown = pulseCooldown;
    private readonly float PulseDelay = pulseDelay;
    private bool PulseState = false;
    protected bool State = false;
    protected bool LastState = false;
    private float Duration = 0;
    public virtual void Tick(float dt)
    {
        if(State == LastState) Duration += dt;
        else Duration = 0;
        // Handle pulses
        // If the duration hasn't gotten past the pulse delay or the button isn't down then we're done
        // Otherwise we want to check if we crossed one of the pulse cooldown thresholds
        // We take the duration, shave off the pulse delay, and get rid of the previous thresholds
        // If what's left over of the duration is less than or equal to dt, that means we just crossed a threshold
        // Todo: testing plz
        if(Duration < PulseDelay || !State) return;
        float duration = Duration - PulseDelay;
        duration -= Mathf.Floor(duration / PulseCooldown) * PulseCooldown;
        PulseState = duration <= dt;
    }
    public bool IsPressed(){return State;}
    public bool IsJustPressed(){return State && !LastState;}
    public bool IsJustReleased(){return !State && LastState;}
    public bool IsPressedBuffered(float buffer){return State || Duration < buffer;}
    public bool IsReleasedBuffered(float buffer){return !State || Duration < buffer;}
    public float GetDuration(){return Duration;}
    public void Suppress(){LastState = State;}
    public bool IsPulsed(){return PulseState;}
    public void Poll()
    {
        LastState = State;
        State = Fns.Any(fn => fn());
    }
    public void Clear(){Fns.Clear();}
    public void AddFn(Func<bool> fn){Fns.Add(fn);}
    public void AddPhysicalKey(Key key)
    {
        AddFn(()=>Godot.Input.IsPhysicalKeyPressed(key));
    }
    // public void AddKey(Key key)
    // {
    //     AddFn(()=>Godot.Input.IsKeyPressed(key));
    // }
    public void AddMouseButton(MouseButton mouseButton)
    {
        AddFn(()=>Godot.Input.IsMouseButtonPressed(mouseButton));
    }
    public void AddJoyButton(JoyButton button, int device = -1)
    {
        AddFn(()=>Godot.Input.IsJoyButtonPressed(device, button));
    }
    public void AddAxisPos(Func<float> fn)
    {
        AddFn(()=>fn() > 0);
    }
    public void AddAxisNeg(Func<float> fn)
    {
        AddFn(()=>fn() < 0);
    }

    public override bool TryAddBind(SwInputBind inputBind)
    {
        switch (inputBind.MethodName)
        {
            case "AddPhysicalKey":
                if(inputBind.Values.Length != 1) return false;
                Key key = (Key)inputBind.Values[0];
                if(Enum.IsDefined(key)) return false;
                AddPhysicalKey(key);
                return true;
            case "AddMouseButton":
                if(inputBind.Values.Length != 1) return false;
                MouseButton mouseButton = (MouseButton)inputBind.Values[0];
                if(Enum.IsDefined(mouseButton)) return false;
                AddMouseButton(mouseButton);
                return true;
            case "AddJoyButton":
                if(inputBind.Values.Length != 1) return false;
                JoyButton joyButton = (JoyButton)inputBind.Values[0];
                if(Enum.IsDefined(joyButton)) return false;
                AddJoyButton(joyButton);
                return true;
            case "AddAxisPos":
                if(inputBind.SourceNames.Length != 1) return false;
                if(!SwGlobal.GetInputManager().TryGetAxis(inputBind.SourceNames[0], out var axis)) return false;
                AddAxisPos(axis.GetValue);
                return true;
            case "AddAxisNeg":
                if(inputBind.SourceNames.Length != 1) return false;
                if(!SwGlobal.GetInputManager().TryGetAxis(inputBind.SourceNames[0], out axis)) return false;
                AddAxisNeg(axis.GetValue);
                return true;
            default:
                throw new Exception("Should be unreachable");
        }
    }
}
