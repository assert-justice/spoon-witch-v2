using Godot;
using SW.Src.Global;

namespace SW.Src.Ui.Menu;

public partial class SwOptionsMenu : SwMenu
{
    private SwCheckBox FullscreenCheckBox;
    private SwCheckBox SkipTutorialsCheckBox;
    public override void _Ready()
    {
        base._Ready();
        FullscreenCheckBox = GetNode<SwCheckBox>("VBox/Fullscreen");
        FullscreenCheckBox.SetOnWakeFn(()=>SwGlobal.IsFullscreen());
        FullscreenCheckBox.SetOnChangeFn(SwGlobal.SetFullscreen);
        SkipTutorialsCheckBox = GetNode<SwCheckBox>("VBox/SkipTutorials");
        SkipTutorialsCheckBox.SetOnWakeFn(()=>SwGlobal.GetSettings().SkipTutorials);
        SkipTutorialsCheckBox.SetOnChangeFn(val => SwGlobal.GetSettings().SkipTutorials = val);
    }
}
