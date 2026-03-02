using System.Collections.Generic;
using Godot;
using SW.Src.GameSpace.DualGrid;
using SW.Src.Global;
using SW.Src.Utils;

namespace SW.Src.GameSpace.Dungeon;

public partial class SwDungeon : Node2D
{
	[Export(PropertyHint.File, "*.json")] private string MapPath;
	[Export] private PackedScene[] EntityScenes = [];
	private readonly Dictionary<string, PackedScene> EntityLookup = [];
	private SwDualGrid Terrain;
	public override void _Ready()
	{
		foreach (var scene in EntityScenes)
		{
			string name = scene.ResourcePath.Split('/')[^1].Split('.')[0];
			// SwStatic.Log(name);
			EntityLookup.Add(name, scene);
		}
		SwFs fs = new();
		if(!fs.TryReadJsonRaw(MapPath, out var json)) return;
		SwJsonDb db = new(json);
		Terrain = GetNode<SwDualGrid>("Terrain");
		if(!db.TryGetDbArray("levels", out var levels)) return;
		foreach (var level in levels)
		{
			if(!level.TryGetDbArray("layerInstances", out var layers)) return;
			foreach (var layer in layers)
			{
				if(!layer.TryGetString("__type", out string layerType)) return;
				if(layerType == "IntGrid") AddGridLayer(level, layer);
				else if(layerType == "Entities") AddEntLayer(layer);
				else SwStatic.LogError("poo");
			}
		}
	}
	private void AddGridLayer(SwJsonDb level, SwJsonDb layer)
	{
		layer.TryGetNumber("__cWid", out int width);
		// layer.TryGetNumber("__cHei", out int height);
		layer.TryGetNumber("__gridSize", out int gridSize);
		level.TryGetNumber("worldX", out int pxWorldX);
		level.TryGetNumber("worldY", out int pxWorldY);
		layer.TryGetString("__identifier", out string identifier);
		identifier = identifier.Split("IntGrid")[1];
		int layerIdx = int.Parse(identifier);
		int worldX = pxWorldX / gridSize;
		int worldY = pxWorldY / gridSize;
		layer.TryGetArray("intGridCsv", out var cells);
		for (int idx = 0; idx < cells.Count; idx++)
		{
			cells[idx].AsValue().TryGetValue(out int cell);
			if(cell == 0) continue;
			cell--;
			int x = idx % width + worldX;
			int y = idx / width + worldY;
			Terrain.SetTiles([new(x,y)], layerIdx, cell);
		}
	}
	private void AddEntLayer(SwJsonDb layer)
	{
		if(!layer.TryGetDbArray("entityInstances", out var entities)) return;
		foreach (var entity in entities)
		{
			if(!TryGetScene(entity, out var scene)) continue;
			entity.TryGetNumber("__worldX", out float x);
			entity.TryGetNumber("__worldY", out float y);
			var ent = scene.Instantiate<Node2D>();
			AddChild(ent);
			ent.Position = new(x, y);
		}
	}
	private bool TryGetScene(SwJsonDb entity, out PackedScene scene)
	{
		scene = default;
		entity.TryGetString("__identifier", out string identifier);
		string name = "";
		var fields = GetEntityFields(entity);
		switch (identifier)
		{
			case "PlayerSpawner":
			name = "player";
			break;
			case "ActorSpawner":
			case "EntitySpawner":
			if(!fields.TryGetString("Type", out name)) return false;
			break;
			default:
			return false;
		}
		return EntityLookup.TryGetValue(name, out scene);
	}
	private static SwJsonDb GetEntityFields(SwJsonDb entity)
	{
		SwJsonDb res = new();
		entity.TryGetDbArray("fieldInstances", out var fields);
		foreach (var field in fields)
		{
			field.TryGetString("__identifier", out string identifier);
			field.TryGetPath("__value", out var value);
			res.TrySetPath(identifier, value.DeepClone());
		}
		return res;
	}
}
