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
    public float waterThreshold = .5f;

    Mesh mesh;

    public WorldData GenerateTerrain(int seed) {
        WorldData worldData = new WorldData();
        VoxelData[,] voxelData = new VoxelData[size, size];

        float[,] noiseMap = Procedural.NoiseMap(seed, size, noiseScale, multiplier);
        float[,] falloffMap = Procedural.FalloffMap(size);

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                noiseMap[x, y] = Mathf.Max(0, noiseMap[x, y] - falloffMap[x, y]);
            }
        }

        bool[,] landFlags = new bool[size, size];
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                voxelData[x, y] = new VoxelData {
                    coords = new Vector2(x, y)
                };
                if (noiseMap[x, y] > waterThreshold) {
                    voxelData[x, y].isLand = true;
                    voxelData[x, y].navigable = true;
                }
                else {
                    voxelData[x, y].isOcean = true;
                }
                Voxel voxel = Instantiate(voxelObj, new Vector3(x, 0, y), Quaternion.identity, this.transform).GetComponent<Voxel>();
                voxel.Initialize(voxelData[x, y]);
            }
        }

        Dictionary<Vector2, VoxelData> voxels = new Dictionary<Vector2, VoxelData>();
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                voxels.Add(new Vector2(x, y), voxelData[x, y]);
            }
        }
        worldData.voxels = voxels;

        Dictionary<Vector2, bool> marked = new Dictionary<Vector2, bool>();
        List<List<Vector2>> bodiesOfWater = new List<List<Vector2>>();
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                marked.Add(new Vector2(x, y), false);
            }
        }
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Vector2 coords = new Vector2(x, y);
                if (!marked[coords] && voxels[coords].isOcean) {
                    Stack<Vector2> stack = new Stack<Vector2>();
                    List<Vector2> list = new List<Vector2>();
                    stack.Push(coords);
                    while (stack.Count > 0) {
                        Vector2 v = stack.Pop();
                        if (!marked[v]) {
                            list.Add(v);
                            marked[v] = true;
                            foreach (Vector2 adj in worldData.GetAdjacent(v, true)) {
                                if (voxels[adj].isOcean) {
                                    stack.Push(adj);
                                }
                            }
                        }
                    }
                    bodiesOfWater.Add(list);
                }
            }
        }
        foreach (List<Vector2> body in bodiesOfWater) {
            if (body.Count < 100) {
                foreach (Vector2 v in body) {
                    voxels[v].isOcean = false;
                    voxels[v].isLake = true;
                }
            }
        }

        MeshData meshData = Procedural.GenerateTerrain(voxelData);
        meshFilter.mesh = meshData.CreateMesh();

        float[,] colorNoise = Procedural.NoiseMap(seed + 1, size, ObjectGenerator.treeNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));
        meshRenderer.sharedMaterial.mainTexture = Procedural.GenerateTexture(seed, worldData, noiseMap, colorNoise);

        navSurface.BuildNavMesh();
        
        return worldData;
    }
}