using Godot;
using SW.Src.Global;

namespace SW.Src.Ui.Menu;

public partial class SwAudioMenu : SwMenu
{
	private AudioStreamPlayer SfxPlayer;
	private AudioStreamPlayer VoicePlayer;
	private SwNamedHScroll MainSlider;
	private SwNamedHScroll MusicSlider;
	private SwNamedHScroll SfxSlider;
	private SwNamedHScroll VoiceSlider;
	public override void _Ready()
	{
		base._Ready();
		SfxPlayer = GetNode<AudioStreamPlayer>("SfxPlayer");
		VoicePlayer = GetNode<AudioStreamPlayer>("VoicePlayer");
		MainSlider = GetNode<SwNamedHScroll>("VBox/MainVolume");
		MainSlider.SetOnWakeFn(()=>SwGlobal.GetVolume(SwGlobal.AudioBus.Main));
		MainSlider.SetOnChangeFn((volume) =>
		{
			SwGlobal.SetVolume(SwGlobal.AudioBus.Main, volume);
			SfxPlayer.Play();
		});
		MusicSlider = GetNode<SwNamedHScroll>("VBox/MusicVolume");
		MusicSlider.SetOnWakeFn(()=>SwGlobal.GetVolume(SwGlobal.AudioBus.Music));
		MusicSlider.SetOnChangeFn((volume) =>
		{
			SwGlobal.SetVolume(SwGlobal.AudioBus.Music, volume);
		});
		SfxSlider = GetNode<SwNamedHScroll>("VBox/SfxVolume");
		SfxSlider.SetOnWakeFn(()=>SwGlobal.GetVolume(SwGlobal.AudioBus.Sfx));
		SfxSlider.SetOnChangeFn((volume) =>
		{
			SwGlobal.SetVolume(SwGlobal.AudioBus.Sfx, volume);
			SfxPlayer.Play();
		});
		VoiceSlider = GetNode<SwNamedHScroll>("VBox/VoiceVolume");
		VoiceSlider.SetOnWakeFn(()=>SwGlobal.GetVolume(SwGlobal.AudioBus.Voice));
		VoiceSlider.SetOnChangeFn((volume) =>
		{
			SwGlobal.SetVolume(SwGlobal.AudioBus.Voice, volume);
			VoicePlayer.Play();
		});
	}
}
