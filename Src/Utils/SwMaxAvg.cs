using Godot;

namespace SW.Src.Utils;

public struct SwMaxAvg
{
    private float Min;
    private float Max;
    public SwMaxAvg(){Reset();}
    public void Reset()
    {
        Min = Mathf.Inf;
        Max = -Mathf.Inf;
    }
    public void AddValue(float value)
    {
        if(value > Max) Max = value;
        if(value < Min) Min = value;
    }
    public readonly float GetValue()
    {
        float val = (Max + Min) * 0.5f;
        return float.IsNaN(val) ? 0 : val;
    }
}
