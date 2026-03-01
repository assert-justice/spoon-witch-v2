using System;
using Godot;

namespace SW.Src.Ui;

public partial class SwCheckBox : CheckBox, ISwUiNode
{
    private Func<bool> OnWakeFn;
    private Action<bool> OnChangeFn;
    public override void _Ready()
    {
        Toggled += OnValueChanged;
    }
    private void OnValueChanged(bool value)
    {
        if(TryGetOnChangeFn(out var onChangeFn)) onChangeFn(value);
    }
    private bool TryGetOnWakeFn(out Func<bool> onWakeFn)
    {
        onWakeFn = OnWakeFn;
        return onWakeFn is not null;
    }
    private bool TryGetOnChangeFn(out Action<bool> onChangeFn)
    {
        onChangeFn = OnChangeFn;
        return onChangeFn is not null;
    }
    public void SetOnWakeFn(Func<bool> onWakeFn){OnWakeFn = onWakeFn;}
    public void SetOnChangeFn(Action<bool> onChangeFn){OnChangeFn = onChangeFn;}
    public void OnSleep(){}
    public void OnWake()
    {
        if(TryGetOnWakeFn(out var onWakeFn)) SetPressedNoSignal(onWakeFn());
    }
}
