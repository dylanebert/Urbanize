using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectGenerator : MonoBehaviour {

    public static float treesPerVoxel = 2f;
    public static float treeNoiseScale = .1f;
    public static float treeThreshold = .5f;
    public static float treeDensity = .5f;
    public static float rockNoiseScale = .05f;
    public static float rockThreshold = .2f;

    public TerrainGenerator terrainGenerator;
    public GameObject treeObjectsGroup;
    public GameObject rockObjectsGroup;

    GameObject[] treeObjects;
    GameObject[] rockObjects;

    private void Awake() {
        
        treeObjects = treeObjectsGroup.GetComponentsInChildren<Tree>().ToList().ConvertAll(t => t.gameObject).ToArray();
        rockObjects = rockObjectsGroup.GetComponentsInChildren<Rock>().ToList().ConvertAll(t => t.gameObject).ToArray();
    }

    public void AddTrees(ref WorldData worldData, int seed) {
        List<TreeData> treeData = new List<TreeData>();

        System.Random rng = new System.Random(seed);
        int size = TerrainGenerator.size * (int)treesPerVoxel;
        float[,] landNoise = Procedural.NoiseMap(seed, TerrainGenerator.size, TerrainGenerator.noiseScale, terrainGenerator.multiplier);
        float[,] treeNoise = Procedural.NoiseMap(seed + 1, size, treeNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Coords voxelCoords = new Coords(Mathf.FloorToInt(x / treesPerVoxel), Mathf.FloorToInt(y / treesPerVoxel));
                bool adjacentOcean = Util.AdjacentProperty(Util.CoordsToVoxels(worldData.voxelData, Util.GetAdjacent(voxelCoords, true)), "isOcean");
                bool adjacentLake = Util.AdjacentProperty(Util.CoordsToVoxels(worldData.voxelData, Util.GetAdjacent(voxelCoords, true)), "isLake");
                if (worldData.voxelData[voxelCoords.x, voxelCoords.y].isLand && !worldData.voxelData[voxelCoords.x, voxelCoords.y].occupied && !adjacentOcean && (treeNoise[x, y] < treeThreshold || adjacentLake) && rng.Next(0, 1000) / 1000f < treeNoise[x, y] * landNoise[voxelCoords.x, voxelCoords.y] * treeDensity) {
                    int treeType = rng.Next(0, treeObjects.Length);
                    Coords treeCoords = new Coords(x, y);
                    float rotation = rng.Next(0, 360);
                    TreeData tree = new TreeData(treeCoords, voxelCoords, treeType, rotation);
                    treeData.Add(tree);
                    worldData.voxelData[voxelCoords.x, voxelCoords.y].occupied = true;
                    worldData.voxelData[voxelCoords.x, voxelCoords.y].hasTrees = true;
                }
            }
        }

        worldData.treeData = treeData;
    }

    public void AddRocks(ref WorldData worldData, int seed) {
        List<RockData> rockData = new List<RockData>();

        System.Random rng = new System.Random(seed);
        int size = TerrainGenerator.size;
        float[,] noiseMap = Procedural.NoiseMap(seed + 2, size, rockNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Coords coords = new Coords(x, y);
                bool adjacentOcean = Util.AdjacentProperty(Util.CoordsToVoxels(worldData.voxelData, Util.GetAdjacent(coords, true)), "isOcean");
                if (worldData.voxelData[x, y].isLand && !adjacentOcean && !worldData.voxelData[x, y].occupied && noiseMap[x, y] < rockThreshold) {
                    RockData rock = new RockData(coords, rng.Next(0, rockObjects.Length), rng.Next(0, 360));
                    rockData.Add(rock);
                    worldData.voxelData[coords.x, coords.y].occupied = true;
                    worldData.voxelData[coords.x, coords.y].navigable = false;
                }
            }
        }

        worldData.rockData = rockData;
    }

    public void GenerateTrees(WorldData world) {
        foreach(TreeData treeData in world.treeData) {
            Vector3 pos = new Vector3(treeData.treeCoords.x / treesPerVoxel - .5f / treesPerVoxel, 0, treeData.treeCoords.y / treesPerVoxel - .5f / treesPerVoxel);
            Tree tree = Instantiate(treeObjects[treeData.type], pos, Quaternion.Euler(0, treeData.rotation, 0), this.transform).GetComponent<Tree>();
            tree.Initialize(treeData);
        }
    }

    public void GenerateRocks(WorldData world) {
        foreach(RockData rockData in world.rockData) {
            Rock rock = Instantiate(rockObjects[rockData.type], Util.CoordsToVector3(rockData.coords), Quaternion.Euler(0, rockData.rotation, 0), this.transform).GetComponent<Rock>();
            rock.Initialize(rockData);
        }
    }
}
