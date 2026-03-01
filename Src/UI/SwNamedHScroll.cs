using System;
using Godot;

namespace SW.Src.Ui;

public partial class SwNamedHScroll : Control, ISwUiNode
{
    [Export] private string DisplayName = "[Unnamed Slider]";
    private Func<float> OnWakeFn;
    private Action<float> OnChangeFn;
    private HSlider Slider;
    private Label TextLabel;
    public override void _Ready()
    {
        Slider = GetNode<HSlider>("VBoxContainer/HSlider");
        TextLabel = GetNode<Label>("VBoxContainer/Label");
        OnChangeFn = (_) => {GD.Print("on change called");};
        Slider.ValueChanged += OnValueChanged;
    }
    private void OnValueChanged(double v)
    {
        float value = (float)v;
        UpdateLabel(value);
        if(TryGetOnChangeFn(out var onChangeFn)) onChangeFn(value);
    }

    private bool TryGetOnWakeFn(out Func<float> onWakeFn)
    {
        onWakeFn = OnWakeFn;
        return onWakeFn is not null;
    }
    private bool TryGetOnChangeFn(out Action<float> onChangeFn)
    {
        onChangeFn = OnChangeFn;
        return onChangeFn is not null;
    }

    public void SetOnWakeFn(Func<float> onWakeFn){OnWakeFn = onWakeFn;}
    public void SetOnChangeFn(Action<float> onChangeFn){OnChangeFn = onChangeFn;}
    private void UpdateLabel(float value)
    {
        TextLabel.Text = $"{DisplayName} {value:F0}%";
    }
    public void OnSleep(){}
    public void OnWake()
    {
        if(!TryGetOnWakeFn(out var onWakeFn)) return;
        float value = onWakeFn() * 100;
        UpdateLabel(value);
        Slider.SetValueNoSignal((double)value);
    }
}
