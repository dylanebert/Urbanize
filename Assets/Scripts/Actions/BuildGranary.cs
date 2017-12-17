using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildGranary : ActionReward {

    public GameObject granaryObj;

    GameController gameController;

    private void Awake() {
        gameController = Util.FindGameController();
    }

    public override bool CanDoAction(Human human) {
        BuildingLocationData loc = human.FindUnoccupiedBuildingSpace();
        if (loc != null)
            return true;
        return false;
    }

    public bool CanDoAction(Human human, ref BuildingLocationData loc) {
        loc = human.FindUnoccupiedBuildingSpace();
        if (loc != null)
            return true;
        return false;
    }

    public override float GetReward(Human human) {
        if (gameController.data.granaryData.Count == 0 && CanDoAction(human))
            return 1f;
        return -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        human.busy = true;

        //Find building location
        BuildingLocationData loc = null;
        if (!CanDoAction(human, ref loc)) {
            human.busy = false;
            yield break;
        }

        //Create data
        GranaryData granaryData = CreateGranaryData(loc);

        //Move to storehouse build location
        yield return human.MoveTo(Util.CoordsToVector3(loc.coords));

        //Build storehouse
        CreateGranary(granaryData);

        human.busy = false;
    }

    public GranaryData CreateGranaryData(BuildingLocationData loc) {
        GranaryData granaryData = new GranaryData(loc, "Granary " + (gameController.data.granaryData.Count + 1));
        VoxelData voxelData = gameController.data.voxelData[loc.coords.x, loc.coords.y];
        gameController.data.granaryData.Add(granaryData);
        voxelData.hasGranary = true;
        voxelData.occupied = true;
        voxelData.navigable = false;
        loc.front.claimed = true;
        foreach (Coords c in Util.GetAdjacent(voxelData.coords, false))
            gameController.data.voxelData[c.x, c.y].UpdateClaimed(gameController.data.voxelData);
        return granaryData;
    }

    void CreateGranary(GranaryData granaryData) {
        Quaternion rotation = Quaternion.LookRotation(Util.CoordsToVector3(granaryData.buildingData.loc.front.coords) - Util.CoordsToVector3(granaryData.buildingData.loc.coords), Vector3.up);
        Granary storehouse = Instantiate(granaryObj, Util.CoordsToVector3(granaryData.buildingData.loc.coords), rotation, this.transform).GetComponent<Granary>();
        storehouse.Initialize(granaryData);
    }
}
