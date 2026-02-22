using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using SW.Src.GameSpace.Terrain;

namespace SW.Src.GameSpace.DualGrid;

[Tool]
public partial class SwDualGrid : TileMapLayer
{
	[Export] private SwTerrainRes TerrainData;
	[Export] private int NumLayers = 0; //{get=>GetNumLayers(); set=>SetNumLayers(value);}
	SwTileCoordLookup TileCoordLookup;
	private int CurrentLayer_ = 0;
	[Export] private int CurrentLayer{get=>CurrentLayer_; set
		{
			if(value < 0 || value >= NumLayers) return;
			CurrentLayer_ = value;
		}
	}
	private int CurrentTerrainType_ = 0;
	[Export] private int CurrentTerrainType{get=>CurrentTerrainType_; set
		{
			if(value < 0 || value >= (TerrainData?.TerrainTypes?.Length ?? 0)) return;
			CurrentTerrainType_ = value;
		}
	}
	[Export] private bool EraseMode = false;
	[ExportToolButton("Clear")]
	public Callable ClearAllButton => Callable.From(ClearAll);
	private TileMapLayer CollisionLayer;
	public override void _Ready()
	{
		TileSet = TerrainData.BaseTileSet;
		CollisionLayer = GetNodeOrNull<TileMapLayer>("CollisionLayer");
		if(CollisionLayer is null)
		{
			CollisionLayer = new()
			{
				Name = "CollisionLayer"
			};
			AddChild(CollisionLayer);
			CollisionLayer.Owner = this;
		}
		CollisionLayer.TileSet = TerrainData.CollisionTileSet;
		SetTerrainData();
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			if(CollisionLayer is null) return;
			if(NumLayers != GetNumLayers()) SetNumLayers(NumLayers);
			Update();
		}
	}
	private int GetNumLayers()
	{
		return CollisionLayer.GetChildCount();
	}
	private void SetNumLayers(int numLayers)
	{
		int currentNumLayers = GetNumLayers();
		if(currentNumLayers < numLayers)
		{
			PushLayer(numLayers - currentNumLayers);
		}
		else if(currentNumLayers > numLayers)
		{
			PopLayer(currentNumLayers - numLayers);
		}
	}
	private void PushLayer(int numNewLayers = 1)
	{
		GD.Print($"pushing {numNewLayers} layers");
		int currentNumLayers = GetNumLayers();
		for (int idx = 0; idx < currentNumLayers; idx++)
		{
			CollisionLayer.GetChild(idx).Name = $"Layer{idx}";
		}
		for (int idx = 0; idx < numNewLayers; idx++)
		{
			SwDualGridLayer layer = new()
			{
				TileSet = TerrainData.DisplayTileSet,
				Name = $"Layer{idx + currentNumLayers}",
				Position = new Vector2(TerrainData.TileWidth, TerrainData.TileHeight) * -0.5f,
			};
			CollisionLayer.AddChild(layer);
			layer.Owner = this;
		}
	}
	private void PopLayer(int numRemovedLayers = 1)
	{
		GD.Print($"popping {numRemovedLayers} layers");
		int numLayers = GetNumLayers();
		for (int layerIdx = 0; layerIdx < numRemovedLayers; layerIdx++)
		{
			var child = CollisionLayer.GetChild(numLayers-layerIdx-1);
			CollisionLayer.RemoveChild(child);
			child.QueueFree();
		}
	}
	private IEnumerable<SwDualGridLayer> GetLayersR()
	{
		int numLayers = CollisionLayer.GetChildCount();
		for (int idx = 0; idx < numLayers; idx++)
		{
			yield return CollisionLayer.GetChild<SwDualGridLayer>(numLayers - idx - 1);
		}
	}
	private IEnumerable<SwDualGridLayer> GetLayers()
	{
		foreach (var child in CollisionLayer.GetChildren())
		{
			if(child is SwDualGridLayer layer) yield return layer;
			else
			{
				GD.PrintErr("Expected child to be a SwDualGridLayer");
				break;
			}
		}
	}
	private bool TryGetDisplayLayer(int layerIdx, out SwDualGridLayer layer)
	{
		layer = default;
		layer = CollisionLayer.GetChildOrNull<SwDualGridLayer>(layerIdx);
		return layer is not null;
	}
	private void SetTerrainData()
	{
		foreach (var layer in GetLayers())
		{
			layer.TileSet = TerrainData.DisplayTileSet;
		}
	}
	private SwTileCoordLookup GetTileCoordLookup()
	{
		if(TileCoordLookup is not null) return TileCoordLookup;
		SwTileCoordLookup coordLookup = new();
		for (int idx = 0; idx < TerrainData.DisplayTileSet.GetSourceCount(); idx++)
		{
			var source = TerrainData.DisplayTileSet.GetSource(idx);
			if(source is not TileSetAtlasSource atlas) continue;
			for (int fdx = 0; fdx < atlas.GetTilesCount(); fdx++)
			{
				Vector2I tilePos = atlas.GetTileId(fdx);
				if(!SwTerrainUtils.TryGetMask(tilePos, out var mask))
				{
					GD.PrintErr($"Probably unreachable");
					continue;
				}
				coordLookup.AddCoords(mask, idx, tilePos);
			}
		}
		TileCoordLookup = coordLookup;
		return coordLookup;
	}
	private void Update()
	{
		if(!TryGetDisplayLayer(CurrentLayer_, out var layer)) return;
		var updated = GetUsedCells();
		if(updated.Count == 0)return;
		// GD.Print($"Updating {updated.Count} tiles");
		if (EraseMode)
		{
			foreach (var cellPos in updated)
			{
				layer.ClearTile(cellPos);
				if(IsSolid(cellPos)) CollisionLayer.SetCell(cellPos, 0, Vector2I.Right);
				else CollisionLayer.SetCell(cellPos);
			}
		}
		else
		{
			foreach (var cellPos in updated)
			{
				layer.SetTile(cellPos, CurrentTerrainType_);
				if(IsSolid(cellPos)) CollisionLayer.SetCell(cellPos, 0, Vector2I.Right);
				else CollisionLayer.SetCell(cellPos);
			}
		}
		Clear();
	}
	public bool IsSolid(Vector2I tilePos)
	{
		foreach (var layer in GetLayersR())
		{
			int tileId = layer.GetTileId(tilePos);
			if(tileId == -1) continue;
			if(TerrainData.TerrainTypes[tileId].GetIsSolid()) return true;
		}
		return false;
	}
	public bool TryGetAtlasCoords((bool, bool, bool, bool) mask, int sourceId, out Vector2I atlasCoords)
	{
		return GetTileCoordLookup().TryGetAtlasCoords(mask, sourceId, out atlasCoords);
	}
	public void ClearAll()
	{
		Clear();
		CollisionLayer.Clear();
		int numLayers = NumLayers;
		SetNumLayers(0);
		SetNumLayers(numLayers);
	}
}
