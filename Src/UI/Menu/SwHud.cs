using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SW.Src.Actor.Player;
using SW.Src.Global;
using SW.Src.Utils;

namespace SW.Src.Ui.Menu;

public partial class SwHud : SwMenu
{
    [Export] private int MaxVisibleMessages = 5;
    private readonly Queue<(string,float)> MessageQueue = new();
    private readonly Queue<Action<SwHud>> DrawQueue = new();
    private Label HealthLabel;
    private Label AmmoLabel;
    private Label RootLabel;
    private VBoxContainer MessageContainer;
    private readonly SwDirtyWrapper<float> PlayerHealth = new(100);
    private readonly SwDirtyWrapper<float> PlayerMaxHealth = new(100);
    private readonly SwDirtyWrapper<float> PlayerRoots = new(100);
    private readonly SwDirtyWrapper<float> PlayerMaxRoots = new(100);
    private readonly SwDirtyWrapper<float> PlayerAmmo = new(5);
    private readonly SwDirtyWrapper<float> PlayerMaxAmmo = new(5);
    public override void _Ready()
    {
        HealthLabel = GetNode<Label>("VBox/HBox/Health");
        AmmoLabel = GetNode<Label>("VBox/HBox/Ammo");
        RootLabel = GetNode<Label>("VBox/HBox/Roots");
        MessageContainer = GetNode<VBoxContainer>("VBox/Messages");
        SwStatic.FreeChildren(MessageContainer);
    }
    public override void _PhysicsProcess(double delta)
    {
        if(PlayerHealth.IsDirty() || PlayerMaxHealth.IsDirty()) 
            HealthLabel.Text = $"Health: {PlayerHealth.Value}/{PlayerMaxHealth.Value}";
        if(PlayerAmmo.IsDirty() || PlayerMaxAmmo.IsDirty()) 
            AmmoLabel.Text = $"Ammo: {PlayerAmmo.Value}/{PlayerMaxAmmo.Value}";
        if(PlayerRoots.IsDirty() || PlayerMaxRoots.IsDirty()) 
            RootLabel.Text = $"Roots: {PlayerRoots.Value}/{PlayerMaxRoots.Value}";
        UpdateMessages();
        // Deliberately does not call base physics process method
    }
    public override void _Draw()
    {
        while(DrawQueue.TryDequeue(out var action)) action(this);
    }
    private void UpdateMessages()
    {
        if(MessageContainer.GetChildCount() >= MaxVisibleMessages) return;
        if(!MessageQueue.TryDequeue(out var result)) return;
        var(message, duration) = result;
        SwMessageLabel messageLabel = new()
        {
            Text = message,
            Duration = duration
        };
        MessageContainer.AddChild(messageLabel);
    }
    protected override bool ShouldPauseOnWake()
    {
        return false;
    }
    public void UpdatePlayer(SwPlayer player)
    {
        PlayerHealth.Value = player.GetHealth();
        PlayerMaxHealth.Value = player.MaxHealth;
        if(player.Inventory.Slots.TryGetValue(Inventory.SwItemType.SlingBullet, out var ammoSlot))
        {
            PlayerAmmo.Value = ammoSlot.Quantity;
            PlayerMaxAmmo.Value = ammoSlot.Capacity;
        }
        if(player.Inventory.Slots.TryGetValue(Inventory.SwItemType.Root, out var rootSlot))
        {
            PlayerRoots.Value = rootSlot.Quantity;
            PlayerMaxRoots.Value = rootSlot.Capacity;
        }
    }
    public void AddMessage(string message, float duration = 1)
    {
        MessageQueue.Enqueue((message, duration));
    }
    public void AddDrawCommand(Action<SwHud> action)
    {
        DrawQueue.Enqueue(action);
        QueueRedraw();
    }
    public void AddDrawRect(Rect2 rect, Color color)
    {
        void fn(SwHud hud){
            hud.DrawRect(rect, color);
        }
        AddDrawCommand(fn);
    }
    public void AddDrawLine(Vector2 from, Vector2 to, Color color)
    {
        void fn(SwHud hud){
            hud.DrawLine(from, to, color);
        }
        AddDrawCommand(fn);
    }
    public void AddDrawText(Vector2 position, string text, Color color)
    {
        void fn(SwHud hud){
            hud.DrawString(ThemeDB.FallbackFont, position, text, HorizontalAlignment.Left, -1, 16, color);
        }
        AddDrawCommand(fn);
    }
}
