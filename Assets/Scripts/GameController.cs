using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour {

    public enum Mode {
        Basic,
        Reinforce
    }

    public Mode mode;
    public Transform actions;
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
    public WorldData data;
    [HideInInspector]
    public List<Human> humans;
    [HideInInspector]
    public Dictionary<VoxelData, Voxel> voxelDict = new Dictionary<VoxelData, Voxel>();
    [HideInInspector]
    public Dictionary<TreeData, Tree> treeDict = new Dictionary<TreeData, Tree>();
    [HideInInspector]
    public Dictionary<RockData, Rock> rockDict = new Dictionary<RockData, Rock>();
    [HideInInspector]
    public Dictionary<GranaryData, Granary> granaryDict = new Dictionary<GranaryData, Granary>();
    [HideInInspector]
    public Dictionary<FarmData, Farm> farmDict = new Dictionary<FarmData, Farm>();
    [HideInInspector]
    public List<Resource> looseResources = new List<Resource>();

    WorldData baseWorldData;

    private void Awake() {
        academy.gameObject.SetActive(mode == Mode.Reinforce);

        VoxelData[,] baseVoxelData = terrainGenerator.GenerateVoxelData(seed);
        baseWorldData = new WorldData() { 
            voxelData = baseVoxelData
        };
        objectGenerator.AddRocks(ref baseWorldData, seed);
        objectGenerator.AddTrees(ref baseWorldData, seed);   
        StartCoroutine(wavesGenerator.GenerateWaves(baseWorldData));

        terrainGenerator.GenerateTerrain(baseWorldData);
        objectGenerator.GenerateRocks(baseWorldData);
        objectGenerator.GenerateTrees(baseWorldData);

        if (mode == Mode.Basic) {
            ResetWorld();
        }
    }

    private void Update() {
        if (mode == Mode.Basic)
            Step();

        if (Input.GetKeyDown(KeyCode.S)) {
            if (Time.timeScale == 1f)
                Time.timeScale = 10f;
            else
                Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.R))
            ResetWorld();
    }

    public void Step() {
        float deltaTime = Time.deltaTime;
        if (mode == Mode.Reinforce && !academy.isInference)
            deltaTime = 30f;
        foreach (Farm farm in farmDict.Values)
            farm.Step(deltaTime);
    }

    public void ResetWorld() {
        pointer.Deselect();

        /*foreach (Tree tree in treeDict.Values)
            Destroy(tree.gameObject);
        foreach (Rock rock in rockDict.Values)
            Destroy(rock.gameObject);*/
        foreach (Granary storehouse in granaryDict.Values)
            Destroy(storehouse.gameObject);
        foreach (Farm farm in farmDict.Values)
            Destroy(farm.gameObject);

        /*treeDict.Clear();
        rockDict.Clear();*/
        granaryDict.Clear();
        farmDict.Clear();

        data = Util.Copy(baseWorldData);
        /*objectGenerator.GenerateTrees(data);
        objectGenerator.GenerateRocks(data);*/

        UpdateDictionaries();
    }

    void UpdateDictionaries() {
        /*Stack<Voxel> voxels = new Stack<Voxel>(voxelDict.Values);
        while(voxels.Count > 0)
            voxels.Pop().UpdateDict();
        Stack<Tree> trees = new Stack<Tree>(treeDict.Values);
        while (trees.Count > 0)
            trees.Pop().UpdateDict();
        Stack<Rock> rocks = new Stack<Rock>(rockDict.Values);
        while (rocks.Count > 0)
            rocks.Pop().UpdateDict();*/
        Stack<Granary> granaries = new Stack<Granary>(granaryDict.Values);
        while (granaries.Count > 0)
            granaries.Pop().UpdateDict();
        Stack<Farm> farms = new Stack<Farm>(farmDict.Values);
        while (farms.Count > 0)
            farms.Pop().UpdateDict();
    }

    public void AddHuman(Human human) {
        humans.Add(human);
    }
}