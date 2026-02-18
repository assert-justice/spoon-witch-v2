using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.DualGrid;

public static class SwMaskLookup
{
    private const int AtlasWidth = 4;
    private const int AtlasHeight = 4;
    private static readonly Dictionary<Vector2I, (bool, bool, bool, bool)> MaskLookup = [];
    private static Vector2I ModVector(Vector2I vector)
    {
        if(vector.X < 0 ||  vector.Y < 0) return vector;
        int x = vector.X % AtlasWidth;
        int y = vector.Y % AtlasHeight;
        return new(x, y);
    }
    public static bool TryGetMask(Vector2I tileCoords, out (bool, bool, bool, bool) mask)
    {
        if(MaskLookup.Count == 0)
        {
            MaskLookup.Add(new(2,1), (true, true, true, true)); // All corners
            MaskLookup.Add(new(1,3), (true, true, true, true)); // Outer bottom-right corner
            MaskLookup.Add(new(0,0), (true, true, true, true)); // Outer bottom-left corner
            MaskLookup.Add(new(0,2), (true, true, true, true)); // Outer top-right corner
            MaskLookup.Add(new(3,3), (true, true, true, true)); // Outer top-left corner
            MaskLookup.Add(new(1,0), (true, true, true, true)); // Right edge
            MaskLookup.Add(new(3,2), (true, true, true, true)); // Left edge
            MaskLookup.Add(new(3,0), (true, true, true, true)); // Bottom edge
            MaskLookup.Add(new(1,2), (true, true, true, true)); // Top edge
            MaskLookup.Add(new(1,1), (true, true, true, true)); // Inner bottom-right corner
            MaskLookup.Add(new(2,0), (true, true, true, true)); // Inner top-left corner
            MaskLookup.Add(new(2,2), (true, true, true, true)); // Inner top-right corner
            MaskLookup.Add(new(3,1), (true, true, true, true)); // Inner top-left corner
            MaskLookup.Add(new(2,3), (true, true, true, true)); // Bottom-left top-right corners
            MaskLookup.Add(new(0,1), (true, true, true, true)); // Top-left down-right corners
        }
        return MaskLookup.TryGetValue(ModVector(tileCoords), out mask);
    }
}
