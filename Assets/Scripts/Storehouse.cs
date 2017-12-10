using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storehouse : Building {

    public StorehouseData storehouseData;

    public override void Initialize() {
        base.Initialize();
        storehouseData = new StorehouseData(Util.GroundVector2(transform.position), (int)transform.rotation.eulerAngles.y / 90);
        gameController.worldData.storehouseData.Add(storehouseData);
        gameController.storehouseDict.Add(storehouseData, this);
        gameController.worldData.voxels[Util.GroundVector2(transform.position)].hasStorehouse = true;
        this.gameObject.name = "Storehouse " + gameController.worldData.storehouseData.Count;
    }
}

[System.Serializable]
public class StorehouseData {
    public BuildingData buildingData;
    public int wood;
    public int food;

    public StorehouseData(Vector2 coords, int rotation) {
        this.buildingData = new BuildingData(coords, rotation);
    }
}