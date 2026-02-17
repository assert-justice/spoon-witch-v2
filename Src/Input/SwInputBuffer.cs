using System;
using SW.Src.Global;
using SW.Src.Utils;

namespace SW.Src.Input;

public class SwInputBuffer : ISwPoll
{
    private bool HasValue = false;
    private int Value = 0;
    private readonly Func<bool>[] Fns;
    private readonly bool[] Values;
    public SwInputBuffer(string[] buttonNames)
    {
        Fns = new Func<bool>[buttonNames.Length];
        Values = new bool[buttonNames.Length];
        for (int idx = 0; idx < buttonNames.Length; idx++)
        {
            string name = buttonNames[idx];
            if(!SwGlobal.GetInputManager().TryGetButton(name, out var button)) throw new Exception($"Invalid button name '{name}'");
            Fns[idx] = button.IsJustPressed;
        }
    }
    public void Poll()
    {
        for (int idx = 0; idx < Fns.Length; idx++)
        {
            bool val = Fns[idx]();
            if(val) Value = idx;
            else if(HasValue && Value == idx) val = true;
            Values[idx] = val;
        }
    }
    public bool TryGetValue(int value)
    {
        if (Values[value])
        {
            HasValue = false;
            return true;
        }
        else return false;
    }
}
