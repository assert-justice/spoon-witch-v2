using Godot;
using SW.Src.GameSpace.DualGrid;

namespace SW.Src.GameSpace.Dungeon;

public partial class SwDungeon : Node2D
{
	private SwDualGrid Terrain;
	public override void _Ready()
	{
		Terrain = GetNode<SwDualGrid>("Terrain");
		Terrain.SetRect(new(0,0,20,10), 0, 4);
		Terrain.SetRect(new(1,1,18,8), 1, 0);
	}
}
