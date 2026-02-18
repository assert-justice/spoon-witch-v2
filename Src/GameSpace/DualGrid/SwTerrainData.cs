using Godot;

namespace SW.Src.GameSpace.DualGrid;

[GlobalClass][Tool]
public partial class SwTerrainData : Resource
{
    [Export] public Texture2D Texture;
    [Export] public float MovementMul = 1; // Set to 0 to indicate a solid
    public SwTerrainData(){}
    // Can also export properties like footstep sounds, color for loose particles, whatever.
}
