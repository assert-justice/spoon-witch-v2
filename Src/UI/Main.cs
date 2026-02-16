using System.Collections.Generic;
using Godot;
using SW.Src.Input;
using SW.Src.StateMachine;

namespace SW.Src.Ui;
public partial class Main : Control
{
	[Export] private PackedScene GameScene;
	public enum SwMainState
	{
		NoGame,
		Running,
		Paused,
		InUi,
	}
	private static readonly Queue<string> MessageQueue = new();
	private SwStateMachine<SwMainState> StateMachine;
	private Node2D GameHolder;
	private SwMenuHolder MenuHolder;
	public override void _Ready()
	{
		GameHolder = GetNode<Node2D>("GameHolder");
		MenuHolder = GetNode<SwMenuHolder>("MenuHolder");
		StateMachine = new(SwMainState.NoGame);
		StateMachine.AddState(new(SwMainState.NoGame, OnEnterMainMenu));
		StateMachine.AddState(new(SwMainState.Running, OnEnterGame));
		StateMachine.AddState(new(SwMainState.Paused, OnEnterPause));
		StateMachine.AddState(new(SwMainState.InUi, OnEnterUi));
	}
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		StateMachine.Tick(dt);
		while(MessageQueue.TryDequeue(out string message)) HandleMessage(message);
	}
	private void HandleMessage(string message)
	{
		switch (message)
		{
			case "quit":
				GetTree().Quit();
				break;
			case "pause":
				StateMachine.QueueState(SwMainState.Paused);
				break;
			case "resume":
				StateMachine.QueueState(SwMainState.Running);
				break;
			case "launch":
				StateMachine.QueueState(SwMainState.Running);
				break;
			case "main_menu":
				StateMachine.QueueState(SwMainState.NoGame);
				break;
			default:
				if(TrySlice(message, "set_ui:", out string menu))
				{
					MenuHolder.SetMenu(menu);
				}
				break;
		}
	}
	private static bool TrySlice(string str, string start, out string slice)
	{
		slice = default;
		if(!str.StartsWith(start)) return false;
		slice = str[start.Length..].Trim();
		return true;
	}
	private void OnEnterMainMenu(SwMainState lastStateId)
	{
		// If game exists, free it
		FreeGame();
		// Make main menu visible
		MenuHolder.ToMainMenu();
	}
	private void OnEnterGame(SwMainState lastStateId)
	{
		// If game does not exist, create it
		LaunchGame();
		// Unpause
		Resume();
	}
	private void OnEnterPause(SwMainState lastStateId)
	{
		// Pause game and show pause menu
		Pause();
		MenuHolder.SetMenu("Pause");
	}
	private void OnEnterUi(SwMainState lastStateId)
	{
		// Show requested ui page
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
		MenuHolder.SetMenu(menuName);
	}
	public static void Message(string message){MessageQueue.Enqueue(message);}
}
