using Godot;

namespace SW.Src.GameSpace.Terrain;

[GlobalClass][Tool]
public partial class SwTerrainRes : Resource
{
    [Export] public int TileWidth = 32;
    [Export] public int TileHeight = 32;
    [Export] public SwTerrainTypeRes[] TerrainTypes = [];
    [Export] public TileSet BaseTileSet;
    [Export] public TileSet CollisionTileSet;
    [Export] public TileSet DisplayTileSet;
}
