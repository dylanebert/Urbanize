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

    List<Voxel> occupied = new List<Voxel>();
    GameController gameController;
    GameObject[] treeObjects;
    GameObject[] rockObjects;
    System.Random rng;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        rng = new System.Random(terrainGenerator.seed);
        treeObjects = treeObjectsGroup.GetComponentsInChildren<Tree>().ToList().ConvertAll(t => t.gameObject).ToArray();
        rockObjects = rockObjectsGroup.GetComponentsInChildren<Rock>().ToList().ConvertAll(t => t.gameObject).ToArray();
    }

    private IEnumerator Start() {
        while (!terrainGenerator.finished)
            yield return null;

        GenerateTrees();
        GenerateRocks();

        gameController.ready = true;
    }

    void GenerateTrees() {
        int size = TerrainGenerator.size * (int)treesPerVoxel;
        float[,] landNoise = terrainGenerator.noiseMap;
        float[,] treeNoise = Procedural.NoiseMap(terrainGenerator.seed + 1, size, treeNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Vector2 coords = new Vector2(Mathf.FloorToInt(x / treesPerVoxel), Mathf.FloorToInt(y / treesPerVoxel));
                Voxel voxel = gameController.grid[coords];
                if(voxel.type == 1 && !voxel.IsTypeNeighboringType(1, 0) && (treeNoise[x, y] < treeThreshold || voxel.IsTypeNeighboringType(1, 2)) && rng.Next(0, 1000) / 1000f < treeNoise[x, y] * landNoise[(int)(x / treesPerVoxel), (int)(y / treesPerVoxel)] * treeDensity) {
                    Tree tree = Instantiate(treeObjects[rng.Next(0, treeObjects.Length)], new Vector3((x + .5f) / treesPerVoxel, 0, (y + .5f) / treesPerVoxel), Quaternion.Euler(0, rng.Next(0, 360), 0), this.transform).GetComponent<Tree>();
                    gameController.trees.Add(tree);
                    voxel.AddTree(tree);
                    if(!occupied.Contains(voxel)) {
                        occupied.Add(voxel);
                    }
                }
            }
        }
    }

    void GenerateRocks() {
        int size = TerrainGenerator.size;
        float[,] noiseMap = Procedural.NoiseMap(terrainGenerator.seed + 2, size, rockNoiseScale, AnimationCurve.Linear(0f, 0f, 1f, 1f));

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Vector2 coords = new Vector2(x, y);
                Voxel voxel = gameController.grid[coords];
                if (voxel.type == 1 && !voxel.IsTypeNeighboringType(1, 0) && !occupied.Contains(voxel) &&  noiseMap[x, y] < rockThreshold) {
                    Rock rock = Instantiate(rockObjects[rng.Next(0, rockObjects.Length)], new Vector3(x + .5f, 0, y + .5f), Quaternion.identity, this.transform).GetComponent<Rock>();
                    gameController.rocks.Add(rock);
                    voxel.occupied = true;
                    voxel.navigable = false;
                }
            }
        }
    }
}
