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

    Mesh mesh;

    public World GenerateWorldState() {
        World world = new World(size);

        float[,] noiseMap = Procedural.NoiseMap(seed, size, noiseScale, multiplier);
        float[,] falloffMap = Procedural.FalloffMap(size);

        world.noiseMap = noiseMap;

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
                if (landFlags[x, y])
                    world.SetProperty(x, y, "isLand", true);
                else {
                    world.SetProperty(x, y, "isOcean", true);
                    world.SetProperty(x, y, "innavigable", true);
                }
            }
        }

        Dictionary<Vector2, bool> marked = new Dictionary<Vector2, bool>();
        List<List<Vector2>> bodiesOfWater = new List<List<Vector2>>();
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                marked.Add(new Vector2(x, y), false);
            }
        }
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Vector2 coords = new Vector2(x, y);
                if (!marked[coords] && world.GetProperty(x, y, "isOcean")) {
                    Stack<Vector2> stack = new Stack<Vector2>();
                    List<Vector2> list = new List<Vector2>();
                    stack.Push(coords);
                    while (stack.Count > 0) {
                        Vector2 v = stack.Pop();
                        if (!marked[v]) {
                            list.Add(v);
                            marked[v] = true;
                            foreach (Vector2 adj in world.GetAdjacent((int)v.x, (int)v.y, true)) {
                                if (world.GetProperty((int)adj.x, (int)adj.y, "isOcean")) {
                                    stack.Push(adj);
                                }
                            }
                        }
                    }
                    bodiesOfWater.Add(list);
                }                
            }
        }
        foreach(List<Vector2> body in bodiesOfWater) {
            if(body.Count < 100) {
                foreach(Vector2 v in body) {
                    world.SetProperty((int)v.x, (int)v.y, "isOcean", false);
                    world.SetProperty((int)v.x, (int)v.y, "isLake", true);
                }
                Debug.Log(body.Count);
            }
        }

        return world;
    }

    public void GenerateTerrain(World world) {
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Voxel voxel = Instantiate(voxelObj, new Vector3(x, 0, y), Quaternion.identity, this.transform).GetComponent<Voxel>();
                voxel.gameObject.name = x + ", " + y;
                voxel.gameObject.layer = world.GetProperty(x, y, "isLand") ? 8 : 4;
            }
        }

        MeshData meshData = Procedural.GenerateTerrain(world);
        meshFilter.mesh = meshData.CreateMesh();

        float[,] colorNoise = Procedural.NoiseMap(seed + 1, size, ObjectGenerator.treeNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));
        meshRenderer.sharedMaterial.mainTexture = Procedural.GenerateTexture(seed, world, world.noiseMap, colorNoise);

        navSurface.BuildNavMesh();
    }
}