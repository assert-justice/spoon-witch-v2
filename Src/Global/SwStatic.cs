using System.Collections.Generic;
using System.Text;
using Godot;

namespace SW.Src.Global;

public static class SwStatic
{
    public static bool IsEqual<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }
    public const float HALF_PI = Mathf.Pi * 0.5f;
    public static bool HasError{get; private set;} = false;
    private static readonly StringBuilder Sb = new();
    public static string Separator{get; set;} = "";
    public static string EndLine{get; set;} = "\n";
    private static string Format(object[] messages)
    {
        if(messages.Length == 0) return EndLine;
        Sb.Clear();
        Sb.Append(messages[0]?.ToString()??"[null]");
        for (int idx = 1; idx < messages.Length; idx++)
        {
            Sb.Append(Separator);
            Sb.Append(messages[idx]?.ToString()??"[null]");
        }
        Sb.Append(EndLine);
        return Sb.ToString();
    }
    public static void Log(params object[] messages)
    {
        GD.Print(Format(messages));
    }
    public static void LogError(params object[] messages)
    {
        GD.PushError(Format(messages));
        HasError = true;
    }
}
