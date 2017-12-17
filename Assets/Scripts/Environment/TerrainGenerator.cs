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

    public VoxelData[,] GenerateVoxelData(int seed) {
        System.Random rng = new System.Random(seed);

        VoxelData[,] voxelData = new VoxelData[size, size];

        float[,] landNoise = Procedural.NoiseMap(seed, size, noiseScale, multiplier);
        float[,] colorNoise = Procedural.NoiseMap(seed + 1, size, ObjectGenerator.treeNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));
        float[,] falloffMap = Procedural.FalloffMap(size);

        //Determine what is land
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                landNoise[x, y] = Mathf.Max(0, landNoise[x, y] - falloffMap[x, y]);
            }
        }

        //Set corresponding voxel values for land
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                voxelData[x, y] = new VoxelData {
                    coords = new Coords(x, y)
                };
                if (landNoise[x, y] > waterThreshold) {
                    voxelData[x, y].isLand = true;
                    voxelData[x, y].navigable = true;
                }
                else {
                    voxelData[x, y].isOcean = true;
                }
            }
        }

        //Find and mark lakes
        bool[,] marked = new bool[size, size];
        List<List<Coords>> bodiesOfWater = new List<List<Coords>>();
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                marked[x, y] = false;
            }
        }
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                if (!marked[x, y] && voxelData[x, y].isOcean) {
                    Stack<Coords> stack = new Stack<Coords>();
                    List<Coords> list = new List<Coords>();
                    stack.Push(new Coords(x, y));
                    while (stack.Count > 0) {
                        Coords v = stack.Pop();
                        if (!marked[(int)v.x, (int)v.y]) {
                            list.Add(v);
                            marked[(int)v.x, (int)v.y] = true;
                            foreach (Coords adj in Util.GetAdjacent(v, true)) {
                                if (voxelData[(int)adj.x, (int)adj.y].isOcean) {
                                    stack.Push(adj);
                                }
                            }
                        }
                    }
                    bodiesOfWater.Add(list);
                }
            }
        }
        foreach (List<Coords> body in bodiesOfWater) {
            if (body.Count < 100) {
                foreach (Coords v in body) {
                    voxelData[v.x, v.y].isOcean = false;
                    voxelData[v.x, v.y].isLake = true;
                }
            }
        }

        //Set voxel colors
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                if (voxelData[x, y].isLand) {
                    Color baseColor = Color.Lerp(Palette.LandMin, Palette.LandMax, rng.Next(0, 1000) / 1000f);
                    voxelData[x, y].color = SerializableColor.FromColor(Color.Lerp(baseColor, Palette.Chartreuse, Mathf.Pow(colorNoise[x, y] * landNoise[x, y], 2)));
                }
                else {
                    voxelData[x, y].color = SerializableColor.FromColor(Palette.Water);
                }
            }
        }

        //Set beaches and coasts
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Coords coords = new Coords(x, y);
                if (voxelData[x, y].isLand && Util.AdjacentProperty(Util.CoordsToVoxels(voxelData, Util.GetAdjacent(coords, true)), "isOcean") && colorNoise[x, y] > .5f) {
                    voxelData[x, y].color = SerializableColor.FromColor(Color.Lerp(voxelData[x, y].color.ToColor(), Palette.Sand, Mathf.Pow(colorNoise[x, y], 2)));
                }
                else if (voxelData[x, y].isOcean && Util.AdjacentProperty(Util.CoordsToVoxels(voxelData, Util.GetAdjacent(coords, true)), "isLand")) {
                    voxelData[x, y].color = SerializableColor.FromColor(Color.Lerp(voxelData[x, y].color.ToColor(), Palette.Coast, .1f));
                }
            }
        }

        return voxelData;
    }

    public void GenerateTerrain(WorldData worldData) {
        MeshData meshData = Procedural.MeshFromVoxelData(worldData.voxelData);
        meshFilter.mesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = Procedural.TextureFromVoxelData(worldData.voxelData);
        foreach(VoxelData v in worldData.voxelData) {
            Voxel voxel = Instantiate(voxelObj, Util.CoordsToVector3(v.coords), Quaternion.identity, this.transform).GetComponent<Voxel>();
            voxel.Initialize(v);
        }
        navSurface.BuildNavMesh();
    }
}