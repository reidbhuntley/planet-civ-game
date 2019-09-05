using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriFace {
    public readonly Vertex[] vertices;
    public readonly Edge[] edges;

    private Vector3 center;
    public Vector3 Center {
        get {
            if(center == Vector3.zero) {
                center = (vertices[0].vector + vertices[1].vector + vertices[2].vector)/3f;
            }
            return center;
        }
    }

    private Tile tile;
    public Tile Tile {
        get {
            if(tile == null) {
                tile = vertices[0].Tiles
                    .Intersect(vertices[1].Tiles)
                    .Intersect(vertices[2].Tiles)
                    .SingleOrDefault();
            }
            return tile;
        }
    }

    public TriFace(Vertex a, Vertex b, Vertex c) {
        vertices = new[] {a, b, c};
        edges = new[] {new Edge(a,b), new Edge(b,c), new Edge(c,a)};
        center = Vector3.zero;
    }
    
}