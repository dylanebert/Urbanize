using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildFarm : ActionReward {

    public GameObject farmObj;

    GameController gameController;

    private void Awake() {
        gameController = Util.FindGameController();
    }

    public override bool CanDoAction(Human human) {
        Coords coords = human.FindUnoccupied();
        if (!coords.Equals(Coords.Invalid))
            return true;
        return false;
    }

    public bool CanDoAction(Human human, ref Coords coords) {
        coords = human.FindUnoccupied();
        if (!coords.Equals(Coords.Invalid))
            return true;
        return false;
    }

    public override float GetReward(Human human) {
        if (gameController.data.farmData.Count(x => x.pendingYield == 1f || x.isNew) == 0 && gameController.data.granaryData.Count(x => x.pendingFood > 0) == 0 && CanDoAction(human))
            return .1f;
        return -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        human.busy = true;

        //Find unoccupied space
        Coords coords = null;
        if (!CanDoAction(human, ref coords)) {
            human.busy = false;
            yield break;
        }

        //Create farm data
        FarmData farmData = CreateFarmData(coords);

        //Move to build site
        yield return human.MoveTo(Util.CoordsToVector3(coords));

        //Build farm
        CreateFarm(farmData);

        human.busy = false;
    }

    public FarmData CreateFarmData(Coords coords) {
        FarmData farmData = new FarmData(coords, "Farm " + (gameController.data.farmData.Count + 1));
        gameController.data.farmData.Add(farmData);
        VoxelData voxelData = gameController.data.voxelData[coords.x, coords.y];
        voxelData.occupied = true;
        voxelData.hasFarm = true;
        return farmData;
    }

    public void CreateFarm(FarmData farmData) {
        Farm farm = Instantiate(farmObj, Util.CoordsToVector3(farmData.coords), Quaternion.identity, this.transform).GetComponent<Farm>();
        farm.Initialize(farmData);
    }
}
