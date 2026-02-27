using System;
using SW.Src.Inventory;

namespace SW.Src.Actor.Player;

public partial class SwPlayer
{
    private void BindHud()
    {
        Hud = new(this, new(){GetHealth=GetHealth, GetAmmo=()=>Inventory.CountItems(SwItemType.SlingBullet)});
    }
    private void BindComponents()
    {
        BindHud();
    }
}
