using Godot;
using SW.Src.Entity.Projectile;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerEvoker(SwPlayer parent)
{
    private readonly SwPlayer Parent = parent;
    private readonly Node2D SpoonPivot = parent.GetNode<Node2D>("SpoonPivot");
    private readonly Area2D Hurtbox = parent.GetNode<Area2D>("SpoonPivot/Hurtbox");
    private readonly CollisionShape2D HurtboxCollision = parent.GetNode<CollisionShape2D>("SpoonPivot/Hurtbox/CollisionShape2D");
    public void StartSpoonAttack()
    {
        HurtboxCollision.Disabled = false;
        SpoonPivot.Rotation = Parent.GetLastAngleRounded();
    }
    public void EndSpoonAttack()
    {
        HurtboxCollision.Disabled = true;
    }
    public void FireSling()
    {
        var bullet = Parent.SlingBulletScene.Instantiate<SwSlingBullet>();
        bullet.Init(Parent.GetParent(), Parent.Controls.Aim() * Parent.SlingBulletSpeed, Parent.Position);
        Parent.Inventory.RemoveItems(Inventory.SwItemType.SlingBullet, 1);
    }
}
