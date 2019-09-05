using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Planet : MonoBehaviour {
    
    [Range(0, 4)]
    public int mapSubdivisions = 3;
    [Range(0, 4)]
    public int meshSubdivisions = 2;
    public bool autoUpdate = true;
    
    public ShapeSettings shapeSettings;
    public MapSettings mapSettings;
    public TileSettings tileSettings;

    [HideInInspector]
    public bool tileSettingsFoldout, shapeSettingsFoldout, mapSettingsFoldout;
    
    private ShapeGenerator shapeGenerator;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private Icosphere icos;
    private TileMap map;

    public void Initialize() {
        shapeGenerator = new ShapeGenerator(shapeSettings);
        if(meshRenderer == null) {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }
        meshRenderer.sharedMaterial = tileSettings.planetMaterial;
        if(meshFilter == null) {
            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
        }
        if(mesh == null) {
            mesh = meshFilter.sharedMesh;
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        if(icos == null) {
            GenerateIcosphere();
        }
    }

    public void GeneratePlanet() {
        icos = null;
        Initialize();
        GenerateMap();
        GenerateMesh();
        GenerateTileInfo();
    }

    public void OnTileSettingsUpdated() {
        if(autoUpdate) {
            Initialize();
            GenerateTileInfo();
        }
    }

    public void OnShapeSettingsUpdated() {
        if(autoUpdate) {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnMapSettingsUpdated() {
        if(autoUpdate) {
            Initialize();
            GenerateMap();
            GenerateMesh();
        }
    }

    public void GenerateIcosphere() {
        icos = new Icosphere();
        icos.Subdivide(mapSubdivisions);
        map = new TileMap(icos, mapSettings, tileSettings);
        icos.Subdivide(meshSubdivisions);
    }

    public void GenerateMap() {
        map.PopulateTiles();
    }

    public void GenerateMesh() {
        mesh.Clear();
        mesh.vertices = shapeGenerator.TransformVertices(icos.Vertices, map);
        mesh.triangles = icos.Triangles;
        mesh.uv = (Vector2[])icos.Vertices
            .Select(v => new Vector2((v.Tiles
                .Select(t => t.type)
                .Aggregate(v.Tiles[0].type, (prev,next) => (next.id > prev.id) ? next : prev).id + 0.5f) / (float)(TileSettings.numTypes)
            , 0)).ToArray().Clone();
        mesh.RecalculateNormals();

        tileSettings.planetMaterial.SetVector("_elevationMinMax", new Vector4(shapeGenerator.elevationMinMax.Min, shapeGenerator.elevationMinMax.Max));
    }

    public void GenerateTileInfo() {
        map.UpdateTileInfo();
    }
}