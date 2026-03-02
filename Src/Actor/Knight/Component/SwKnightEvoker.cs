using Godot;
using SW.Src.Entity;

namespace SW.Src.Actor.Knight.Component;

public class SwKnightEvoker
{
    private readonly SwKnight Parent;
    private readonly Node2D SwordPivot;
    private readonly SwHurtbox Hurtbox;
    private readonly CollisionShape2D Hitbox;
    public SwKnightEvoker(SwKnight parent)
    {
        Parent = parent;
        SwordPivot = Parent.GetNode<Node2D>("SwordPivot");
        Hurtbox = Parent.GetNode<SwHurtbox>("SwordPivot/Hurtbox");
        Hitbox = Parent.GetNode<CollisionShape2D>("Hitbox");
    }
    public void StartSwordAttack()
    {
        SwordPivot.Rotation = Parent.GetLastAngleRounded();
    }
    public void Die()
    {
        Hurtbox.IsEnabled = false;
        Hitbox.Disabled = true;
    }
    public void HurtboxEnable(bool isEnabled){Hurtbox.IsEnabled = isEnabled;}
}
