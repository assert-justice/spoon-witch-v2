using Godot;

namespace SW.Src.GameSpace.Terrain;

[GlobalClass][Tool]
public partial class SwTerrainTypeRes : Resource
{
    [Export] public Texture2D Texture;
    [Export] public bool IsSolid = false;
    [Export] public float MovementMul = 1;
    // [Export(PropertyHint.Flags)] public uint CollisionLayer;
    // [Export(PropertyHint.Flags)] public uint CollisionMask;
    // Footstep sound effect
    // Ground particle color & whether to use or not
    // Other properties
    public SwTerrainTypeRes(){}
    public bool GetIsSolid(){return IsSolid || MovementMul == 0;}
}
