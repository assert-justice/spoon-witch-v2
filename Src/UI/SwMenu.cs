using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;
using SW.Src.Input;

namespace SW.Src.Ui;

public partial class SwMenu : Control, ISwUiNode
{
    private SwMenuHolder MenuHolder;
    private readonly List<Control> FocusPoints = [];
    private readonly List<ISwUiNode> UiNodes = [];
    private SwInputManager InputManager;
    private int FocusIdx = 0;
    private bool IsAwake = false;
    public override void _Ready()
    {
        MenuHolder = GetParent<SwMenuHolder>();
        BindControlNodes([
            ("Embark", ()=>Main.Message("launch")),
            ("Back", MenuHolder.Back),
            ("Quit", ()=>Main.Message("quit")),
            ("Resume", ()=>Main.Message("resume")),
            ("Restart", ()=>Main.Message("restart")),
            ("MainMenu", ()=>Main.Message("main_menu")),
            ("Options", ()=>Main.Message("options")),
            ("Credits", ()=>MenuHolder.QueueMenu("Credits")),
            ("Audio", ()=>MenuHolder.QueueMenu("AudioMenu")),
            ("Accessibility", ()=>MenuHolder.QueueMenu("AccessibilityMenu")),
            ("Debug", ()=>MenuHolder.QueueMenu("DebugMenu")),
        ]);
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
                slider.Value -= slider.Step;
            }
        }
        else if (InputManager.UiRight.IsJustPressed())
        {
            if(fp is HSlider slider)
            {
                slider.Value += slider.Step;
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
    private void SetFocus()
    {
        if(TryGetCurrentFocus(out var focus)) focus.GrabFocus();
    }
    private void IncFocus()
    {
        FocusIdx++;
        if(FocusIdx >= FocusPoints.Count) FocusIdx = 0;
        SetFocus();
    }
    private void DecFocus()
    {
        FocusIdx--;
        if(FocusIdx < 0) FocusIdx = FocusPoints.Count - 1;
        SetFocus();
    }
    private List<Control> GetControlNodesDfs(Control parent = null, List<Control> list = null)
    {
        parent ??= this;
        list ??= [];
        foreach (var child in parent.GetChildren())
        {
            if(child is Control control)
            {
                list.Add(control);
                GetControlNodesDfs(control, list);
            }
        }
        return list;
    }
    private void BindControlNodes((string, Action)[] values)
    {
        Dictionary<string, Action> actions = new(values.Length);
        foreach (var (name, action) in values)
        {
            actions.Add(name, action);
        }
        foreach (var control in GetControlNodesDfs())
        {
            if(control is Button button && actions.TryGetValue(control.Name, out var action))
            {
                button.Pressed += action;
            }
            if(control.FocusMode != FocusModeEnum.None)
            {
                FocusPoints.Add(control);
            }
            if(control is ISwUiNode uiNode)
            {
                UiNodes.Add(uiNode);
            }
        }
    }
    protected virtual bool ShouldPauseOnWake(){return true;}
    public virtual void OnWake()
    {
        foreach (var uiNode in UiNodes)
        {
            uiNode.OnWake();
        }
        IsAwake = true;
        Visible = true;
        GetTree().Paused = ShouldPauseOnWake();
        SetFocus();
    }
    public virtual void OnSleep()
    {
        foreach (var uiNode in UiNodes)
        {
            uiNode.OnSleep();
        }
        IsAwake = false;
        Visible = false;
    }
}
