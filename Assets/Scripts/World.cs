using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {

    public float[,] noiseMap;
    bool[,][] grid;
    public List<TreeData> treeData = new List<TreeData>();
    public List<StorehouseData> storehouseData = new List<StorehouseData>();

    public World(int size) {
        grid = new bool[size, size][];
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                grid[x, y] = new bool[Util.WorldStateEncodingLength];
            }
        }
    }

    public void SetProperty(int x, int y, string property, bool value) {
        grid[x, y][Util.WorldStateIndices[property]] = value;
    }

    public bool GetProperty(int x, int y, string property) {
        return grid[x, y][Util.WorldStateIndices[property]];
    }

    public List<Vector2> GetAdjacent(int x, int y, bool corners) {
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

    public bool PropertyAdjacentProperty(int x, int y, int a, int b, bool corners) {
        List<Vector2> adjacent = GetAdjacent(x, y, corners);
        if (grid[x, y][a]) {
            foreach (Vector2 adj in adjacent) {
                if (grid[(int)adj.x, (int)adj.y][b])
                    return true;
            }
        }
        return false;
    }

    public bool PropertyAdjacentProperty(int x, int y, string propertyA, string propertyB, bool corners) {
        int a = Util.WorldStateIndices[propertyA];
        int b = Util.WorldStateIndices[propertyB];
        List<Vector2> adjacent = GetAdjacent(x, y, corners);
        if (grid[x, y][a]) {
            foreach (Vector2 adj in adjacent) {
                if (grid[(int)adj.x, (int)adj.y][b])
                    return true;
            }
        }
        return false;
    }
}
