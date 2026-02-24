using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Effect;

namespace SW.Src.Entity;

public partial class SwHurtbox : Area2D
{
    [Export] protected Node2D DamageSource;
    public readonly HashSet<string> GroupWhitelist = [];
    public List<SwDamage> Damages = [];
    private CollisionShape2D Collider;
    public bool IsEnabled{get=>!Collider.Disabled; set{Collider.Disabled = !value;}}
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
        Collider = GetChild<CollisionShape2D>(0);
    }
    private void OnAreaEntered(Area2D area)
    {
        OnNodeEnteredInternal(area);
    }
    private void OnBodyEntered(Node2D body)
    {
        OnNodeEnteredInternal(body);
    }
    private void OnNodeEnteredInternal(Node2D node)
    {
        if(IsTarget(node)) OnNodeEntered(node);
    }
    protected virtual void OnNodeEntered(Node2D node)
    {
        if(node is ISwDamageable damageable) DoDamage(damageable);
    }
    protected virtual void DoDamage(ISwDamageable damageable)
    {
        foreach (var damage in Damages)
        {
            damageable.Damage(damage, DamageSource);
        }
    }
    protected virtual bool IsTarget(Node2D node)
    {
        foreach (var group in node.GetGroups())
        {
            if(GroupWhitelist.Contains(group)) return false;
        }
        return true;
    }
    public void SetDamageSource(Node2D damageSource){DamageSource = damageSource;}
}
