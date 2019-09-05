using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeGenerator {

    private ShapeSettings settings;
    private NoiseFilter[] noiseFilters;

    public MinMax elevationMinMax;

    public ShapeGenerator(ShapeSettings settings) {
        this.settings = settings;
        elevationMinMax = new MinMax();
        // noiseFilters = new NoiseFilter[settings.noiseLayers.Length];
        // for (int i = 0; i < noiseFilters.Length; i++) {
        //     noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i].noiseSettings);
        // }
    }

    public Vector3[] TransformVertices(IList<Vertex> vertices, TileMap map) {
        Vector3[] vectors = new Vector3[vertices.Count];

        List<Edge> coastalEdges = map.Tiles
            .Where(t => t.type.isLand)
            .SelectMany(t => t.face.edges
                .Where(e => e.Tiles
                    .Where(et => !et.type.isLand)
                    .Count() >= 1
                )
            ).ToList();

        for(int i = 0; i < vectors.Length; i++) {
            Vertex vertex = vertices[i];
            Vector3 pointOnUnitSphere = vertex.vector;
            float elevation = 0;
            if(vertex.Tiles.Where(t => t.type.isLand).Count() > 0) {
                if(coastalEdges.Count > 0) {
                    Edge closestEdge = coastalEdges[0];
                    float closestEdgeDist = float.MaxValue;
                    foreach(Edge edge in coastalEdges) {
                        float edgeDist = Vector3.Distance(pointOnUnitSphere, edge.Midpoint);
                        if(edgeDist < closestEdgeDist) {
                            closestEdge = edge;
                            closestEdgeDist = edgeDist;
                        }
                    }

                    float distToEdgeSquared;

                    Vector3 n = closestEdge.vertices[1].vector - closestEdge.vertices[0].vector;
                    Vector3 pa = closestEdge.vertices[0].vector - pointOnUnitSphere;
                
                    float c = Vector3.Dot(n, pa);
                
                    // Closest point is a
                    if ( c > 0.0f ) {
                        distToEdgeSquared = Vector3.Dot(pa, pa);
                    } else {
                        Vector3 bp = pointOnUnitSphere - closestEdge.vertices[1].vector;
                    
                        // Closest point is b
                        if(Vector3.Dot(n, bp) > 0.0f) {
                            distToEdgeSquared = Vector3.Dot(bp, bp);
                        } else {
                            // Closest point is between a and b
                            Vector3 e = pa - n * (c / Vector3.Dot(n, n));
                            distToEdgeSquared = Vector3.Dot(e, e);
                        }
                    }
                    
                    elevation += Mathf.Sqrt(distToEdgeSquared) * settings.mountainHeight;
                }

                //elevation += 0.02f;
            }

            float radius = settings.planetRadius * (1+elevation);
            vectors[i] = pointOnUnitSphere * radius;
            elevationMinMax.AddValue(radius);
        }

        return vectors;
        
        // Vector3 pointOnUnitSphere = vertex.vector;
        // float firstLayerValue = 0;
        // float elevation = 0;

        // if (noiseFilters.Length > 0) {
        //     firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
        //     if (settings.noiseLayers[0].enabled){
        //         elevation = firstLayerValue;
        //     }
        // }

        // for (int i = 1; i < noiseFilters.Length; i++) {
        //     if (settings.noiseLayers[i].enabled) {
        //         float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
        //         elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
        //     }
        // }
        // return pointOnUnitSphere * settings.planetRadius * (1+elevation);
    }
}