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
    // [Export] public string TileCoordLookup;
    public SwTileCoordLookup TileCoordLookup;
    // [Export] private int TileWidth{get=>TileWidth_.Value; set=>TileWidth_.Value = value;}
    // private SwOnChange<int> TileWidth_;
    // [Export] private int TileHeight{get=>TileHeight_.Value; set=>TileHeight_.Value = value;}
    // private SwOnChange<int> TileHeight_;
    // private SwOnChange<SwTerrainData[]> Terrains_;
    // [Export] public SwTerrainData[] Terrains {get=>Terrains_.Value; set=>Terrains_.Value = value;}
    // private TileSet Tiles = new();
    // private SwDirtyFlag NeedsRebuild_ = new();
    // private readonly long ConstructorTime;
    // [Export] public bool NeedsRebuild{get=>NeedsRebuild_.IsDirty(); private set{}}
    // public SwTerrain()
    // {
    //     GD.Print("constructor");
    //     ConstructorTime = DateTime.UtcNow.Ticks;
    //     Changed += QueueRebuild;
    //     TileWidth_ = new(QueueRebuild, 32);
    //     TileHeight_ = new(QueueRebuild, 32);
    //     Terrains_ = new(QueueRebuild, []);
    //     // Terrains_ = new(() =>
    //     // {
    //     //     GD.Print("sup");
    //     // }, null, (lastValue, nextValue) =>
    //     // {
    //     //     GD.Print($"old len: {lastValue?.Length}, new len: {nextValue.Length}");
    //     //     // Lots of false negatives but it's whatever
    //     //     return lastValue?.Length == nextValue.Length;
    //     // });
    //     // Tiles = new();
    //     // Rebuild();
    // }
    // private void QueueRebuild()
    // {
    //     long elapsed = DateTime.UtcNow.Ticks - ConstructorTime;
    //     var e = TimeSpan.FromTicks(elapsed);
    //     GD.Print($"c time: {ConstructorTime}, now: {DateTime.UtcNow.Ticks}, elapsed: {e.Milliseconds / 1000.0f} seconds");
    //     if(ConstructorTime == DateTime.UtcNow.Ticks)
    //     {
    //         GD.Print("not today satan");
    //         return;
    //     }
    //     GD.Print("Rebuild queued");
    //     NeedsRebuild_.SetDirty();
    //     // EmitChanged();
    // }
    // private void Rebuild()
    // {
    //     if(!NeedsRebuild_.IsDirty()) return;
    //     NeedsRebuild_.Clean();
    //     // Create tile set
    //     Tiles.TileSize = new(TileWidth, TileWidth);
    //     // Add physics layer if needed
    //     if(Tiles.GetPhysicsLayersCount() == 0) Tiles.AddPhysicsLayer(0);
    //     // Remove existing sources
    //     int sourceCount = Tiles.GetSourceCount();
    //     if(sourceCount > 0)
    //     {
    //         for (int idx = sourceCount - 1; idx <= 0; idx--)
    //         {
    //             Tiles.RemoveSource(Tiles.GetSourceId(idx));
    //         }
    //     }
    //     // Create base atlas
    //     var baseAtlas = SwTerrainUtils.AddAtlas(Tiles, 1, 1, out _);
    //     SwTerrainUtils.AddTile(baseAtlas, Vector2I.Zero);
    //     // Create collision atlas
    //     var collisionAtlas = SwTerrainUtils.AddAtlas(Tiles, 1, Terrains.Length + 1, out _);
    //     SwTerrainUtils.AddTile(collisionAtlas, Vector2I.Zero);
    //     SwTerrainUtils.AddTile(collisionAtlas, Vector2I.Right, true);
    //     // Create terrain atlases and tile coord lookup
    //     TileCoordLookup.Clear();
    //     foreach (var terrain in Terrains)
    //     {
    //         int sourceId = Tiles.GetSourceCount();
    //         var texture = terrain.Texture;
    //         var image = texture.GetImage();
    //         var atlas = SwTerrainUtils.AddAtlas(Tiles, texture, out _);
    //         for (int x = 0; x < texture.GetWidth(); x++)
	// 		{
	// 			for (int y = 0; y < texture.GetHeight(); y++)
	// 			{
	// 				var region = image.GetRegion(new(TileWidth * x, TileHeight * y, TileWidth, TileHeight));
	// 				if(region.IsInvisible()) continue;
	// 				Vector2I tileCoord = new(x, y);
	// 				if(!SwTerrainUtils.TryGetMask(tileCoord, out var mask))
	// 				{
	// 					GD.PrintErr($"No mask for region {x},{y}");
	// 					continue;
	// 				}
	// 				TileCoordLookup.AddCoords(mask, sourceId, tileCoord);
	// 				atlas.CreateTile(tileCoord);
	// 			}
	// 		}
    //     }
    //     // Try save changes
    //     var err = ResourceSaver.Save(this);
    //     if(err != Error.Ok) GD.PrintErr("Failed to save terrain resource");
    // }
    // public bool TryGetAtlasCoords((bool, bool, bool, bool) mask, int terrainIdx, out Vector2I atlasCoords)
    // {
    //     return TileCoordLookup.TryGetAtlasCoords(mask, terrainIdx + 2, out atlasCoords);
    // }
    // Can also export properties like footstep sounds, color for loose particles, whatever.
}
