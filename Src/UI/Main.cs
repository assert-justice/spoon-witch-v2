using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;

namespace SW.Src.Ui;
public partial class Main : Control
{
	[Export] private PackedScene GameScene;
	private static readonly Queue<string> MessageQueue = new();
	private SubViewport GameHolder;
	private SwMenuHolder MenuHolder;
	private Control Hud;
	public override void _Ready()
	{
		GameHolder = GetNode<SubViewport>("GameHolder/SubViewport");
		MenuHolder = GetNode<SwMenuHolder>("MenuHolder");
		Hud = GetNode<Control>("Hud");
	}
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
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
				MenuHolder.Visible = true;
				Hud.Visible = false;
				SetMenu("Pause");
				Pause();
				break;
			case "resume":
				MenuHolder.Visible = false;
				Hud.Visible = true;
				Resume();
				break;
			case "restart":
				MenuHolder.Visible = false;
				Hud.Visible = true;
				LaunchGame();
				break;
			case "launch":
				Hud.Visible = true;
				MenuHolder.Visible = false;
				LaunchGame();
				break;
			case "main_menu":
				Hud.Visible = false;
				FreeGame();
				MenuHolder.Visible = true;
				SetMenu("MainMenu");
				break;
			case "options":
				SetMenu("OptionsMenu");
				break;
			default:
				if(SwStatic.TrySlice(message, "set_submenu:", out string menu))
				{
					MenuHolder.QueueMenu(menu);
				}
				else SwStatic.LogError("Unexpected message:", message);
				break;
		}
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
		Resume();
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
