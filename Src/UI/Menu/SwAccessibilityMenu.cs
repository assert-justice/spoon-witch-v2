using SW.Src.Global;

namespace SW.Src.Ui.Menu;

public partial class SwAccessibilityMenu : SwMenu
{
    private SwNamedHScroll DamageTakenMultiplier;
    private SwNamedHScroll DamageDealtMultiplier;
    private SwCheckBox PauseOnSubmenu;
    public override void _Ready()
    {
        base._Ready();
        DamageTakenMultiplier = GetNode<SwNamedHScroll>("VBox/DamageTaken");
        DamageTakenMultiplier.SetOnWakeFn(()=>SwGlobal.GetSettings().DamageTakenMultiplier);
        DamageTakenMultiplier.SetOnChangeFn((value) =>SwGlobal.GetSettings().DamageTakenMultiplier = value);
        DamageDealtMultiplier = GetNode<SwNamedHScroll>("VBox/DamageDealt");
        DamageDealtMultiplier.SetOnWakeFn(()=>SwGlobal.GetSettings().DamageDealtMultiplier);
        DamageDealtMultiplier.SetOnChangeFn((value) =>SwGlobal.GetSettings().DamageDealtMultiplier = value);
        PauseOnSubmenu = GetNode<SwCheckBox>("VBox/PauseOnSubmenu");
        PauseOnSubmenu.SetOnWakeFn(()=>SwGlobal.GetSettings().PauseOnSubmenu);
        PauseOnSubmenu.SetOnChangeFn((value) =>SwGlobal.GetSettings().PauseOnSubmenu = value);
    }
}
