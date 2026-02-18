using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.DualGrid;

[Tool]
public partial class SwDualGrid : Node2D
{
	[Export] private int TileWidth = 32;
	[Export] private int TileType = 0;
	[Export] private int NumLayers = 4;
	private int CurrentLayer_ = 0;
	[Export] private int CurrentLayer{get=>CurrentLayer_; set
		{
			if(value < 0 || value >= NumLayers)
			{
				GD.PrintErr($"Attempted to use an invalid layer '{value}'");
			}
			CurrentLayer_ = value;
		}
	}
	[Export] private SwTerrainData[] TerrainDataArray;
	// Dirty flags
	private bool LayersNeedRebuild = false;
	[ExportToolButton("Queue Rebuild Layers")]
	public Callable QueueRebuildLayersButton => Callable.From(()=>LayersNeedRebuild = true);
	private bool TilesNeedRefresh = false;
	[ExportToolButton("Queue Refresh Tiles")]
	public Callable QueueRefreshTilesButton => Callable.From(()=>TilesNeedRefresh = true);
	// Children
	private TileSet SharedTileSet;
	private TileMapLayer CollisionLayer;
	private readonly List<TileMapLayer> VisualMapLayers = [];
	private readonly SwTileCoordLookup CoordLookup = new();
	public override void _Ready()
	{
		// Consider saving data to Terrain Data on rebuild in editor, to speed up player loads.
		if (Engine.IsEditorHint())
		{
			// RebuildTileLayers();
			// Not working?
			// GD.Print("Tool ready");
			// QueueRebuildLayers = true;
		}
		// RebuildTileSet();
	}
	public override void _PhysicsProcess(double delta)
	{
		if (LayersNeedRebuild)
		{
			if(GetChildCount() > 0) FreeChildren();
			else
			{
				LayersNeedRebuild = false;
				RebuildTileLayers();
			}
		}
		if (TilesNeedRefresh)
		{
			TilesNeedRefresh = false;
			RefreshAllTiles();
		}
	}
	private void RebuildTileSet()
	{
		SharedTileSet = new()
		{
			TileSize = new(TileWidth, TileWidth)
		};
		// TileSet SharedTileSet = new()
		// {
		// 	TileSize = new(TileWidth, TileWidth)
		// };
		// CollisionLayer.TileSet = tileSet;
		// Add physics layer
		SharedTileSet.AddPhysicsLayer();
		// Create collision layer atlas
		var image = Image.CreateEmpty(TileWidth * 2, TileWidth, false, Image.Format.Rgba8);
		Texture2D texture = ImageTexture.CreateFromImage(image);
		TileSetAtlasSource atlas = new()
		{
			Texture = texture,
			TextureRegionSize = new(TileWidth, TileWidth)
		};
		texture.ResourceName = "test";
		SharedTileSet.AddSource(atlas);
		// Create non colliding tile
		atlas.CreateTile(Vector2I.Zero);
		// Create colliding tile
		atlas.CreateTile(Vector2I.Right);
		// Add polygon collision to tile
		var tileData = atlas.GetTileData(Vector2I.Right, 0);
		tileData.AddCollisionPolygon(0);
		tileData.SetCollisionPolygonPoints(0, 0, 
		[
			new Vector2(-0.5f, -0.5f) * TileWidth,
			new Vector2(0.5f, -0.5f) * TileWidth,
			new Vector2(0.5f, 0.5f) * TileWidth,
			new Vector2(-0.5f, 0.5f) * TileWidth,
		]);
		// Add visual tile atlases
		for (int tileType = 0; tileType < TerrainDataArray.Length; tileType++)
		{
			texture = TerrainDataArray[tileType].Texture;
			atlas = new()
			{
				Texture = texture,
				TextureRegionSize = new(TileWidth, TileWidth)
			};
			SharedTileSet.AddSource(atlas);
			image = texture.GetImage();
			for (int x = 0; x < image.GetWidth() / TileWidth; x++)
			{
				for (int y = 0; y < image.GetHeight() / TileWidth; y++)
				{
					var region = image.GetRegion(new(TileWidth * x, TileWidth * y, TileWidth, TileWidth));
					if(region.IsInvisible())
					{
						// GD.Print($"Skipped invisible region {x},{y}");
						continue;
					}
					Vector2I tileCoord = new(x, y);
					if(!SwMaskLookup.TryGetMask(tileCoord, out var mask))
					{
						GD.PrintErr($"No mask for region {x},{y}");
						continue;
					}
					CoordLookup.AddCoords(mask, tileType, tileCoord);
					atlas.CreateTile(tileCoord);
				}
			}
		}
		// this.SharedTileSet = SharedTileSet;
	}
	private void RebuildTileLayers()
	{
		GD.Print("RebuildTileLayers");
		// Add collision layer
		CollisionLayer = new()
		{
			Name = "CollisionLayer"
		};
		AddChild(CollisionLayer);
		CollisionLayer.Owner = this;
		// Offset collision layer
		CollisionLayer.Position = new(TileWidth * 0.5f, TileWidth * 0.5f);
		if(SharedTileSet is null)
		{
			RebuildTileSet();
		}
		CollisionLayer.TileSet = SharedTileSet;
		// Create collision layer tile set
		// TileSet tileSet = new()
		// {
		// 	TileSize = new(TileWidth, TileWidth)
		// };
		// CollisionLayer.TileSet = tileSet;
		// // Add physics layer
		// tileSet.AddPhysicsLayer();
		// // Create collision layer atlas
		// var image = Image.CreateEmpty(TileWidth * 2, TileWidth, false, Image.Format.Rgba8);
		// Texture2D texture = ImageTexture.CreateFromImage(image);
		// TileSetAtlasSource atlas = new()
		// {
		// 	Texture = texture,
		// 	TextureRegionSize = new(TileWidth, TileWidth)
		// };
		// texture.ResourceName = "test";
		// tileSet.AddSource(atlas);
		// // Create non colliding tile
		// atlas.CreateTile(Vector2I.Zero);
		// // Create colliding tile
		// atlas.CreateTile(Vector2I.Right);
		// // Add polygon collision to tile
		// var tileData = atlas.GetTileData(Vector2I.Right, 0);
		// tileData.AddCollisionPolygon(0);
		// tileData.SetCollisionPolygonPoints(0, 0, 
		// [
		// 	new Vector2(-0.5f, -0.5f) * TileWidth,
		// 	new Vector2(0.5f, -0.5f) * TileWidth,
		// 	new Vector2(0.5f, 0.5f) * TileWidth,
		// 	new Vector2(-0.5f, 0.5f) * TileWidth,
		// ]);
		// // Add visual tile atlases
		// for (int tileType = 0; tileType < TerrainDataArray.Length; tileType++)
		// {
		// 	texture = TerrainDataArray[tileType].Texture;
		// 	atlas = new()
		// 	{
		// 		Texture = texture,
		// 		TextureRegionSize = new(TileWidth, TileWidth)
		// 	};
		// 	tileSet.AddSource(atlas);
		// 	image = texture.GetImage();
		// 	for (int x = 0; x < image.GetWidth() / TileWidth; x++)
		// 	{
		// 		for (int y = 0; y < image.GetHeight() / TileWidth; y++)
		// 		{
		// 			var region = image.GetRegion(new(TileWidth * x, TileWidth * y, TileWidth, TileWidth));
		// 			if(region.IsInvisible())
		// 			{
		// 				// GD.Print($"Skipped invisible region {x},{y}");
		// 				continue;
		// 			}
		// 			Vector2I tileCoord = new(x, y);
		// 			if(!SwMaskLookup.TryGetMask(tileCoord, out var mask))
		// 			{
		// 				GD.PrintErr($"No mask for region {x},{y}");
		// 				continue;
		// 			}
		// 			CoordLookup.AddCoords(mask, tileType, tileCoord);
		// 			atlas.CreateTile(tileCoord);
		// 		}
		// 	}
		// }
		for (int layerIdx = 0; layerIdx < NumLayers; layerIdx++)
		{
			TileMapLayer mapLayer = new()
			{
				Name = $"VisualLayer{layerIdx + 1}"
			};
			AddChild(mapLayer);
			mapLayer.Owner = this;
			mapLayer.TileSet = SharedTileSet;
			mapLayer.Changed += ()=>TilesNeedRefresh = true;
			VisualMapLayers.Add(mapLayer);
		}
	}
	private void FreeChildren()
	{
		CollisionLayer = null;
		VisualMapLayers.Clear();
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
	}
	public bool TryGetTerrainData(int tileType, out SwTerrainData terrainData)
	{
		terrainData = default;
		if(tileType < 0 || tileType >= TerrainDataArray.Length) return false;
		terrainData = TerrainDataArray[tileType];
		return true;
	}
	private void RefreshAllTiles(){}
	// public void QueueRefreshTiles(){CollisionLayerIsDirty = true;}
}
