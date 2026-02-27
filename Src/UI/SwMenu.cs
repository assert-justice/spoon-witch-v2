using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;
using SW.Src.Input;

namespace SW.Src.Ui;

public partial class SwMenu : Control
{
    private SwMenuHolder MenuHolder;
    private readonly List<Control> FocusPoints = [];
    private SwInputManager InputManager;
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
        AttachButton("VBox/Audio", ()=>MenuHolder.QueueMenu("AudioMenu"));
        AttachButton("VBox/Accessibility", ()=>MenuHolder.QueueMenu("AccessibilityMenu"));
        GetFocusPoints(this);
        InputManager = SwGlobal.GetInputManager();
    }
    public override void _PhysicsProcess(double delta)
    {
        if(!IsAwake) return;
        if(!TryGetCurrentFocus(out var fp)) return;
        if(InputManager.UiConfirm.IsJustPressed())
        {
            // This is *dumb*
            if(fp is CheckBox checkBox)
            {
                checkBox.ButtonPressed = !checkBox.ButtonPressed;
            }
            else if(fp is Button button)
            {
                button.EmitSignal("pressed");
            }
        }
        else if(InputManager.UiCancel.IsJustPressed()) MenuHolder.Back();
        else if (InputManager.UiDown.IsJustPressed()) IncFocus();
        else if (InputManager.UiUp.IsJustPressed()) DecFocus();
        else if (InputManager.UiLeft.IsJustPressed())
        {
            if(fp is HSlider slider)
            {
                slider.Value += slider.Step;
            }
        }
        else if (InputManager.UiRight.IsJustPressed())
        {
            if(fp is HSlider slider)
            {
                slider.Value -= slider.Step;
            }
        }
    }
    private bool TryGetCurrentFocus(out Control focusPoint)
    {
        if(FocusPoints.Count > FocusIdx)
        {
            focusPoint = FocusPoints[FocusIdx];
            return true;
        }
        focusPoint = default;
        return false;
    }
    private void IncFocus()
    {
        FocusIdx++;
        if(FocusIdx >= FocusPoints.Count) FocusIdx = 0;
        if(TryGetCurrentFocus(out var focus)) focus.GrabFocus();
    }
    private void DecFocus()
    {
        FocusIdx--;
        if(FocusIdx < 0) FocusIdx = FocusPoints.Count - 1;
        if(TryGetCurrentFocus(out var focus)) focus.GrabFocus();
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
        if(TryGetCurrentFocus(out var focus)) focus.GrabFocus();
        // if(FocusPoints.Count > FocusIdx) FocusPoints[FocusIdx].GrabFocus();
    }
    public virtual void Sleep()
    {
        IsAwake = false;
        Visible = false;
    }
}
