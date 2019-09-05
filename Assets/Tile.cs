using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile {
    public TileType type;

    public readonly TriFace face;

    private List<Tile> neighbors;
    public IList<Tile> Neighbors { 
        get { 
            if(neighbors == null) {
                throw new System.Exception("Can't get neighbors before they've been computed");
            }
            return neighbors.AsReadOnly();
        }
    }

    public Tile(TriFace face, TileType type) {
        this.face = face;
        this.type = type;
    }

    public void ComputeNeighbors() {
        neighbors = new List<Tile>(3);
        foreach(Edge edge in face.edges) {
            if(edge.Tiles.Count != 2) {
                throw new System.Exception("Edge should be associated with exactly 2 tiles");
            }

            Tile neighbor;
            if(this == edge.Tiles[0]) {
                neighbor = edge.Tiles[1];
            } else if(this == edge.Tiles[1]) {
                neighbor = edge.Tiles[0];
            } else {
                throw new System.Exception("Edge should be associated with this tile");
            }
            neighbors.Add(neighbor);
        }
    }

    public bool IsNeighbor(Tile other) {
        return Neighbors.Contains(other);
    }
}