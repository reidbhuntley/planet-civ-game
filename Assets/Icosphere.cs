using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Icosphere {
    public readonly float radius = 1f;
    private List<Vertex> vertices;
    private List<TriFace> faces;
    private Dictionary<Edge, Vertex> midpointsCache;

    public Icosphere() {
        vertices = new List<Vertex>();
        faces = new List<TriFace>(20);
        midpointsCache = new Dictionary<Edge, Vertex>();

        // Create initial vertices from lookup table
        for(int i = 0; i < 12; i++) {
            CreateVertex(new Vector3(
                initialVertices[i,0], initialVertices[i,1], initialVertices[i,2])
            );
        }

        // Create initial faces from lookup table
        for(int i = 0; i < 20; i++) {
            faces.Add(new TriFace(
                vertices[initialFaces[i,0]], vertices[initialFaces[i,1]], vertices[initialFaces[i,2]]
            ));
        }
    }

    public IList<Vertex> Vertices => vertices.AsReadOnly();
    public int[] Triangles => faces.SelectMany(f => f.vertices.Select(v => v.id)).ToArray();


    public void Subdivide(int recursions) {
        for(int r = 0; r < recursions; r++) {
            var newFaces = new List<TriFace>(faces.Count*4);

            // Subdivide each face
            foreach (var face in faces) {
                List<Vertex> midpoints = new List<Vertex>(3);

                // Get the midpoint of each edge
                foreach(Edge edge in face.edges) {
                    Vertex midpoint;

                    // Use the midpoint if it already exists; otherwise create a new midpoint
                    if(!midpointsCache.TryGetValue(edge, out midpoint)) {
                        midpoint = CreateVertex(edge.Midpoint);
                        midpointsCache.Add(edge, midpoint);

                        // Link the new midpoint to the tile(s) linked to the edge it was created from
                        foreach(Tile edgeTile in edge.Tiles) {
                            midpoint.AddTile(edgeTile);
                        }
                    }
                    midpoints.Add(midpoint);
                }
                
                // Create the four new faces from the initial vertices and the new midpoints
                for(int i = 0; i < 3; i++) {
                    newFaces.Add(new TriFace(face.vertices[i], midpoints[i], midpoints[(i+2)%3]));
                }
                newFaces.Add(new TriFace(midpoints[0], midpoints[1], midpoints[2]));
            }

            // Replace the old faces with the new subdivided ones
            faces = newFaces;
        }
    }


    public List<Tile> GenerateTileMap(TileType defaultTileType) {
        List<Tile> tileMap = new List<Tile>(faces.Count);

        // Create a tile for each face and link it to the vertices of that face
        foreach(TriFace face in faces) {
            Tile tile = new Tile(face, defaultTileType);
            tileMap.Add(tile);

            foreach(Vertex vertex in face.vertices) {
                vertex.AddTile(tile);
            }
        }

        // Create connections between tiles that share an edge
        foreach(Tile tile in tileMap) {
            tile.ComputeNeighbors();
        }

        // List<Vertex> oldVertices = new List<Vertex>(vertices);
        // foreach(Vertex vertex in oldVertices) {
        //     IList<Tile> tiles = vertex.Tiles;
        //     var facesToUpdate = faces.Where(f => f.vertices.Contains(vertex));
        //     for(int i = 0; i < tiles.Count; i++) {
        //         Tile thisTile = tiles[i];
        //         Vertex newVertex = CreateVertex(vertex.vector);
        //         List<Tile> newTiles = new List<Tile>(tiles);
        //         newTiles.RemoveAt(i);
        //         newVertex.AddTile(thisTile);
        //         foreach(Tile newTile in newTiles) newVertex.AddTile(newTile);

        //         Vertex[] verticesToUpdate = facesToUpdate.Where(f => f.Tile == thisTile).Single().vertices;
        //         int indexToUpdate = verticesToUpdate.ToList().IndexOf(vertex);
        //         verticesToUpdate[indexToUpdate] = newVertex;
        //     }
        // }

        return tileMap;
    }


    private Vertex CreateVertex(Vector3 vector) {
        Vertex vertex = new Vertex(vector.normalized, vertices.Count);
        vertices.Add(vertex);
        return vertex;
    }

    private static float t = (1.0f + Mathf.Sqrt (5.0f)) / 2.0f;
    private static float[,] initialVertices = {
        {-1, t, 0}, {1, t, 0}, {-1, -t, 0}, {1, -t, 0},
        {0, -1, t}, {0, 1, t}, {0, -1, -t}, {0, 1, -t},
        {t, 0, -1}, {t, 0, 1}, {-t, 0, -1}, {-t, 0, 1}
    };
    private static int[,] initialFaces =  {
        {0, 11, 5}, {0, 5, 1},  {0, 1, 7},   {0, 7, 10}, {0, 10, 11},
        {1, 5, 9},  {5, 11, 4}, {11, 10, 2}, {10, 7, 6}, {7, 1, 8},
        {3, 9, 4},  {3, 4, 2},  {3, 2, 6},   {3, 6, 8},  {3, 8, 9},
        {4, 9, 5},  {2, 4, 11}, {6, 2, 10},  {8, 6, 7},  {9, 8, 1}
    };

}