using System;

namespace SW.Src.Timer;

public class SwClock : SwTimer
{
    private float Duration = 0;
    private float Time = 0;
    public SwClock()
    {
        IsPaused = true;
    }
    public SwClock(float duration)
    {
        SetDuration(duration);
    }
    public SwClock(float duration, 
        bool repeats, 
        Action onStart = null, 
        Action onFinish = null, 
        Action<float> onTick = null)
        :base(onStart, onFinish, onTick, repeats)
    {
        SetDuration(duration);
    }
    public float GetTime(){return Time;}
    public float GetDuration(){return Duration;}
    public float GetProgress(){return Time / Duration;}
    public void SetDuration(float duration)
    {
        Restart();
        if(duration <= 0) IsPaused = true;
        Duration = duration;
    }
    public override void Restart()
    {
        base.Restart();
        Time = 0;
    }
    public override bool IsFinished(){return Time > Duration;}
    protected override void TickInternal(float dt)
    {
        base.TickInternal(dt);
        Time += dt;
    }
}
