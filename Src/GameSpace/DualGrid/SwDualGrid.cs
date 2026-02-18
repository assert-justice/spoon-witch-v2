using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.DualGrid;

[Tool]
public partial class SwDualGrid : TileMapLayer
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
	// [Export] private TileSet GridTileSet;
	// Dirty flags
	private bool TileSetNeedRebuild = false;
	// [Export] private bool QueueRebuildTileSetButton {get=>TileSetNeedRebuild; set{TileSetNeedRebuild = value;}}
	[ExportToolButton("Rebuild TileSet")]
	public Callable QueueRebuildTileSetButton => Callable.From(()=>TileSetNeedRebuild = true);
	private bool LayersNeedRebuild = false;
	[ExportToolButton("Rebuild Layers")]
	public Callable QueueRebuildLayersButton => Callable.From(()=>LayersNeedRebuild = true);
	private bool TilesNeedRefresh = false;
	// [ExportToolButton("Refresh Tiles")]
	// public Callable QueueRefreshTilesButton => Callable.From(()=>TilesNeedRefresh = true);
	// Children
	// private TileSet SharedTileSet;
	private TileMapLayer CollisionLayer;
	private readonly List<TileMapLayer> VisualMapLayers = [];
	private readonly SwTileCoordLookup CoordLookup = new();
	public override void _Ready()
	{
		// Changed += ()=>TilesNeedRefresh = true;
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
		// if(TileSet is null)
		// {
		// 	TileSetNeedRebuild = true;
		// 	TileSet = SwTileMapUtils.InitTileSet(TileWidth);
		// }
		if(TileSetNeedRebuild)
		{
			TileSetNeedRebuild = false;
			LayersNeedRebuild = true;
			RebuildTileSet();
		}
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
		TileSet = SwTileMapUtils.InitTileSet(TileWidth);
		// Add base atlas
		var atlas = SwTileMapUtils.AddAtlas(TileSet, 1, 1);
		SwTileMapUtils.AddTile(atlas, Vector2I.Zero);
		// Add collision atlas
		atlas = SwTileMapUtils.AddAtlas(TileSet, 2, 1);
		SwTileMapUtils.AddTile(atlas, Vector2I.Zero);
		SwTileMapUtils.AddTile(atlas, Vector2I.Right, true);
		// Add visual atlases
		for (int tileType = 0; tileType < TerrainDataArray.Length; tileType++)
		{
			var texture = TerrainDataArray[tileType].Texture;
			var image = texture.GetImage();
			atlas = SwTileMapUtils.AddAtlas(TileSet, texture);
			for (int x = 0; x < texture.GetWidth(); x++)
			{
				for (int y = 0; y < texture.GetHeight(); y++)
				{
					var region = image.GetRegion(new(TileWidth * x, TileWidth * y, TileWidth, TileWidth));
					if(region.IsInvisible()) continue;
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
	}
	private void RebuildTileLayers()
	{
		GD.Print("RebuildTileLayers");
		CollisionLayer = new()
		{
			Name = "CollisionLayer"
		};
		AddChild(CollisionLayer);
		CollisionLayer.Owner = this;
		CollisionLayer.TileSet = TileSet;
		for (int layerIdx = 0; layerIdx < NumLayers; layerIdx++)
		{
			TileMapLayer mapLayer = new()
			{
				Name = $"VisualLayer{layerIdx + 1}"
			};
			AddChild(mapLayer);
			mapLayer.Owner = this;
			mapLayer.Position = Vector2.One * TileWidth * -0.5f;
			mapLayer.TileSet = TileSet;
			VisualMapLayers.Add(mapLayer);
		}
	}
	private void FreeChildren()
	{
		Clear();
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
	private void RefreshAllTiles()
	{
		// For each modified tile in the base layer
		// Go through each visual layer and see if it's solid
		// Then update the collision layer accordingly
	}
}
