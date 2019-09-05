using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType {
    public readonly int id;
    public Gradient colors;
    public bool isLand;

    public TileType(int id) {
        this.id = id;
    }
}