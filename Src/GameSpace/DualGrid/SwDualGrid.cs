using System.Collections.Generic;
using Godot;
using SW.Src.Utils;

namespace SW.Src.GameSpace.DualGrid;

[Tool]
public partial class SwDualGrid : TileMapLayer
{
	// Exports
	private int TileWidth_ = 32;
	[Export] private int TileWidth{get=>TileWidth_; set
		{
			TileWidth_ = value;
			TileSetRebuildFlag.SetDirty();
		}
	}
	private int CurrentTerrain_ = 0;
	[Export] private int CurrentTerrain{get=>CurrentTerrain_; set
		{
			if(value == 0 || IsValidTerrainIdx(value, out _)) CurrentTerrain_ = value;
		}
	}
	[Export] private bool Erase = false;
	private int NumLayers_ = 4;
	[Export] private int NumLayers{get=>NumLayers_; set
		{
			NumLayers_ = value;
			LayerRebuildFlag.SetDirty();
		}
	}
	private int CurrentLayer_ = 0;
	[Export] private int CurrentLayer{get=>CurrentLayer_; set
		{
			if(value == 0 || IsValidLayerIdx(value, out _)) CurrentLayer_ = value;
		}
	}
	[Export] private SwTerrainData[] TerrainDataArray = [];
	// Private data
	private readonly Dictionary<int, int> SourceIdLookup = [];
	private readonly SwTileCoordLookup CoordLookup = new();
	private TileSet SharedTileSet;
	// Tool buttons and dirty flags
	private SwDirtyFlag TileSetRebuildFlag = new();
	// private bool TileSetNeedRebuild = true;
	[ExportToolButton("Rebuild TileSet")]
	public Callable QueueRebuildTileSetButton => Callable.From(()=>TileSetRebuildFlag.SetDirty());
	private SwDirtyFlag LayerRebuildFlag = new();
	// private bool LayersNeedRebuild = true;
	[ExportToolButton("Rebuild Layers")]
	public Callable QueueRebuildLayersButton => Callable.From(()=>LayerRebuildFlag.SetDirty());
	[ExportToolButton("Clear All")]
	public Callable ClearButton => Callable.From(ClearAll);
	// private SwDirtyFlag FillFlag = new(); 
	// Child nodes
	private int BaseSourceId;
	private TileMapLayer CollisionLayer;
	private int CollisionLayerSourceId;
	private readonly List<SwDualGridLayer> VisualMapLayers = [];

	public override void _PhysicsProcess(double delta)
	{
		if(TileSetRebuildFlag.IsDirty())
		{
			TileSetRebuildFlag.Clean();
			LayerRebuildFlag.SetDirty();
			RebuildTileSet();
		}
		if (LayerRebuildFlag.IsDirty())
		{
			if(GetChildCount() > 0) FreeChildren();
			else
			{
				LayerRebuildFlag.Clean();
				RebuildTileLayers();
			}
		}
		UpdateCells();
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
	private void RebuildTileSet()
	{
		GD.Print("Rebuild TileSet");
		TileSet = SwTileMapUtils.InitTileSet(TileWidth_);
		// Add base atlas
		var atlas = SwTileMapUtils.AddAtlas(TileSet, 1, 1, out BaseSourceId);
		SwTileMapUtils.AddTile(atlas, Vector2I.Zero);
		// Add collision atlas
		atlas = SwTileMapUtils.AddAtlas(TileSet, 2, 1, out CollisionLayerSourceId);
		SwTileMapUtils.AddTile(atlas, Vector2I.Zero);
		SwTileMapUtils.AddTile(atlas, Vector2I.Right, true);
		CoordLookup.Clear();
		SourceIdLookup.Clear();
		// Add visual atlases
		for (int terrainIdx = 0; terrainIdx < TerrainDataArray.Length; terrainIdx++)
		{
			if(!TryGetTerrain(terrainIdx, out var terrainData)) return;
			// var terrainData = TerrainDataArray[terrainIdx];
			var texture = terrainData.Texture;
			var image = texture.GetImage();
			atlas = SwTileMapUtils.AddAtlas(TileSet, texture, out int sourceId);
			SourceIdLookup.Add(terrainIdx, sourceId);
			// atlas.GetIdForPath()
			for (int x = 0; x < texture.GetWidth(); x++)
			{
				for (int y = 0; y < texture.GetHeight(); y++)
				{
					var region = image.GetRegion(new(TileWidth_ * x, TileWidth_ * y, TileWidth_, TileWidth_));
					if(region.IsInvisible()) continue;
					Vector2I tileCoord = new(x, y);
					if(!SwMaskLookup.TryGetMask(tileCoord, out var mask))
					{
						GD.PrintErr($"No mask for region {x},{y}");
						continue;
					}
					CoordLookup.AddCoords(mask, sourceId, tileCoord);
					atlas.CreateTile(tileCoord);
				}
			}
		}
	}
	private void RebuildTileLayers()
	{
		GD.Print("Rebuild TileGridLayers");
		VisualMapLayers.Clear();
		CollisionLayer = new()
		{
			Name = "CollisionLayer"
		};
		AddChild(CollisionLayer);
		CollisionLayer.Owner = this;
		CollisionLayer.TileSet = TileSet;
		for (int layerIdx = 0; layerIdx < NumLayers_; layerIdx++)
		{
			SwDualGridLayer mapLayer = new()
			{
				Name = $"VisualLayer{layerIdx + 1}"
			};
			AddChild(mapLayer);
			mapLayer.Owner = this;
			mapLayer.Position = Vector2.One * TileWidth_ * -0.5f;
			mapLayer.TileSet = TileSet;
			mapLayer.Init(CoordLookup);
			VisualMapLayers.Add(mapLayer);
		}
	}
	private void UpdateCells()
	{
		// Refresh each modified tile in the base layer, then clear self
		var usedCells = GetUsedCells();
		if(usedCells.Count == 0) return;
		// GD.Print($"Updating {usedCells.Count} Cells");
		if(!IsValidLayerIdx(CurrentLayer, out var layer)) return;
		foreach (var tilePos in GetUsedCells())
		{
			SetTile(tilePos, layer, CurrentTerrain);
		}
		Clear();
	}
	private void SetTile(Vector2I tilePos, SwDualGridLayer layer, int terrainIdx)
	{
		if(Erase || !SourceIdLookup.TryGetValue(terrainIdx, out int sourceId))
		{
			ClearTile(tilePos, layer);
			return;
		}
		// Look up terrain data
		if(!IsValidTerrainIdx(terrainIdx, out var terrainData)) return;
		// Set collision to match
		CollisionLayer.SetCell(tilePos, CollisionLayerSourceId, terrainData.MovementMul == 0 ? Vector2I.Right : Vector2I.Zero);
		// Set tile on child visual layer
		layer.SetTile(tilePos, sourceId);
	}
	private void ClearTile(Vector2I tilePos, SwDualGridLayer layer)
	{
		CollisionLayer?.SetCell(tilePos);
		layer?.SetTile(tilePos);
	}
	private bool TryGetTerrain(int terrainIdx, out SwTerrainData terrainData)
	{
		terrainData = default;
		if(TerrainDataArray is null || 
			TerrainDataArray.Length == 0 || 
			terrainIdx < 0 ||
			terrainIdx >= TerrainDataArray.Length) return false;
		terrainData = TerrainDataArray[terrainIdx];
		return terrainData is not null;
	}
	private bool TryGetLayer(int layerIdx, out SwDualGridLayer layer)
	{
		layer = default;
		if(VisualMapLayers is null || 
			VisualMapLayers.Count == 0 || 
			layerIdx < 0 ||
			layerIdx >= VisualMapLayers.Count) return false;
		layer = VisualMapLayers[layerIdx];
		return layer is not null;
	}
	private bool IsValidTerrainIdx(int terrainIdx, out SwTerrainData terrainData)
	{
		if(TryGetTerrain(terrainIdx, out terrainData)) return true;
		GD.PrintErr($"Attempted to use an invalid terrain index '{terrainIdx}'");
		return false;
	}
	private bool IsValidLayerIdx(int layerIdx, out SwDualGridLayer layer)
	{
		if(TryGetLayer(layerIdx, out layer)) return true;
		GD.PrintErr($"Attempted to use an invalid layer index '{layerIdx}'");
		return false;
	}
	public void SetTile(Vector2I tilePos, int layerIdx, int terrainIdx)
	{
		if(IsValidLayerIdx(layerIdx, out var layer)) SetTile(tilePos, layer, terrainIdx);
	}
	public void ClearTile(Vector2I tilePos, int layerIdx)
	{
		if(IsValidLayerIdx(layerIdx, out var layer)) ClearTile(tilePos, layer);
	}
	public void ClearAll()
	{
		Clear();
		CollisionLayer.Clear();
		foreach (var layer in VisualMapLayers)
		{
			layer.ClearContents();
		}
	}
}
