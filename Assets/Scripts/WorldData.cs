using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData {

    public VoxelData[,] voxelData = new VoxelData[TerrainGenerator.size, TerrainGenerator.size];
    public List<TreeData> treeData = new List<TreeData>();
    public List<RockData> rockData = new List<RockData>();
    public List<GranaryData> granaryData = new List<GranaryData>();
    public List<FarmData> farmData = new List<FarmData>();
}
