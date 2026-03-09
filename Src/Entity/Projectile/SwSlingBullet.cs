using Godot;
using SW.Src.Effect;
using SW.Src.GameSpace.Dungeon;

namespace SW.Src.Entity.Projectile;
public partial class SwSlingBullet : SwProjectile
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		if(!SwDungeon.TryGetTerrainAtPos(Position, out var terrain)) return;
		if(terrain.IsSolid) QueueFree();
    }
}
