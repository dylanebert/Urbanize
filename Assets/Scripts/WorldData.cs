using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData {

    public Dictionary<Vector2, VoxelData> voxels = new Dictionary<Vector2, VoxelData>();
    public List<TreeData> treeData = new List<TreeData>();
    public List<StorehouseData> storehouseData = new List<StorehouseData>();

    public List<Vector2> GetAdjacent(Vector2 coords, bool corners) {
        int x = (int)coords.x;
        int y = (int)coords.y;
        List<Vector2> adjacent = new List<Vector2>();
        if (x > 0) {
            adjacent.Add(new Vector2(x - 1, y));
            if (corners && y > 0) {
                adjacent.Add(new Vector2(x - 1, y - 1));
            }
            if (corners && y < TerrainGenerator.size - 1) {
                adjacent.Add(new Vector2(x - 1, y + 1));
            }
        }
        if (x < TerrainGenerator.size - 1) {
            adjacent.Add(new Vector2(x + 1, y));
            if (corners && y > 0) {
                adjacent.Add(new Vector2(x + 1, y - 1));
            }
            if (corners && y < TerrainGenerator.size - 1) {
                adjacent.Add(new Vector2(x + 1, y + 1));
            }
        }
        if (y > 0) {
            adjacent.Add(new Vector2(x, y - 1));
        }
        if (y < TerrainGenerator.size - 1) {
            adjacent.Add(new Vector2(x, y + 1));
        }
        return adjacent;
    }

    public bool AdjacentProperty(Vector2 coords, string property, bool corners) {
        foreach(Vector2 v in GetAdjacent(coords, corners)) {
            if ((bool)typeof(VoxelData).GetField(property).GetValue(voxels[v]))
                return true;
        }
        return false;
    }
}
