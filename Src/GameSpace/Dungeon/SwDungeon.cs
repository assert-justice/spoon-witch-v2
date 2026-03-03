using System.Collections.Generic;
using Godot;
using SW.Src.Entity;
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
		if(!db.TryGetArray("levels", out SwJsonDb[] levels)) return;
		foreach (var level in levels)
		{
			if(!level.TryGetArray("layerInstances", out SwJsonDb[] layers)) return;
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
		layer.TryGetArray("intGridCsv", out int[] cells);
		for (int idx = 0; idx < cells.Length; idx++)
		{
			int cell = cells[idx];
			if(cell == 0) continue;
			cell--;
			int x = idx % width + worldX;
			int y = idx / width + worldY;
			Terrain.SetTiles([new(x,y)], layerIdx, cell);
		}
	}
	private void AddEntLayer(SwJsonDb layer)
	{
		if(!layer.TryGetArray("entityInstances", out SwJsonDb[] entities)) return;
		foreach (var entityDb in entities)
		{
			TrySpawnEntity(entityDb);
		}
	}
	private static SwJsonDb GetEntityFields(SwJsonDb entityDb)
	{
		SwJsonDb res = new();
		entityDb.TryGetArray("fieldInstances", out SwJsonDb[] fields);
		foreach (var field in fields)
		{
			field.TryGetString("__identifier", out string identifier);
			field.TryGetPath("__value", out var value);
			res.TrySetPath(identifier, value.DeepClone());
		}
		return res;
	}
	private bool TrySpawnEntity(SwJsonDb entityDb)
	{
		var fields = GetEntityFields(entityDb);
		string sceneName;
		entityDb.TryGetString("__identifier", out string identifier);
		entityDb.TryGetNumber("__worldX", out float posX);
		entityDb.TryGetNumber("__worldY", out float posY);
		entityDb.TryGetNumber("width", out float width);
		entityDb.TryGetNumber("height", out float height);
		fields.TrySetPath("width", width);
		fields.TrySetPath("height", height);
		switch (identifier)
		{
			case "PlayerSpawner":
				sceneName = "player";
			break;
			case "ActorSpawner":
			case "EntitySpawner":
				if(!fields.TryGetString("Type", out sceneName)) return false;
			break;
			case "Trigger":
				sceneName = "trigger";
			break;
			default:
			return false;
		}
		if(!EntityLookup.TryGetValue(sceneName, out var scene)) return false;
		var entity = scene.Instantiate<Node2D>();
		AddChild(entity);
		entity.Position = new(posX, posY);
		if(entity is ISwEntity ent) return ent.TryInit(fields);
		return true;
	}
}
