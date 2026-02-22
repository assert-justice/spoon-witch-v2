using System.Collections.Generic;
using Godot;

namespace SW.Src.GameSpace.Terrain;

public static class SwTerrainUtils
{
    public const int AtlasWidth = 4;
    public const int AtlasHeight = 4;
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
            MaskLookup.Add(new(1,3), (false, false, false, true)); // Outer bottom-right corner
            MaskLookup.Add(new(0,0), (false, false, true, false)); // Outer bottom-left corner
            MaskLookup.Add(new(0,2), (false, true, false, false)); // Outer top-right corner
            MaskLookup.Add(new(3,3), (true, false, false, false)); // Outer top-left corner
            MaskLookup.Add(new(1,0), (false, true, false, true)); // Right edge
            MaskLookup.Add(new(3,2), (true, false, true, false)); // Left edge
            MaskLookup.Add(new(3,0), (false, false, true, true)); // Bottom edge
            MaskLookup.Add(new(1,2), (true, true, false, false)); // Top edge
            MaskLookup.Add(new(1,1), (false, true, true, true)); // Inner bottom-right corner
            MaskLookup.Add(new(2,0), (true, false, true, true)); // Inner top-left corner
            MaskLookup.Add(new(2,2), (true, true, false, true)); // Inner top-right corner
            MaskLookup.Add(new(3,1), (true, true, true, false)); // Inner top-left corner
            MaskLookup.Add(new(2,3), (false, true, true, false)); // Bottom-left top-right corners
            MaskLookup.Add(new(0,1), (true, false, false, true)); // Top-left down-right corners
        }
        return MaskLookup.TryGetValue(ModVector(tileCoords), out mask);
    }
    public static TileSet InitTileSet(TileSet tileSet, int tileWidth, int tileHeight)
    {
        if(tileSet is null)
        {
            // GD.PrintErr("Null tile set. Saving new tile sets is not currently supported");
            tileSet = new();
        };
        tileSet.TileSize = new(tileWidth, tileHeight);
        if(tileSet.GetPhysicsLayersCount() == 0) tileSet.AddPhysicsLayer(0);
        return tileSet;
    }
    public static TileSet TileSetRemoveSources(TileSet tileSet)
    {
        for (int idx = 0; idx < tileSet.GetSourceCount(); idx++)
		{
			var id = tileSet.GetSourceId(idx);
			tileSet.RemoveSource(id);
		}
        return tileSet;
    }
    public static TileSetAtlasSource AddAtlas(TileSet tileSet, int widthInTiles, int heightInTiles, out int atlasId)
    {
        var image = Image.CreateEmpty(widthInTiles * tileSet.TileSize.X, heightInTiles * tileSet.TileSize.Y, false, Image.Format.Rgba8);
        Texture2D texture = ImageTexture.CreateFromImage(image);
        return AddAtlas(tileSet, texture, out atlasId);
    }
    public static TileSetAtlasSource AddAtlas(TileSet tileSet, Texture2D texture, out int atlasId)
    {
        TileSetAtlasSource atlas = new()
		{
			Texture = texture,
			TextureRegionSize = tileSet.TileSize
		};
        atlasId = tileSet.AddSource(atlas);
        return atlas;
    }
    public static TileData AddTile(TileSetAtlasSource atlas, Vector2I coord, bool setCollision = false)
    {
        atlas.CreateTile(coord);
        int tileWidth = atlas.TextureRegionSize.X;
        var tileData = atlas.GetTileData(coord, 0);
        if (setCollision)
        {
            tileData.AddCollisionPolygon(0);
            tileData.SetCollisionPolygonPoints(0, 0, 
            [
                new Vector2(-0.5f, -0.5f) * tileWidth,
                new Vector2(0.5f, -0.5f) * tileWidth,
                new Vector2(0.5f, 0.5f) * tileWidth,
                new Vector2(-0.5f, 0.5f) * tileWidth,
            ]);
        }
        return tileData;
    }
}
