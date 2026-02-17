using System;
using SW.Src.Utils;

namespace SW.Src.Timer;

public abstract class SwTimer(SwTimerData timerData) : ISwTick
{
    private bool IsFirstTick = true;
    private readonly Action OnStart = timerData.OnStart;
    private readonly Action OnFinish = timerData.OnFinish;
    private readonly Action<float> OnTick = timerData.OnTick;
    public readonly bool Repeats = timerData.Repeats;
    public bool IsPaused = timerData.IsPaused;
    protected virtual void TickInternal(float dt)
    {
        OnTick(dt);
    }
    public bool IsRunning(){return !IsPaused && !IsFinished();}
    public void Tick(float dt)
    {
        if(IsPaused) return;
        if(IsFinished()) return;
        if (IsFirstTick)
        {
            IsFirstTick = false;
            OnStart();
        }
        TickInternal(dt);
        if (IsFinished())
        {
            if(Repeats)Restart();
            else OnFinish();
        }
    }
    public abstract bool IsFinished();
    public virtual void Restart(bool isPaused = false)
    {
        IsFirstTick = true;
        IsPaused = isPaused;
    }
}

public record class SwTimerData
{
    public Action OnStart{get; init;} = ()=>{};
    public Action OnFinish{get; init;} = ()=>{};
    public Action<float> OnTick{get; init;} = (dt)=>{};
    public bool IsPaused{get; init;} = false;
    public bool Repeats{get; init;} = false;
}
