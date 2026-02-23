using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Effect;
using SW.Src.Timer;

namespace SW.Src.Entity.Projectile;

public abstract partial class SwProjectile : SwHurtbox
{
    protected Vector2 Velocity = Vector2.Zero;
    public override void _Ready()
    {
        base._Ready();
    }
    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Position += Velocity * dt;
    }
    protected override void OnNodeEntered(Node2D node)
    {
        base.OnNodeEntered(node);
        if(ShouldDie(node)) Die();
    }
    protected virtual bool ShouldDie(Node2D node){return false;}
    protected virtual void Die()
    {
        Cleanup();
    }
    protected virtual void Cleanup()
    {
        QueueFree();
    }
    public void Init(Node parent, Vector2 velocity, Vector2 position)
    {
        parent.AddChild(this);
        Velocity = velocity;
        Position = position;
    }
}
