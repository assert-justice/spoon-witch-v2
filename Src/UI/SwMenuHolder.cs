using System.Collections.Generic;
using Godot;

namespace SW.Src.Ui;

public partial class SwMenuHolder : Control
{
    private readonly List<SwMenu> Menus = [];
    public override void _Ready()
    {
        Menus.Add(GetChild<SwMenu>(0));
        SetMenuInternal();
    }
    private void SetMenuInternal()
    {
        Visible = true;
        var top = GetTop();
        foreach (var child in GetChildren())
        {
            if(child is SwMenu menu)
            {
                if(menu == top) menu.Focus();
                else menu.Visible = false;
            }
        }
    }
    private void Pop()
    {
        // Cannot back out of main menu
        if(Menus.Count <= 1) return;
        Menus.RemoveAt(Menus.Count-1);
    }
    private SwMenu GetTop()
    {
        return Menus[^1];
    }
    public void SetMenu(string menuName)
    {
        for (int idx = 0; idx < Menus.Count; idx++)
        {
            var menu = Menus[idx];
            if(menu.Name == menuName)
            {
                while(Menus.Count > idx + 1) Pop();
                SetMenuInternal();
                return;
            }
        }
        Menus.Add(GetNode<SwMenu>(menuName));
        SetMenuInternal();
    }
    public void Back()
    {
        // Cannot back out of main menu
        if(Menus.Count <= 1) return;
        Pop();
        SetMenuInternal();
    }
    public void ToMainMenu()
    {
        while(Menus.Count > 1)Pop();
        SetMenuInternal();
    }
}
