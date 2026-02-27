using System;
using SW.Src.Entity;
using SW.Src.Ui.Menu;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerHud : SwComponent<SwPlayer, SwPlayerHud.HudApi>
{
    public record class HudApi
    {
        public required Func<float> GetHealth{get; init;}
        public required Func<float> GetAmmo{get; init;}
    }
    private readonly SwHud Hud;
    public SwPlayerHud(SwPlayer parent, HudApi api) : base(parent, api)
    {
        var nodes = parent.GetTree().GetNodesInGroup("Hud");
        if(nodes.Count == 0) return;
        Hud = nodes[0] as SwHud;
    }
    public override void Tick(float dt)
    {
        if(Hud is null) return;
        Hud.SetHealth(Api.GetHealth());
        Hud.SetAmmo(Api.GetAmmo());
    }
    public void AddMessage(string message, float duration = 1)
    {
        Hud.AddMessage(message, duration);
    }
}
