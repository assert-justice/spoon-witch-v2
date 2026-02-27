using System;
using Godot;
using SW.Src.Entity;
using SW.Src.Ui;
using SW.Src.Ui.Menu;

namespace SW.Src.Actor.Player.Component;

public class SwPlayerHud : SwComponent<SwPlayer, SwPlayerHud.PlayerHudApi>
{
    public record class PlayerHudApi
    {
        public required Func<float> GetHealth{get; init;}
        public required Func<float> GetAmmo{get; init;}
    }
    private readonly SwHud Hud;
    public SwPlayerHud(SwPlayer parent, PlayerHudApi api) : base(parent, api)
    {
        Main.TryGetHud(out Hud);
    }
    public override void Tick(float dt)
    {
        if(Hud is null) return;
        Hud.UpdatePlayer(Parent);
    }
    public void AddMessage(string message, float duration = 2)
    {
        if(Hud is null) return;
        Hud.AddMessage(message, duration);
    }
}
