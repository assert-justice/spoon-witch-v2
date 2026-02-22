using System.Text.Json;
using Godot;

namespace SW.Src.GameSpace.Terrain;

[Tool] public partial class SwTerrainTool : Node
{
	[Export] private int TileWidth = 32;
	[Export] private int TileHeight = 32;
	[Export] private SwTerrainTypeRes[] TerrainTypes = [];
	[ExportToolButton("Rebuild")]
	public Callable QueueRebuildTileSetButton => Callable.From(Rebuild);
	[Export] private SwTerrainRes TerrainData;
	private void Rebuild()
	{
		TerrainData.TerrainTypes = TerrainTypes;
		// Base tile set
		TileSet tileSet = new()
		{
			TileSize = new(TileWidth, TileHeight)
		};
		TerrainData.BaseTileSet = tileSet;
		var atlas = SwTerrainUtils.AddAtlas(tileSet, 1, 1, out _);
		SwTerrainUtils.AddTile(atlas, Vector2I.Zero);
		// Collision tile set
		tileSet = new()
		{
			TileSize = new(TileWidth, TileHeight)
		};
		TerrainData.CollisionTileSet = tileSet;
		tileSet.AddPhysicsLayer();
		atlas = SwTerrainUtils.AddAtlas(tileSet, 2, 1, out _);
		SwTerrainUtils.AddTile(atlas, Vector2I.Zero);
		SwTerrainUtils.AddTile(atlas, Vector2I.Right, true);
		// Visual tile set
		tileSet = new()
		{
			TileSize = new(TileWidth, TileHeight)
		};
		TerrainData.DisplayTileSet = tileSet;
		// SwTileCoordLookup coordLookup = new();
		// TerrainData.TileCoordLookup = coordLookup;
		// Loop through terrain types
		foreach (var terrain in TerrainTypes)
		{
			// init atlas
			var texture = terrain.Texture;
			atlas = SwTerrainUtils.AddAtlas(tileSet, texture, out int atlasId);
			var image = texture.GetImage();
			int width = texture.GetWidth() / TileWidth;
			int height = texture.GetHeight() / TileHeight;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var region = image.GetRegion(new(TileWidth * x, TileHeight * y, TileWidth, TileHeight));
					if(region.IsInvisible()) continue;
					Vector2I tileCoord = new(x, y);
					if(!SwTerrainUtils.TryGetMask(tileCoord, out var mask))
					{
						GD.PrintErr($"No mask for region {x},{y}");
						continue;
					}
					GD.Print(mask, atlasId, tileCoord);
					// coordLookup.AddCoords(mask, atlasId, tileCoord);
					atlas.CreateTile(tileCoord);
				}
			}
		}
		// TerrainData.TileCoordLookupJson = JsonSerializer.Serialize(coordLookup);
		ResourceSaver.Save(TerrainData);
	}
}
