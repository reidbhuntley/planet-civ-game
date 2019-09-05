using System.Collections.Generic;
using UnityEngine;

public class TileMap {
    private List<Tile> tiles;
    public IList<Tile> Tiles => tiles.AsReadOnly();

    private MapSettings mapSettings;
    private TileSettings tileSettings;

    public TileMap(Icosphere icos, MapSettings mapSettings, TileSettings tileSettings) {
        this.mapSettings = mapSettings;
        this.tileSettings = tileSettings;
        this.tiles = icos.GenerateTileMap(tileSettings.DEFAULT);
    }

    public void PopulateTiles() {
        NoiseFilter noise = new NoiseFilter(new NoiseSettings());

        foreach(Tile tile in tiles) {
            Vector3 center = tile.face.Center.normalized;
            float value = noise.Evaluate(center);
            
            if(value < mapSettings.percentLand) {
                tile.type = tileSettings.LAND;
            } else {
                tile.type = tileSettings.OCEAN;
            }
        }
    }

    public const int tileMapTextureResolution = 64;
    public void UpdateTileInfo() {
        int numTypes = TileSettings.numTypes*16;
        Texture2D texture = new Texture2D(tileMapTextureResolution, numTypes);
        
        Color[] colors = new Color[tileMapTextureResolution*numTypes];
        
        for(int v = 0; v < numTypes; v++) {
            for(int u = 0; u < tileMapTextureResolution; u++) {
                Color color = tileSettings.TileTypes[v/16].colors.Evaluate(u / (tileMapTextureResolution - 1f));
                colors[u + v*tileMapTextureResolution] = color;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        tileSettings.planetMaterial.SetTexture("_tileMapTexture", texture);
    }
}