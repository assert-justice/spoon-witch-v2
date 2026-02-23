using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using SW.Src.GameSpace.Terrain;
using SW.Src.Global;

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
	private bool EditorEnabled = false;
	// private bool EditorEnabled_ = false;
	// [Export] private bool EditorEnabled{get=>EditorEnabled_; set
	// 	{
	// 		EnableEditor(value);
	// 		EditorEnabled_ = value;
	// 	}
	// }
	private TileMapLayer CollisionLayer;
	public override void _Ready()
	{
		// EnableEditor(true);
		TileSet = TerrainData.BaseTileSet;
		CollisionLayer = GetNodeOrNull<TileMapLayer>("CollisionLayer");
		if(CollisionLayer is null)
		{
			CollisionLayer = new()
			{
				Name = "CollisionLayer"
			};
			AddChild(CollisionLayer);
			CollisionLayer.Owner = GetTree().Root;
		}
		CollisionLayer.TileSet = TerrainData.CollisionTileSet;
		SetNumLayers(NumLayers);
		SetTerrainData();
	}

	// public override void _Process(double delta)
	// {
	// 	if (Engine.IsEditorHint())
	// 	{
	// 		if(!EditorEnabled || CollisionLayer is null) return;
	// 		if(NumLayers != GetNumLayers()) SetNumLayers(NumLayers);
	// 		Update();
	// 	}
	// }
	// private void EnableEditor(bool enabled)
	// {
	// 	if(!enabled) return;
	// 	TileSet = TerrainData.BaseTileSet;
	// 	CollisionLayer = GetNodeOrNull<TileMapLayer>("CollisionLayer");
	// 	if(CollisionLayer is null)
	// 	{
	// 		CollisionLayer = new()
	// 		{
	// 			Name = "CollisionLayer"
	// 		};
	// 		AddChild(CollisionLayer);
	// 		CollisionLayer.Owner = GetTree().CurrentScene;
	// 	}
	// 	CollisionLayer.TileSet = TerrainData.CollisionTileSet;
	// 	SetNumLayers(NumLayers);
	// 	SetTerrainData();
	// }
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
		SwStatic.Log($"pushing {numNewLayers} layers");
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
			layer.Owner = GetTree().CurrentScene;
		}
	}
	private void PopLayer(int numRemovedLayers = 1)
	{
		SwStatic.Log($"popping {numRemovedLayers} layers");
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
				SwStatic.LogError("Expected child to be a SwDualGridLayer");
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
	private bool IsValidTerrainIdx(int terrainTypeIdx)
	{
		return terrainTypeIdx >= 0 && terrainTypeIdx < TerrainData.TerrainTypes.Length;
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
					SwStatic.LogError($"Probably unreachable");
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
		// SwStatic.Log($"Updating {updated.Count} tiles");
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
			return TerrainData.TerrainTypes[tileId].GetIsSolid();
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
	public void SetTiles(Vector2I[] tilePositions, int layerIdx, int terrainTypeIdx)
	{
		if(!TryGetDisplayLayer(layerIdx, out var layer))
		{
			SwStatic.LogError($"Invalid layer index '{layerIdx}'");
			return;
		}
		if (!IsValidTerrainIdx(terrainTypeIdx))
		{
			SwStatic.LogError($"Invalid terrain type index '{terrainTypeIdx}'");
			return;
		}
		foreach (var tilePos in tilePositions)
		{
			layer.SetTile(tilePos, terrainTypeIdx);
			if(IsSolid(tilePos)) CollisionLayer.SetCell(tilePos, 0, Vector2I.Right);
			else CollisionLayer.SetCell(tilePos);
		}
	}
	public void SetRect(Rect2I rect, int layerIdx, int terrainTypeIdx)
	{
		List<Vector2I> vectors = new(rect.Size.X * rect.Size.Y);
		for (int x = 0; x < rect.Size.X; x++)
		{
			for (int y = 0; y < rect.Size.Y; y++)
			{
				vectors.Add(new(x + rect.Position.X, y + rect.Position.Y));
			}
		}
		SetTiles([..vectors], layerIdx, terrainTypeIdx);
	}
	public void ClearTiles(Vector2I[] tilePositions, int layerIdx)
	{
		if(!TryGetDisplayLayer(layerIdx, out var layer))
		{
			SwStatic.LogError($"Invalid layer index '{layerIdx}'");
			return;
		}
		foreach (var tilePos in tilePositions)
		{
			layer.ClearTile(tilePos);
			if(IsSolid(tilePos)) CollisionLayer.SetCell(tilePos, 0, Vector2I.Right);
			else CollisionLayer.SetCell(tilePos);
		}
	}
	public void ClearRect(Rect2I rect, int layerIdx)
	{
		List<Vector2I> vectors = new(rect.Size.X * rect.Size.Y);
		for (int x = 0; x < rect.Size.X; x++)
		{
			for (int y = 0; y < rect.Size.Y; y++)
			{
				vectors.Add(new(x + rect.Position.X, y + rect.Position.Y));
			}
		}
		ClearTiles([..vectors], layerIdx);
	}
}
