namespace SW.Src.Timer;

public class SwClock(SwClockData clockData) : SwTimer(clockData)
{
    private float Duration = clockData.Duration;
    private float Time = 0;

    public float GetTime(){return Time;}
    public float GetDuration(){return Duration;}
    public float GetProgress(){return Time / Duration;}
    public void SetDuration(float duration)
    {
        Restart();
        if(duration <= 0) IsPaused = true;
        Duration = duration;
    }
    public override void Restart(bool isPaused = false)
    {
        base.Restart(isPaused);
        Time = 0;
    }
    public override bool IsFinished(){return Time > Duration;}
    protected override void TickInternal(float dt)
    {
        base.TickInternal(dt);
        Time += dt;
    }
}

public record class SwClockData : SwTimerData
{
    public float Duration{get; init;} = 1;
}
