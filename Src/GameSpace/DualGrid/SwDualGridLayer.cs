using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.DualGrid;

[Tool]
public partial class SwDualGridLayer : TileMapLayer
{
    [Export] private int[] SolidCells{get=>Save(); set=>Load(value);}
    private readonly Dictionary<Vector2I, int> GridData = [];
    private static readonly Vector2I[] Neighbors = [Vector2I.Zero, Vector2I.Right, Vector2I.Down, Vector2I.One];
    private SwDualGrid Parent;
    public override void _Ready()
    {
        base._Ready();
        Parent = GetParent().GetParent<SwDualGrid>();
    }
    private void Load(int[] ints)
    {
        GridData.Clear();
        for (int idx = 0; idx < ints.Length; idx+=3)
        {
            GridData.Add(new(ints[idx], ints[idx+1]), ints[idx + 2]);
        }
    }
    private int[] Save()
    {
        int[] res = new int[GridData.Count * 3];
        int idx = 0;
        foreach (var (key, value) in GridData)
        {
            res[idx] = key.X;
            res[idx+1] = key.Y;
            res[idx+2] = value;
            idx += 3;
        }
        return res;
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
        bool br = IsTileFilled(displayTilePos - Neighbors[0]);
        bool bl = IsTileFilled(displayTilePos - Neighbors[1]);
        bool tr = IsTileFilled(displayTilePos - Neighbors[2]);
        bool tl = IsTileFilled(displayTilePos - Neighbors[3]);
        return(tl, tr, bl, br);
    }
    public bool IsTileFilled(Vector2I tilePos)
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
    public int GetTileId(Vector2I tilePos)
    {
        if(GridData.TryGetValue(tilePos, out var id)) return id;
        return -1;
    }
}
