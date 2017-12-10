using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour {

    public GameObject dustParticleObj;
    public DayNightCycle dayNightCycle;
    public GameObject woodObj;
    public Camera uiCam;
    public Pointer pointer;
    public TerrainGenerator terrainGenerator;
    public ObjectGenerator objectGenerator;
    public WavesGenerator wavesGenerator;
    public TinyverseAcademy academy;
    public int seed = 0;

    [HideInInspector]
    public WorldData worldData;
    [HideInInspector]
    public List<Human> humans;
    [HideInInspector]
    public List<Wood> wood;
    [HideInInspector]
    public Dictionary<VoxelData, Voxel> voxelDict = new Dictionary<VoxelData, Voxel>();
    [HideInInspector]
    public Dictionary<TreeData, Tree> treeDict = new Dictionary<TreeData, Tree>();
    [HideInInspector]
    public Dictionary<StorehouseData, Storehouse> storehouseDict = new Dictionary<StorehouseData, Storehouse>();
    [HideInInspector]
    public Dictionary<FarmData, Farm> farmDict = new Dictionary<FarmData, Farm>();

    private void Awake() {
        worldData = terrainGenerator.GenerateTerrain(seed);
        objectGenerator.GenerateTrees(worldData, seed);
        objectGenerator.GenerateRocks(worldData, seed);
        StartCoroutine(wavesGenerator.GenerateWaves(worldData));
    }

    private void Start() {
        Time.timeScale = 10f;
    }

    public void AddHuman(Human human) {
        humans.Add(human);
    }
}