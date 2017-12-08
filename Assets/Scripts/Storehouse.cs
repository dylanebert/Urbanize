using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storehouse : Building {

    public StorehouseData storehouseData;

    protected override void Awake() {
        base.Awake();
        storehouseData = new StorehouseData(Util.GroundVector2(transform.position), (int)transform.rotation.eulerAngles.y / 90);
        gameController.world.storehouseData.Add(storehouseData);
        gameController.storehouseDict.Add(storehouseData, this);
        this.gameObject.name = "Storehouse " + gameController.world.storehouseData.Count;
    }
}

[System.Serializable]
public class StorehouseData {
    public BuildingData buildingData;
    public int wood;

    public StorehouseData(Vector2 coords, int rotation) {
        this.buildingData = new BuildingData(coords, rotation);
    }
}