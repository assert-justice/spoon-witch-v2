using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using SW.Src.GameSpace.Dungeon;
using SW.Src.Global;
using SW.Src.Ui.Menu;
using SW.Src.Utils;

namespace SW.Src.Ui;
public partial class Main : Control
{
	[Export] private PackedScene GameScene;
	[Export] private AudioStream[] MusicTracks = [];
	private static readonly Queue<string> MessageQueue = new();
	private readonly Dictionary<string, AudioStream> MusicLookup = [];
	private static Main Instance;
	private SubViewport GameHolder;
	private SwMenuHolder MenuHolder;
	private Control SubmenuHolder;
	private AudioStreamPlayer MusicPlayer;
	private readonly Dictionary<string,(string,string)> Tutorials = [];
	private int DeadSlumes = 0;
	private int InjuredSlumes = 0;
	private int DeadKnights = 0;
	public override void _Ready()
	{
		GameHolder = GetNode<SubViewport>("GameHolder/SubViewport");
		MenuHolder = GetNode<SwMenuHolder>("MenuHolder");
		MusicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		foreach (var track in MusicTracks)
		{
			var path = track.ResourcePath.Split('/');
			path = path[^1].Split('.');
			MusicLookup.Add(path[0], track);
		}
		QueueMusic(MusicTracks[0]);
		Instance = this;
		SwFs fs = new();
		if(!fs.TryReadFileRaw("res://tutorials.md", out string contents)) return;
		string id = "";
		string title = "";
		StringBuilder sb = new();
		string[] lines = contents.Split(["\r\n", "\r", "\n"],StringSplitOptions.None);
		foreach (var line in lines)
		{
			if (SwStatic.TrySlice(line, "#", out string heading))
			{
				if(id.Length > 0)
				{
					Tutorials.Add(id, (title, sb.ToString()));
					id = "";
					sb.Clear();
				}
				var h = heading.Split(':');
				id = h[0].Trim();
				title = h[1].Trim();
			}
			else
			{
				sb.Append(line);
				sb.Append('\n');
			}
		}
		if(id.Length > 0)
		{
			Tutorials.Add(id, (title, sb.ToString().Trim()));
		}
		// foreach (var (key, value) in Tutorials)
		// {
		// 	var (t, body) = value;
		// 	GD.Print(key, " ", t);
		// 	GD.Print(body);
		// }
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
				SwGlobal.GetSettings().Save();
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
			case "game_over":
				SetMenu("GameOver");
				break;
			case "options":
				SetMenu("OptionsMenu");
				break;
			case "injured_slume":
				if(InjuredSlumes == 0)
				{
					SetTutorial("flee");
				}
				InjuredSlumes++;
				break;
			case "dead_slume":
				if(DeadSlumes == 0)
				{
					SetTutorial("kill");
					SwDungeon.Message("clear_rect:3269a9f0-fa90-11f0-9f4c-ad66fec81f90,2");
				}
				else if(DeadSlumes == 2)
				{
					SetTutorial("finale");
					SwDungeon.Message("clear_rect:152c4460-fa90-11f0-9f4c-d1f35b1a9b32,2");
				}
				DeadSlumes++;
				break;
			case "dead_knight":
				if(DeadKnights == 1)
				{
					SetMenu("Victory");
				}
				DeadKnights++;
				break;
			default:
				if(SwStatic.TrySlice(message, "queue_music:", out string trackId))
				{
					if(MusicLookup.TryGetValue(trackId, out var track)) QueueMusic(track);
					else if(!int.TryParse(trackId, out int trackIdx)) SwStatic.LogError($"Invalid audio track string '{trackId}'");
					else if(trackIdx < 0 || trackIdx >= MusicTracks.Length) SwStatic.LogError($"Invalid audio track idx '{trackIdx}'");
					else QueueMusic(MusicTracks[trackIdx]);
				}
				else if(SwStatic.TrySlice(message, "log:", out string logMessage)){
					SwStatic.Log(logMessage);
				}
				else if(SwStatic.TrySlice(message, "log_err:", out logMessage)){
					SwStatic.LogError(logMessage);
				}
				else if(SwStatic.TrySlice(message, "tutorial:", out string tutorialId)){
					SetTutorial(tutorialId);
				}
				else SwStatic.LogError("Unexpected message:", message);
				break;
		}
	}
	private void SetTutorial(string tutorialId)
	{
		if(!Tutorials.TryGetValue(tutorialId, out var tutorial)) return;
		GetNode<Label>("MenuHolder/Tutorial/VBox/PanelContainer/VBoxContainer/Title").Text = tutorial.Item1;
		GetNode<Label>("MenuHolder/Tutorial/VBox/PanelContainer/VBoxContainer/Body").Text = tutorial.Item2;
		SetMenu("Tutorial");
	}
	private void LaunchGame()
	{
		FreeGame();
		DeadSlumes = 0;
		InjuredSlumes = 0;
		DeadKnights = 0;
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
	private void QueueMusic(AudioStream track)
	{
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
