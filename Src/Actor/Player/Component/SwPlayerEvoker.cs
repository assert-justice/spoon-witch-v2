using Godot;
using SW.Src.Entity;
using SW.Src.Entity.Projectile;
using SW.Src.Global;

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
        Hurtbox.DamageList = [..Parent.SpoonDamages];
    }
    public void EndSpoonAttack()
    {
        Hurtbox.IsEnabled = false;
    }
    public void FireSling()
    {
        if(!Parent.Inventory.Slots.TryGetValue(Inventory.SwItemType.SlingBullet, out var slot))
        {
            SwStatic.LogError("Should be unreachable");
            return;
        }
        float count = 1;
        if(!slot.TryRemoveItems(ref count))
        {
            SwStatic.LogError("Should be unreachable", count);
            return;
        }
        var bullet = Parent.SlingBulletScene.Instantiate<SwProjectile>();
        bullet.Init(Parent.GetParent(), Parent.Controls.Aim() * Parent.SlingBulletSpeed, Parent.Position);
        bullet.DamageList = [..Parent.SlingDamages];
    }
}
