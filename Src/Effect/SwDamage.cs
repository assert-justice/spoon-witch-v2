using Godot;

namespace SW.Src.Effect;

[GlobalClass][Tool]
public partial class SwDamage : Resource
{
    [Export] public SwDamageType Type{get; private set;}
    [Export] public float Value{get; private set;}
    public SwDamage(){}
    public SwDamage(SwDamageType type, float value)
    {
        Type = type;
        Value = value;
    }
}
