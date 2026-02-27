using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;
using SW.Src.Utils;

namespace SW.Src.Ui.Menu;

public partial class SwHud : SwMenu
{
    [Export] private int MaxVisibleMessages = 5;
    private readonly Queue<(string,float)> MessageQueue = new();
    private readonly Queue<Action<SwHud>> DrawQueue = new();
    private Label TextLabel;
    private VBoxContainer MessageContainer;
    private SwDirtyWrapper<float> PlayerHealth = new(100);
    private SwDirtyWrapper<float> PlayerAmmo = new(5);
    public override void _Ready()
    {
        TextLabel = GetNode<Label>("VBox/Text");
        MessageContainer = GetNode<VBoxContainer>("VBox/Messages");
        SwStatic.FreeChildren(MessageContainer);
    }
    public override void _PhysicsProcess(double delta)
    {
        if(PlayerHealth.IsDirty() || PlayerAmmo.IsDirty()) SetText();
        UpdateMessages();
        // Deliberately does not call base physics process method
    }
    public override void _Draw()
    {
        while(DrawQueue.TryDequeue(out var action)) action(this);
    }
    private void SetText()
    {
        TextLabel.Text = $"Health: {PlayerHealth.Value} Ammo: {PlayerAmmo.Value}";
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
    public void SetHealth(float health){PlayerHealth.Value = health;}
    public void SetAmmo(float ammo){PlayerAmmo.Value = ammo;}
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
