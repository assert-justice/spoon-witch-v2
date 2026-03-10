using Godot;
using SW.Src.Global;

namespace SW.Src.Ui.Menu;

public partial class SwOptionsMenu : SwMenu
{
    // private SwCheckBox FullscreenCheckBox;
    // private SwCheckBox SkipTutorialsCheckBox;
    public override void _Ready()
    {
        base._Ready();
        var checkbox = GetNode<SwCheckBox>("PanelContainer/VBox/Fullscreen");
        checkbox.SetOnWakeFn(()=>SwGlobal.IsFullscreen());
        checkbox.SetOnChangeFn(SwGlobal.SetFullscreen);
        checkbox = GetNode<SwCheckBox>("PanelContainer/VBox/SkipTutorials");
        checkbox.SetOnWakeFn(()=>SwGlobal.GetSettings().SkipTutorials);
        checkbox.SetOnChangeFn(val => SwGlobal.GetSettings().SkipTutorials = val);
        checkbox = GetNode<SwCheckBox>("PanelContainer/VBox/SpoonAimMode");
        checkbox.SetOnWakeFn(()=>SwGlobal.GetSettings().AimSpoonWithKeyboard);
        checkbox.SetOnChangeFn(val => SwGlobal.GetSettings().AimSpoonWithKeyboard = val);
        checkbox = GetNode<SwCheckBox>("PanelContainer/VBox/SlingChargeMode");
        checkbox.SetOnWakeFn(()=>SwGlobal.GetSettings().AutoChargeSlingWithGamepad);
        checkbox.SetOnChangeFn(val => SwGlobal.GetSettings().AutoChargeSlingWithGamepad = val);
    }
}
