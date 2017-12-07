using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class TerrainGenerator : MonoBehaviour {

    public static float noiseScale = .1f;
    public static int size = 100;

    public GameObject voxelObj;
    public AnimationCurve multiplier;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public NavMeshSurface navSurface;
    public int seed = 0;
    public float waterThreshold = .5f;

    [HideInInspector]
    public float[,] noiseMap;
    [HideInInspector]
    public bool finished;

    GameController gameController;
    Mesh mesh;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Generate();
    }

    void Generate() {
        Dictionary<Vector2, Voxel> grid = gameController.grid;

        noiseMap = Procedural.NoiseMap(seed, size, noiseScale, multiplier);
        float[,] falloffMap = Procedural.FalloffMap(size);

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                noiseMap[x, y] = Mathf.Max(0, noiseMap[x, y] - falloffMap[x, y]);
            }
        }

        bool[,] landFlags = new bool[size, size];
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                landFlags[x, y] = noiseMap[x, y] > waterThreshold;
            }
        }

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Voxel voxel = Instantiate(voxelObj, new Vector3(x + .5f, -.5f, y + .5f), Quaternion.identity, this.transform).GetComponent<Voxel>();
                voxel.gameObject.name = x + ", " + y;
                Vector2 coords = new Vector2(x, y);
                voxel.Initialize(coords, landFlags[x, y] ? 1 : 0);
                grid.Add(coords, voxel);
            }
        }

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Vector2 coords = new Vector2(x, y);
                Voxel voxel = grid[coords];
                if(x > 0) {
                    voxel.adjacent.Add(grid[coords + Vector2.left]);
                    voxel.neighbors.Add(grid[coords + Vector2.left]);
                    if (y > 0)
                        voxel.neighbors.Add(grid[coords - Vector2.one]);
                    if (y < size - 1)
                        voxel.neighbors.Add(grid[coords + new Vector2(-1, 1)]);
                }
                if(x < size - 1) {
                    voxel.adjacent.Add(grid[coords + Vector2.right]);
                    voxel.neighbors.Add(grid[coords + Vector2.right]);
                    if (y > 0)
                        voxel.neighbors.Add(grid[coords + new Vector2(1, -1)]);
                    if (y < size - 1)
                        voxel.neighbors.Add(grid[coords + Vector2.one]);
                }
                if (y > 0) {
                    voxel.adjacent.Add(grid[coords + Vector2.down]);
                    voxel.neighbors.Add(grid[coords + Vector2.down]);
                }
                if (y < size - 1) {
                    voxel.adjacent.Add(grid[coords + Vector2.up]);
                    voxel.neighbors.Add(grid[coords + Vector2.up]);
                }
            }
        }

        foreach(Voxel voxel in grid.Values) {
            if(!voxel.visited && voxel.type == 0) {
                Stack<Voxel> stack = new Stack<Voxel>();
                List<Voxel> visited = new List<Voxel>();
                Voxel c = voxel;
                stack.Push(c);
                visited.Add(c);
                voxel.visited = true;
                int i = 1;
                while(stack.Count > 0) {
                    c = stack.Pop();
                    foreach(Voxel adj in c.adjacent) {
                        if(!adj.visited && adj.type == 0) {
                            adj.visited = true;
                            i++;
                            stack.Push(adj);
                            visited.Add(adj);
                        }
                    }
                }
                if(i < 100) {
                    foreach(Voxel v in visited) {
                        v.type = 2;
                    }
                }
            }
        }

        MeshData meshData = Procedural.GenerateTerrain(landFlags);
        meshFilter.mesh = meshData.CreateMesh();

        float[,] colorNoise = Procedural.NoiseMap(seed + 1, size, ObjectGenerator.treeNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));
        meshRenderer.sharedMaterial.mainTexture = Procedural.GenerateTexture(seed, landFlags, noiseMap, colorNoise, grid);

        navSurface.BuildNavMesh();

        gameController.grid = grid;

        finished = true;
    }

    int WaterSize(Voxel voxel) {
        int i = 0;
        voxel.visited = true;
        foreach(Voxel adj in voxel.adjacent) {
            if(!adj.visited && adj.type == 0) {
                i++;
                i += WaterSize(adj);
            }
        }
        return i;
    }

    void MarkLake(Voxel voxel) {
        voxel.type = 2;
        foreach(Voxel adj in voxel.adjacent) {
            if(adj.type == 0) {
                MarkLake(adj);
            }
        }
    }
}