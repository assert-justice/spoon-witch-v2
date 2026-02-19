using System.Collections.Generic;

namespace SW.Src.Global;

public static class SwStatic
{
    public static bool IsEqual<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }
}
