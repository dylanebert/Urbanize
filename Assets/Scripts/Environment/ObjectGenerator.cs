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
    public static float rockThreshold = .15f;

    public TerrainGenerator terrainGenerator;
    public GameObject treeObjectsGroup;
    public GameObject rockObjectsGroup;

    GameObject[] treeObjects;
    GameObject[] rockObjects;

    private void Awake() {
        
        treeObjects = treeObjectsGroup.GetComponentsInChildren<Tree>().ToList().ConvertAll(t => t.gameObject).ToArray();
        rockObjects = rockObjectsGroup.GetComponentsInChildren<Rock>().ToList().ConvertAll(t => t.gameObject).ToArray();
    }

    public void GenerateTrees(WorldData world, int seed) {
        System.Random rng = new System.Random(seed);
        int size = TerrainGenerator.size * (int)treesPerVoxel;
        float[,] landNoise = Procedural.NoiseMap(seed, size, TerrainGenerator.noiseScale, terrainGenerator.multiplier);
        float[,] treeNoise = Procedural.NoiseMap(seed + 1, size, treeNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Vector2 coords = new Vector2(Mathf.FloorToInt(x / treesPerVoxel), Mathf.FloorToInt(y / treesPerVoxel));
                if(world.voxels[coords].isLand && !world.AdjacentProperty(coords, "isOcean", true) && (treeNoise[x, y] < treeThreshold || world.AdjacentProperty(coords, "isLake", true)) && rng.Next(0, 1000) / 1000f < treeNoise[x, y] * landNoise[(int)coords.x, (int)coords.y] * treeDensity) {
                    int type = rng.Next(0, treeObjects.Length);
                    Tree tree = Instantiate(treeObjects[type], new Vector3(x / treesPerVoxel - .5f / treesPerVoxel, 0, y / treesPerVoxel - .5f / treesPerVoxel), Quaternion.Euler(0, rng.Next(0, 360), 0), this.transform).GetComponent<Tree>();
                    tree.Initialize(type);
                }
            }
        }
    }

    public void GenerateRocks(WorldData world, int seed) {
        System.Random rng = new System.Random(seed);
        int size = TerrainGenerator.size;
        float[,] noiseMap = Procedural.NoiseMap(seed + 2, size, rockNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Vector2 coords = new Vector2(x, y);
                if (world.voxels[coords].isLand && !world.AdjacentProperty(coords, "isOcean", true) && !world.voxels[coords].occupied && noiseMap[x, y] < rockThreshold) {
                    Instantiate(rockObjects[rng.Next(0, rockObjects.Length)], new Vector3(x, 0, y), Quaternion.identity, this.transform);
                }
            }
        }
    }
}
