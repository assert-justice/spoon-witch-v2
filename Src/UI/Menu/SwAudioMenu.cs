using Godot;
using SW.Src.Global;

namespace SW.Src.Ui.Menu;

public partial class SwAudioMenu : SwMenu
{
    private AudioStreamPlayer SfxPlayer;
    private SwNamedHScroll MainSlider;
    private SwNamedHScroll MusicSlider;
    private SwNamedHScroll SfxSlider;
    public override void _Ready()
    {
        base._Ready();
        SfxPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
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
    }
}
