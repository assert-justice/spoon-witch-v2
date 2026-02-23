using SW.Src.Effect;

namespace SW.Src.Entity.Projectile;
public partial class SwSlingBullet : SwProjectile
{
    public override void _Ready()
    {
        base._Ready();
        GroupWhitelist.Add("Player");
        Damages = [
            new(SwDamageType.Untyped, 1),
        ];
    }
}
