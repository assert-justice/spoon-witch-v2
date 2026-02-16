using Godot;

namespace SW.Src.Ui;

public partial class SwMenu : Control
{
    private SwMenuHolder MenuHolder;
    public override void _Ready()
    {
        MenuHolder = GetParent<SwMenuHolder>();
        if(GetNodeOrNull<Button>("VBox/Back") is Button back) back.Pressed += MenuHolder.Back;
        if(GetNodeOrNull<Button>("VBox/Quit") is Button quit) quit.Pressed += ()=>Main.Message("quit");
        if(GetNodeOrNull<Button>("VBox/Credits") is Button credits) credits.Pressed += ()=>MenuHolder.SetMenu("Credits");
    }
    private static void GetFocus(Control parent)
    {
        foreach (var child in parent.GetChildren())
        {
            if(child is Control control)
            {
                if(control.FocusMode != FocusModeEnum.None)
                {
                    control.GrabFocus();
                    return;
                }
                GetFocus(control);
            }
        }
    }
    public void Focus()
    {
        Visible = true;
        GetFocus(this);
    }
}
