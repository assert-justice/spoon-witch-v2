using SW.Src.Global;

namespace SW.Src.Ui.Menu;

public partial class SwDebugMenu : SwMenu
{
    private SwNamedHScroll MaxPitchShift;
    private SwNamedHScroll MinPitchShift;
    private SwCheckBox DebugDraw;
    private SwCheckBox CreativeMode;
    public override void _Ready()
    {
        base._Ready();
        MaxPitchShift = GetNode<SwNamedHScroll>("VBox/MaxPitchShift");
        MaxPitchShift.SetOnWakeFn(()=>SwGlobal.GetSettings().MaxPitchShift);
        MaxPitchShift.SetOnChangeFn((value) =>SwGlobal.GetSettings().MaxPitchShift = value);
        MinPitchShift = GetNode<SwNamedHScroll>("VBox/MinPitchShift");
        MinPitchShift.SetOnWakeFn(()=>SwGlobal.GetSettings().MinPitchShift);
        MinPitchShift.SetOnChangeFn((value) =>SwGlobal.GetSettings().MinPitchShift = value);
        DebugDraw = GetNode<SwCheckBox>("VBox/DebugDraw");
        DebugDraw.SetOnWakeFn(()=>SwGlobal.GetSettings().DebugDraw);
        DebugDraw.SetOnChangeFn((value) =>SwGlobal.GetSettings().DebugDraw = value);
        CreativeMode = GetNode<SwCheckBox>("VBox/CreativeMode");
        CreativeMode.SetOnWakeFn(()=>SwGlobal.GetSettings().CreativeMode);
        CreativeMode.SetOnChangeFn((value) =>SwGlobal.GetSettings().CreativeMode = value);
    }
}
