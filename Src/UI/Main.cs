using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;
using SW.Src.Ui.Menu;

namespace SW.Src.Ui;
public partial class Main : Control
{
	[Export] private PackedScene GameScene;
	[Export] private AudioStream[] MusicTracks = [];
	private static readonly Queue<string> MessageQueue = new();
	private static Main Instance;
	private SubViewport GameHolder;
	private SwMenuHolder MenuHolder;
	private Control SubmenuHolder;
	private AudioStreamPlayer MusicPlayer;
	public override void _Ready()
	{
		GameHolder = GetNode<SubViewport>("GameHolder/SubViewport");
		MenuHolder = GetNode<SwMenuHolder>("MenuHolder");
		MusicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		QueueMusic(0);
		Instance = this;
	}
	public override void _PhysicsProcess(double delta)
	{
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
				SetMenu("Pause");
				break;
			case "resume":
				SetMenu("Hud");
				break;
			case "restart":
				LaunchGame();
				SetMenu("Hud");
				break;
			case "launch":
				LaunchGame();
				SetMenu("Hud");
				break;
			case "main_menu":
				FreeGame();
				SetMenu("MainMenu");
				break;
			case "options":
				SetMenu("OptionsMenu");
				break;
			default:
				if(SwStatic.TrySlice(message, "set_music:", out string trackIdxStr))
				{
					if(!int.TryParse(trackIdxStr, out int trackIdx)) SwStatic.LogError($"Invalid audio track string '{trackIdxStr}'");
					else if(trackIdx < 0 || trackIdx >= MusicTracks.Length) SwStatic.LogError($"Invalid audio track idx '{trackIdx}'");
					else QueueMusic(trackIdx);
				}
				else SwStatic.LogError("Unexpected message:", message);
				break;
		}
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
	private bool InGame()
	{
		return GameHolder.GetChildCount() != 0;
	}
	private void QueueMusic(int trackIdx)
	{
		var track = MusicTracks[trackIdx];
		MusicPlayer.Stream = track;
		MusicPlayer.Play();
	}
	public static void Message(string message){MessageQueue.Enqueue(message);}
	public static bool TryGetHud(out SwHud hud)
	{
		hud = default;
		if(Instance is null) return false;
		var nodes = Instance.GetTree().GetNodesInGroup("Hud");
		if(nodes.Count != 1) return false;
		if(nodes[0] is not SwHud h) return false;
		hud = h;
		return true;
	} 
}
