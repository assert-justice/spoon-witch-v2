using System.Collections.Generic;
using Godot;
using SW.Src.Utils;

namespace SW.Src.Ui;

public partial class SwMenuHolder : Control
{
    private readonly List<SwMenu> MenuStack = [];
    private readonly SwQueueOne<string> NextMenuQueue = new();
    public override void _Ready()
    {
        MenuStack.Add(GetChild<SwMenu>(0));
        SetMenuInternal();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if(!NextMenuQueue.TryDequeue(out string menuName)) return;
        for (int idx = 0; idx < MenuStack.Count; idx++)
        {
            var menu = MenuStack[idx];
            if(menu.Name == menuName)
            {
                while(MenuStack.Count > idx + 1) Pop();
                SetMenuInternal();
                return;
            }
        }
        MenuStack.Add(GetNode<SwMenu>(menuName));
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
                if(menu == top) menu.Wake();
                else menu.Sleep();
            }
        }
    }
    private void Pop()
    {
        // Cannot back out of main menu
        if(MenuStack.Count <= 1) return;
        MenuStack.RemoveAt(MenuStack.Count-1);
    }
    private SwMenu GetTop()
    {
        return MenuStack[^1];
    }
    public void QueueMenu(string menuName)
    {
        NextMenuQueue.Enqueue(menuName);
    }
    public void Back()
    {
        // Cannot back out of main menu
        if(MenuStack.Count <= 1) return;
        Pop();
        SetMenuInternal();
    }
    public void ToMainMenu()
    {
        while(MenuStack.Count > 1)Pop();
        SetMenuInternal();
    }
}
