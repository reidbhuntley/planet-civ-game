using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Edge {
    public readonly Vertex[] vertices;
    public readonly int id;

    private Vector3 midpoint;
    public Vector3 Midpoint {
        get {
            if(midpoint == Vector3.zero) {
                midpoint = (vertices[0].vector + vertices[1].vector)/2f;
            }
            return midpoint;
        }
    }

    private List<Tile> tiles;
    public IList<Tile> Tiles {
        get {
            if(tiles == null) {
                tiles = vertices[0].Tiles
                    .Intersect(vertices[1].Tiles)
                    .ToList();
                if(tiles.Count > 2) throw new System.Exception("Edge should not be associated with more than 2 tiles");
            }
            return tiles.AsReadOnly();
        }
    }

    public Edge(Vertex a, Vertex b) {
        if(a.Equals(b)) throw new System.ArgumentException("Can't create an edge with identical vertices");
        
        vertices = (a.GetHashCode() < b.GetHashCode()) ? new[] {a, b} : new[] {b, a};
        id = (vertices[0].GetHashCode() << 16) ^ vertices[1].GetHashCode();
        midpoint = Vector3.zero;
    }

    public override bool Equals(object value) {
        if(object.ReferenceEquals(null, value)) return false;
        if(object.ReferenceEquals(this, value)) return true;
        if(value.GetType() != this.GetType()) return false;
        return Equals((Edge) value);
    }

    public bool Equals(Edge edge) {
        return vertices[0] == edge.vertices[0] && vertices[1] == edge.vertices[1];
    }

    public override int GetHashCode() {
        return id;
    }
}