using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;

namespace SW.Src.Ui;

public partial class SwMenu : Control
{
    private SwMenuHolder MenuHolder;
    private readonly List<Control> FocusPoints = [];
    private int FocusIdx = 0;
    private bool IsAwake = false;
    public override void _Ready()
    {
        MenuHolder = GetParent<SwMenuHolder>();
        AttachButton("VBox/Embark", ()=>Main.Message("launch"));
        AttachButton("VBox/Back", MenuHolder.Back);
        AttachButton("VBox/Quit", ()=>Main.Message("quit"));
        AttachButton("VBox/Resume", ()=>Main.Message("resume"));
        AttachButton("VBox/Restart", ()=>Main.Message("restart"));
        AttachButton("VBox/MainMenu", ()=>Main.Message("main_menu"));
        AttachButton("VBox/Options", ()=>Main.Message("options"));
        AttachButton("VBox/Credits", ()=>MenuHolder.QueueMenu("Credits"));
        GetFocusPoints(this);
    }
    public override void _PhysicsProcess(double delta)
    {
        if(!IsAwake) return;
        var im = SwGlobal.GetInputManager();
        if(im.UiConfirm.IsJustPressed())
        {
            Control fp = FocusPoints[FocusIdx];
            if(fp is Button button)
            {
                button.EmitSignal("pressed");
            }
        }
        else if(im.UiCancel.IsJustPressed()) MenuHolder.Back();
        else if (im.UiDown.IsJustPressed())
        {
            
            FocusIdx++;
            if(FocusIdx >= FocusPoints.Count) FocusIdx = 0;
            FocusPoints[FocusIdx].GrabFocus();
        }
        else if (im.UiDown.IsJustPressed())
        {
            FocusIdx++;
            if(FocusIdx >= FocusPoints.Count) FocusIdx = 0;
            FocusPoints[FocusIdx].GrabFocus();
        }
        else if (im.UiUp.IsJustPressed())
        {
            FocusIdx--;
            if(FocusIdx < 0) FocusIdx = FocusPoints.Count - 1;
            FocusPoints[FocusIdx].GrabFocus();
        }
    }
    private void GetFocusPoints(Control parent)
    {
        foreach (var child in parent.GetChildren())
        {
            if(child is Control control)
            {
                if(control.FocusMode != FocusModeEnum.None)
                {
                    FocusPoints.Add(control);
                }
                GetFocusPoints(control);
            }
        }
    }
    private void AttachButton(string nodePath, Action action)
    {
        if(GetNodeOrNull<Button>(nodePath) is Button button) button.Pressed += action;
    }
    protected virtual bool ShouldPauseOnWake(){return true;}
    public virtual void Wake()
    {
        IsAwake = true;
        Visible = true;
        GetTree().Paused = ShouldPauseOnWake();
        if(FocusPoints.Count > FocusIdx) FocusPoints[FocusIdx].GrabFocus();
    }
    public virtual void Sleep()
    {
        IsAwake = false;
        Visible = false;
    }
}
