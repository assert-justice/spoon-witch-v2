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
	SwTileCoordLookup tileCoordLookup;
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
	public override void _Ready()
	{
		TileSet = TerrainData.BaseTileSet;
		if(TryGetCollisionLayer(out var collisionLayer)) collisionLayer.TileSet = TerrainData.CollisionTileSet;
		SetTerrainData();
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			// UpdateNumTerrainTypes();
			int numLayersCurrent = GetNumLayers();
			if(numLayersCurrent != NumLayers)
			{
				SetNumLayers(NumLayers);
			}
			Update();
		}
	}

	private bool TryGetCollisionLayer(out TileMapLayer collisionLayer)
	{
		collisionLayer = default;
		if(GetChild(0) is TileMapLayer layer)
		{
			collisionLayer = layer;
			return true;
		}
		GD.PrintErr("Invalid or absent collision layer");
		return false;
	}
	private int GetNumLayers()
	{
		if(!TryGetCollisionLayer(out var collisionLayer)) return 0;
		return collisionLayer.GetChildCount();
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
		if(!TryGetCollisionLayer(out var collisionLayer)) return;
		GD.Print($"pushing {numNewLayers} layers");
		int currentNumLayers = GetNumLayers();
		for (int idx = 0; idx < currentNumLayers; idx++)
		{
			collisionLayer.GetChild(idx).Name = $"Layer{idx}";
		}
		for (int idx = 0; idx < numNewLayers; idx++)
		{
			SwDualGridLayer layer = new()
			{
				TileSet = TerrainData.DisplayTileSet,
				Name = $"Layer{idx + currentNumLayers}",
			};
			collisionLayer.AddChild(layer);
			layer.Owner = this;
		}
	}
	private void PopLayer(int numRemovedLayers = 1)
	{
		if(!TryGetCollisionLayer(out var collisionLayer)) return;
		GD.Print($"popping {numRemovedLayers} layers");
		int numLayers = GetNumLayers();
		for (int layerIdx = 0; layerIdx < numRemovedLayers; layerIdx++)
		{
			var child = collisionLayer.GetChild(numLayers-layerIdx-1);
			collisionLayer.RemoveChild(child);
			child.QueueFree();
		}
	}
	private SwDualGridLayer[] GetLayers()
	{
		if(!TryGetCollisionLayer(out var collisionLayer)) return [];
		List<SwDualGridLayer> layers = new(collisionLayer.GetChildCount());
		var children = collisionLayer.GetChildren();
		for (int layerIdx = 0; layerIdx < children.Count; layerIdx++)
		{
			var child = children[layerIdx];
			if(child is SwDualGridLayer layer) layers.Add(layer);
			else
			{
				GD.PrintErr($"Layer {layerIdx} is not a DualGridLayer");
				return [];
			}
		}
		return [..layers];
	}
	private bool TryGetDisplayLayer(int layerIdx, out SwDualGridLayer layer)
	{
		layer = default;
		if(!TryGetCollisionLayer(out var collisionLayer)) return false;
		layer = collisionLayer.GetChildOrNull<SwDualGridLayer>(layerIdx);
		return layer is not null;
	}
	private void SetTerrainData()
	{
		foreach (var layer in GetLayers())
		{
			layer.TileSet = TerrainData.DisplayTileSet;
		}
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
			}
		}
		else
		{
			foreach (var cellPos in updated)
			{
				layer.SetTile(cellPos, CurrentTerrainType_);
			}
		}
		foreach (var item in layer.GetUsedCells())
		{
			GD.Print(item);
		}
		Clear();
	}
	public bool TryGetAtlasCoords((bool, bool, bool, bool) mask, int sourceId, out Vector2I atlasCoords)
	{
		atlasCoords = default;
		return TerrainData?.TileCoordLookup?.TryGetAtlasCoords(mask, sourceId, out atlasCoords)??false;
	}
}
