using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.Terrain;

public class SwTileCoordLookup
{
    private class SwTileMaskCoords
    {
        private readonly Dictionary<int, List<Vector2I>> TypeLookup = [];
        public void AddCoords(int sourceId, Vector2I tileCoords)
        {
            if(!TypeLookup.TryGetValue(sourceId, out var options))
            {
                options = [];
                TypeLookup.Add(sourceId, options);
            }
            options.Add(tileCoords);
        }
        public bool TryGetOption(int sourceId, out Vector2I tileCoords)
        {
            tileCoords = default;
            if(!TypeLookup.TryGetValue(sourceId, out var options)) return false;
            if(options.Count == 0) return false;
            int idx = Mathf.FloorToInt(GD.Randf() * options.Count);
            tileCoords = options[idx];
            return true;
        }
    }
    private readonly Dictionary<(bool, bool, bool, bool), SwTileMaskCoords> MaskLookup = [];
    public void AddCoords((bool, bool, bool, bool) mask, int sourceId, Vector2I tileCoords)
    {
        if(!MaskLookup.TryGetValue(mask, out var maskCoords))
        {
            maskCoords = new();
            MaskLookup.Add(mask, maskCoords);
        }
        maskCoords.AddCoords(sourceId, tileCoords);
    }
    public void Clear(){MaskLookup.Clear();}
    public bool TryGetAtlasCoords((bool, bool, bool, bool) mask, int sourceId, out Vector2I atlasCoords)
    {
        atlasCoords = default;
        if(!MaskLookup.TryGetValue(mask, out var lookup)) return false;
        return lookup.TryGetOption(sourceId, out atlasCoords);
    }
}
