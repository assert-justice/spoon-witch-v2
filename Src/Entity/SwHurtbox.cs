using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Effect;

namespace SW.Src.Entity;

public partial class SwHurtbox : Area2D
{
    public enum SwHurtboxDamageEvent
    {
        OnEnter,
        OnRemain,
    }
    [Export] private SwHurtboxDamageEvent DamageEvent = SwHurtboxDamageEvent.OnEnter;
    [Export] protected Node2D DamageSource;
    [Export] private string[] Whitelist;
    [Export] private SwDamage[] Damages;
    public HashSet<string> GroupWhitelist = [];
    public HashSet<Node2D> TargetsInArea = [];
    public List<SwDamage> DamageList = [];
    private CollisionShape2D Collider;
    public bool IsEnabled{get=>!Collider.Disabled; set{Collider.Disabled = !value;}}
    public Action OnDamageFn = ()=>{};
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnBodyEntered;
        BodyExited += OnNodeExitedInternal;
        AreaExited += OnNodeExitedInternal;
        Collider = GetChild<CollisionShape2D>(0);
        DamageSource ??= this;
        if(Whitelist is not null) GroupWhitelist = [..Whitelist];
        if(Damages is not null) DamageList = [..Damages];
    }
    public override void _PhysicsProcess(double delta)
    {
        if(DamageEvent != SwHurtboxDamageEvent.OnRemain) return;
        foreach (var target in TargetsInArea)
        {
            OnTargetInArea(target);
        }
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
        if(!IsTarget(node)) return;
        TargetsInArea.Add(node);
        OnNodeEntered(node);
    }
    private void OnNodeExitedInternal(Node2D node)
    {
        TargetsInArea.Remove(node);
    }
    protected virtual void OnNodeEntered(Node2D node)
    {
        if(DamageEvent == SwHurtboxDamageEvent.OnEnter && node is ISwDamageable damageable) DoDamage(damageable);
    }
    protected virtual void OnTargetInArea(Node2D target)
    {
        if(target is ISwDamageable damageable) DoDamage(damageable);
    }
    protected virtual void DoDamage(ISwDamageable damageable)
    {
        foreach (var damage in DamageList)
        {
            damageable.Damage(damage, DamageSource);
        }
        OnDamageFn();
    }
    protected virtual bool IsTarget(Node2D node)
    {
        if(node == DamageSource) return false;
        foreach (var group in node.GetGroups())
        {
            if(GroupWhitelist.Contains(group)) return false;
        }
        return true;
    }
    public void SetDamageSource(Node2D damageSource){DamageSource = damageSource;}
}
