using Godot;
using SW.Src.Entity;
using SW.Src.Entity.Projectile;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerEvoker
{
    private readonly SwPlayer Parent;
    private readonly Node2D SpoonPivot;
    private readonly SwHurtbox Hurtbox;
    public SwPlayerEvoker(SwPlayer parent)
    {
        Parent = parent;
        SpoonPivot = parent.GetNode<Node2D>("SpoonPivot");
        Hurtbox = parent.GetNode<SwHurtbox>("SpoonPivot/Hurtbox");
    }
    public void StartSpoonAttack()
    {
        Hurtbox.IsEnabled = true;
        SpoonPivot.Rotation = Parent.GetLastAngleRounded();
        Hurtbox.Damages = [..Parent.SpoonDamages];
    }
    public void EndSpoonAttack()
    {
        Hurtbox.IsEnabled = false;
    }
    public void FireSling()
    {
        var bullet = Parent.SlingBulletScene.Instantiate<SwProjectile>();
        bullet.Init(Parent.GetParent(), Parent.Controls.Aim() * Parent.SlingBulletSpeed, Parent.Position);
        Parent.Inventory.RemoveItems(Inventory.SwItemType.SlingBullet, 1);
        bullet.Damages = [..Parent.SlingDamages];
    }
}
