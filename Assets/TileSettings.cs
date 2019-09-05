using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TileSettings : ScriptableObject {
    public Material planetMaterial;

    public const int numTypes = 3;
    public TileType
        DEFAULT = new TileType(0),
        OCEAN = new TileType(1),
        LAND = new TileType(2)
    ;

    public TileType[] TileTypes => new[] {
        DEFAULT,
        OCEAN,
        LAND
    };
    
}