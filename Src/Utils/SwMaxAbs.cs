using Godot;

namespace SW.Src.Utils;

public struct SwMaxAbs
{
    private float MaxAbs = 0;
    private float Value = 0;
    public SwMaxAbs(){}
    public void Reset()
    {
        MaxAbs = 0;
        Value = 0;
    }
    public void AddValue(float value)
    {
        float abs = Mathf.Abs(value);
        if(abs > MaxAbs)
        {
            Value = value;
            MaxAbs = abs;
        }
    }
    public readonly float GetValue()
    {
        return Value;
    }
}
