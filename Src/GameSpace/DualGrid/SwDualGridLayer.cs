using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.DualGrid;

[Tool]
public partial class SwDualGridLayer : TileMapLayer
{
    private readonly Dictionary<Vector2I, int> GridData = [];
    private static readonly Vector2I[] Neighbors = [Vector2I.Zero, Vector2I.Right, Vector2I.Down, Vector2I.One];
    private SwDualGrid Parent;
    public override void _Ready()
    {
        base._Ready();
        Parent = GetParent().GetParent<SwDualGrid>();
    }
    private void RefreshSetTile(Vector2I tilePos, int sourceId)
    {
        SetCell(tilePos, sourceId, Vector2I.Zero);
        foreach (var delta in Neighbors)
        {
            Vector2I displayTilePos = tilePos + delta;
            var mask = GetMask(displayTilePos);
            int oldId = GetCellSourceId(displayTilePos);
            if(Parent.TryGetAtlasCoords(mask, sourceId, out var atlasCoords)){}
            else if(Parent.TryGetAtlasCoords(mask, oldId, out atlasCoords)){sourceId = oldId;}
            else atlasCoords = new(-1,-1);
            SetCell(displayTilePos, sourceId, atlasCoords);
        }
    }
    private void RefreshClearedTile(Vector2I tilePos)
    {
        foreach (var delta in Neighbors)
        {
            Vector2I displayTilePos = tilePos + delta;
            var mask = GetMask(displayTilePos);
            int oldId = GetCellSourceId(displayTilePos);
            if(Parent.TryGetAtlasCoords(mask, oldId, out var atlasCoords)){}
            else atlasCoords = new(-1,-1);
            SetCell(displayTilePos, oldId, atlasCoords);
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
    public void SetTile(Vector2I tilePos, int sourceId)
    {
        GridData[tilePos] = sourceId;
        RefreshSetTile(tilePos, sourceId);
    }
    public void ClearTile(Vector2I tilePos)
    {
        GridData.Remove(tilePos);
        RefreshClearedTile(tilePos);
    }
    public void ClearContents()
    {
        Clear();
        GridData.Clear();
    }
}
