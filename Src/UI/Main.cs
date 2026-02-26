using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;
using SW.Src.Input;
using SW.Src.Utils;

namespace SW.Src.Ui;
public partial class Main : Control
{
	[Export] private PackedScene GameScene;
	public enum SwGameState
	{
		NoGame,
		Running,
		Paused,
		InSubmenu,
	}
	private static readonly Queue<string> MessageQueue = new();
	private readonly Dictionary<SwGameState, Action<SwGameState>> GameStates = [];
	// private readonly Queue<SwGameState> StateQueue = new();
	private readonly SwQueueOne<SwGameState> StateQueue = new();
	private SwGameState GameState = SwGameState.NoGame;
	private SwGameState LastGameState = SwGameState.NoGame;
	private Node2D GameHolder;
	private SwMenuHolder MenuHolder;
	public override void _Ready()
	{
		GameHolder = GetNode<Node2D>("GameHolder");
		MenuHolder = GetNode<SwMenuHolder>("MenuHolder");
		GameStates.Add(SwGameState.NoGame, OnEnterMainMenu);
		GameStates.Add(SwGameState.Running, OnEnterGame);
		GameStates.Add(SwGameState.Paused, OnEnterPause);
		GameStates.Add(SwGameState.InSubmenu, OnEnterSubmenu);
	}
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		while(MessageQueue.TryDequeue(out string message)) HandleMessage(message);
		while(StateQueue.TryDequeue(out var state))
		{
			if(!GameStates.TryGetValue(state, out var action)) throw new Exception($"Unexpected game state '{state}'");
			if(state == GameState) continue;
			action(GameState);
			GameState = state;
		}
	}
	private void HandleMessage(string message)
	{
		switch (message)
		{
			case "quit":
				GetTree().Quit();
				break;
			case "pause":
				StateQueue.Enqueue(SwGameState.Paused);
				break;
			case "resume":
				StateQueue.Enqueue(SwGameState.Running);
				break;
			case "launch":
				StateQueue.Enqueue(SwGameState.Running);
				break;
			case "main_menu":
				StateQueue.Enqueue(SwGameState.NoGame);
				break;
			default:
				StateQueue.Enqueue(SwGameState.InSubmenu);
				if(SwStatic.TrySlice(message, "set_submenu:", out string menu))
				{
					MenuHolder.QueueMenu(menu);
				}
				else SwStatic.LogError("Unexpected message:", message);
				break;
		}
	}
	private void OnEnterMainMenu(SwGameState lastStateId)
	{
		// If game exists, free it
		FreeGame();
		// Make main menu visible
		MenuHolder.ToMainMenu();
	}
	private void OnEnterGame(SwGameState lastStateId)
	{
		// If game does not exist, create it
		LaunchGame();
		// Unpause
		Resume();
	}
	private void OnEnterPause(SwGameState lastStateId)
	{
		// Pause game and show pause menu
		Pause();
		MenuHolder.QueueMenu("Pause");
	}
	private void OnEnterSubmenu(SwGameState lastStateId)
	{
		// Show requested submenu
	}
	private void Pause()
	{
		GetTree().Paused = true;
	}
	private void Resume()
	{
		GetTree().Paused = false;
	}
	private bool InGame()
	{
		return GameHolder.GetChildCount() != 0;
	}
	private void LaunchGame()
	{
		FreeGame();
		var game = GameScene.Instantiate();
		GameHolder.AddChild(game);
	}
	private void FreeGame()
	{
		if(InGame())GameHolder.GetChild(0).QueueFree();
	}
	private void SetMenu(string menuName)
	{
		MenuHolder.QueueMenu(menuName);
	}
	public static void Message(string message){MessageQueue.Enqueue(message);}
}
