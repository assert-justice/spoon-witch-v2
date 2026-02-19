using Godot;

namespace SW.Src.GameSpace.DualGrid;

public static class SwTileMapUtils
{
    public static TileSet InitTileSet(int tileWidth)
    {
        TileSet tileSet = new()
        {
            TileSize = new(tileWidth, tileWidth)
        };
        tileSet.AddPhysicsLayer(0);
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
