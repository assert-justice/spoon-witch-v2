using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Actor;
using SW.Src.Actor.Player;
using SW.Src.GameSpace.GameState;
using SW.Src.Global;
using SW.Src.Ui;
using SW.Src.Utils;

namespace SW.Src.GameSpace;

public partial class SwGame : Node2D
{
    public enum GameState
    {
        Default,
        Combat,
        GameOver,
    }
    private readonly Dictionary<Type, List<SwEnemy>> EnemyLookup = [];
    public const string BEACH_THEME = "SW Beach Theme";
    public const string COMBAT_THEME = "SW Combat Theme (hard synth)";
    private string CurrentMusic;
    public SwStateMachine<SwGame, GameState> StateMachine{get; private set;}
    public bool InCombat{get; private set;} = false;
    public override void _Ready()
    {
        base._Ready();
        StateMachine = new(GameState.Default);
        StateMachine.AddState(new SwGameStateDefault(this));
        StateMachine.AddState(new SwGameStateCombat(this));
        StateMachine.AddState(new SwGameStateGameOver(this));
        StateMachine.LogStates = true;
    }
    public override void _PhysicsProcess(double delta)
    {
        if(!TryGetPlayer(out _)) Main.Message("game_over"); //StateMachine.QueueState(GameState.GameOver);
        GetEnemies();
        StateMachine.Tick((float)delta);
    }
    private bool TryGetPlayer(out SwPlayer player)
    {
        player = default;
        var nodes = GetTree().GetNodesInGroup("Player");
        if(nodes.Count != 1) return false;
        if(nodes[0] is not SwPlayer p) return false;
        player = p;
        return true;
    }
    private void GetEnemies()
    {
        var nodes = GetTree().GetNodesInGroup("Enemy");
        EnemyLookup.Clear();
        InCombat = false;
        foreach (var node in nodes)
        {
            if(node is not SwEnemy enemy)
            {
                SwStatic.LogError("Found node in enemy group that is not a member of <enemies>");
                continue;
            }
            Type enemyType = enemy.GetType();
            if(!EnemyLookup.TryGetValue(enemyType, out var enemies))
            {
                enemies = [];
                EnemyLookup.Add(enemyType, enemies);
            }
            if(enemy.IsAlive()) InCombat = true;
            enemies.Add(enemy);
        }
    }
    public void QueueMusic(string trackName)
    {
        if(CurrentMusic == trackName) return;
        CurrentMusic = trackName;
        Main.Message("queue_music:" + trackName);
    }
}
