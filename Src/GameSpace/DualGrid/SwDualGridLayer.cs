using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.DualGrid;

[Tool]
public partial class SwDualGridLayer : TileMapLayer
{
    private readonly Dictionary<Vector2I, int> GridData = [];
    private static readonly Vector2I[] Neighbors = [Vector2I.Zero, Vector2I.Right, Vector2I.Down, Vector2I.One];
    private SwTileCoordLookup CoordLookup = new();
    private void RefreshTile(Vector2I tilePos, int sourceId)
    {
        GD.Print("Refresh tile");
        foreach (var delta in Neighbors)
        {
            Vector2I displayTilePos = tilePos + delta;
            // get atlas coords
            var mask = GetMask(displayTilePos);
            GD.Print(mask);
            int oldId = GetCellSourceId(displayTilePos);
            if(CoordLookup.TryGetAtlasCoords(mask, sourceId, out var atlasCoords)){}
            else if(CoordLookup.TryGetAtlasCoords(mask, oldId, out atlasCoords)){sourceId = oldId;}
            else atlasCoords = new(-1,-1);
            GD.Print(atlasCoords);
            SetCell(displayTilePos, sourceId, atlasCoords);
        }
    }
    private (bool, bool, bool, bool) GetMask(Vector2I displayTilePos)
    {
        bool br = IsTileSolid(displayTilePos - Neighbors[0]);
        bool bl = IsTileSolid(displayTilePos - Neighbors[1]);
        bool tr = IsTileSolid(displayTilePos - Neighbors[2]);
        bool tl = IsTileSolid(displayTilePos - Neighbors[3]);
        return(tl, tr, bl, br);
    }
    private bool IsTileSolid(Vector2I tilePos)
    {
        return GridData.TryGetValue(tilePos, out int sourceId) && sourceId != -1;
    }
    public void Init(SwTileCoordLookup coordLookup)
    {
        CoordLookup = coordLookup;
    }
    public void SetTile(Vector2I tilePos, int sourceId = -1)
    {
        GD.Print("Set tile");
        if(sourceId == -1) GridData.Remove(tilePos);
        else GridData[tilePos] = sourceId;
        RefreshTile(tilePos, sourceId);
    }
    // public bool TryGetTile(Vector2I tilePos, out int sourceId)
    // {
    //     sourceId = -1;
    //     if(!GridData.TryGetValue(tilePos, out int type)) return false;
    //     sourceId = type;
    //     return true;
    // }
    public new void Clear()
    {
        base.Clear();
        GridData.Clear();
    }
}
