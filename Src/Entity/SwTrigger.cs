using System.Collections.Generic;
using Godot;
using SW.Src.Global;
using SW.Src.Ui;
using SW.Src.Utils;

namespace SW.Src.Entity;

public partial class SwTrigger : Area2D, ISwEntity
{
	[Export] public string Command;
	[Export] private string[] Filters;
	[Export] private bool SingleUse = false;
	private int TimesUsed = 0;
	private HashSet<string> GroupFilters;
	private CollisionShape2D CollisionShape;
	public override void _Ready()
	{
		CollisionShape = GetChild<CollisionShape2D>(0);
	}
	private void OnBodyEntered(Node2D body)
	{
		if(SingleUse && TimesUsed > 0) return;
		if (Match(body))
		{
			Main.Message(Command);
			TimesUsed++;
			if(SingleUse) QueueFree();
		}
	}
	private bool Match(Node2D body)
	{
		if(GroupFilters is null) return true;
		foreach (var group in body.GetGroups())
		{
			if(GroupFilters.Contains(group)) return true;
		}
		return false;
	}
	public bool TryInit(SwJsonDb db)
	{
		if(!db.TryGetString("Command", out Command)) return false;
		if(!db.TryGetArray("Filters", out Filters)) return false;
		if(!db.TryGetBool("single_use", out SingleUse)) return false;
		if(!db.TryGetNumber("width", out float width)) return false;
		if(!db.TryGetNumber("height", out float height)) return false;
		Vector2 size = new(width, height);
		RectangleShape2D shape = new()
		{
			Size = size,
		};
		CollisionShape.Position = size * 0.5f;
		CollisionShape.Shape = shape;
		if(Filters is not null && Filters.Length > 0) GroupFilters = [..Filters];
		BodyEntered += OnBodyEntered;
		CollisionShape.Disabled = false;
		return true;
	}
}
