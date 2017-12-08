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

    [HideInInspector]
    public World world;
    [HideInInspector]
    public List<Human> humans;
    [HideInInspector]
    public List<Wood> wood;
    [HideInInspector]
    public Dictionary<TreeData, Tree> treeDict = new Dictionary<TreeData, Tree>();
    [HideInInspector]
    public Dictionary<StorehouseData, Storehouse> storehouseDict = new Dictionary<StorehouseData, Storehouse>();

    private void Awake() {
        world = terrainGenerator.GenerateWorldState();
        terrainGenerator.GenerateTerrain(world);
        objectGenerator.GenerateTrees(world);
        objectGenerator.GenerateRocks(world);
        StartCoroutine(wavesGenerator.GenerateWaves(world));
    }

    private void Start() {
        Time.timeScale = 1f;
    }

    public void AddHuman(Human human) {
        humans.Add(human);
    }
}