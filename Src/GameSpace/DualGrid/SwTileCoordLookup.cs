using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.DualGrid;

public class SwTileCoordLookup
{
    private class SwTileMaskCoords
    {
        private readonly Dictionary<int, List<Vector2I>> TypeLookup = [];
        public void AddCoords(int tileType, Vector2I tileCoords)
        {
            if(!TypeLookup.TryGetValue(tileType, out var options))
            {
                options = [];
                TypeLookup.Add(tileType, options);
            }
            options.Add(tileCoords);
        }
        public bool TryGetOption(int tileType, out Vector2I tileCoords)
        {
            tileCoords = default;
            if(!TypeLookup.TryGetValue(tileType, out var options)) return false;
            if(options.Count == 0) return false;
            int idx = Mathf.FloorToInt(GD.Randf() * options.Count);
            tileCoords = options[idx];
            return true;
        }
    }
    private readonly Dictionary<(bool, bool, bool, bool), SwTileMaskCoords> MaskLookup = [];
    public void AddCoords((bool, bool, bool, bool) mask, int tileType, Vector2I tileCoords)
    {
        if(!MaskLookup.TryGetValue(mask, out var maskCoords))
        {
            maskCoords = new();
            MaskLookup.Add(mask, maskCoords);
        }
        maskCoords.AddCoords(tileType, tileCoords);
    }
    public void Clear(){MaskLookup.Clear();}
}
