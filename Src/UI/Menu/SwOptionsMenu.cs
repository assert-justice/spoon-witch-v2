using Godot;
using SW.Src.Global;

namespace SW.Src.Ui.Menu;

public partial class SwOptionsMenu : SwMenu
{
    private SwCheckBox FullscreenCheckBox;
    public override void _Ready()
    {
        base._Ready();
        FullscreenCheckBox = GetNode<SwCheckBox>("VBox/Fullscreen");
        FullscreenCheckBox.SetOnWakeFn(()=>SwGlobal.IsFullscreen());
        FullscreenCheckBox.SetOnChangeFn(SwGlobal.SetFullscreen);
    }
}
