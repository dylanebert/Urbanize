using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granary : Building {

    public GranaryData data;

    public override void Initialize(object granaryData) {
        if(granaryData == null) {
            BuildingLocationData loc = new BuildingLocationData(Util.Vector3ToCoords(transform.position), new Coords(1, 1), gameController.data.voxelData[(int)transform.position.x + (int)transform.forward.x, (int)transform.position.z + (int)transform.forward.z]);
            this.data = new GranaryData(loc, "Storehouse " + (gameController.data.granaryData.Count + 1));
            VoxelData voxelData = gameController.data.voxelData[loc.coords.x, loc.coords.y];
            gameController.data.granaryData.Add(this.data);
            voxelData.navigable = false;
            voxelData.occupied = true;
            voxelData.hasGranary = true;
        } else
            this.data = (GranaryData)granaryData;

        this.gameObject.name = data.name;
        UpdateDict();
    }

    public void UpdateDict() {
        if (!gameController.granaryDict.ContainsKey(data))
            gameController.granaryDict.Add(data, this);
        else
            gameController.granaryDict[data] = this;
    }
}

[System.Serializable]
public class GranaryData {
    public string name;
    public BuildingData buildingData;
    public int wood;
    public int pendingWood;
    public int food;
    public int pendingFood;

    public GranaryData(BuildingLocationData loc, string name) {
        this.buildingData = new BuildingData(loc);
        this.name = name;
    }
}