using System;

namespace SW.Src.Timer;

public abstract class SwTimer
{
    public SwTimer(){}
    public SwTimer(Action onStart, Action onFinish = null, Action<float> onTick = null, bool repeats = false)
    {
        if(onStart is not null) OnStart = onStart;
        if(onFinish is not null) OnFinish = onFinish;
        if(onTick is not null) OnTick = onTick;
        if(onStart is not null) OnStart = onStart;
        Repeats = repeats;
    }
    public readonly bool Repeats = false;
    public bool IsPaused = false;
    private bool IsFirstTick = true;
    protected readonly Action OnStart = ()=>{};
    protected readonly Action OnFinish = ()=>{};
    protected Action<float> OnTick = (dt)=>{};
    protected virtual void TickInternal(float dt)
    {
        OnTick(dt);
    }
    public abstract bool IsFinished();
    public bool IsRunning(){return !IsPaused && !IsFinished();}
    public virtual void Restart()
    {
        IsFirstTick = true;
        IsPaused = false;
    }
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
}
