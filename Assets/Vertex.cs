using System.Collections.Generic;
using UnityEngine;

public class Vertex {
    public readonly int id;
    public readonly Vector3 vector;
    private List<Tile> tiles;
    public IList<Tile> Tiles {
        get { return tiles.AsReadOnly(); }  
    }

    public Vertex(Vector3 vector, int id) {
        this.vector = vector;
        this.id = id;
        this.tiles = new List<Tile>(6);
    }

    public void AddTile(Tile tile) {
        if(tiles.Count >= 6) throw new System.Exception("Vertex should not be associated with more than 6 tiles");
        tiles.Add(tile);
    }

    public override bool Equals(object value) {
        if(Object.ReferenceEquals(null, value)) return false;
        if(Object.ReferenceEquals(this, value)) return true;
        if(value.GetType() != this.GetType()) return false;
        return Equals((Vertex) value);
    }

    public bool Equals(Vertex vertex) {
        return id == vertex.id;
    }

    public override int GetHashCode() {
        return id;
    }
}