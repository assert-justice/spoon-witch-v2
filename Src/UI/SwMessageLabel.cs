using Godot;
using SW.Src.Timer;

namespace SW.Src.Ui;

public partial class SwMessageLabel : Label
{
    private readonly SwClock DeathClock = new(new(){});
    public float Duration = 1;
    public override void _Ready()
    {
        DeathClock.SetDuration(Duration);
    }
    public override void _Process(double delta)
    {
        DeathClock.Tick((float)delta);
        if(!DeathClock.IsRunning()) QueueFree(); // Maybe try a fadeout or something later
    }
}
