using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest : ActionReward {

    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public override bool CanDoAction(Human human) {
        Farm farm = human.FindFarm(true);
        if (farm != null)
            return true;
        return false;        
    }

    public bool CanDoAction(Human human, ref Farm farm) {
        farm = human.FindFarm(true);
        if (farm != null)
            return true;
        return false;
    }

    public override float GetReward(Human human) {
        if(CanDoAction(human))
            return .1f;
        return -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        human.busy = true;

        //Find farm
        Farm farm = null;
        if(!CanDoAction(human, ref farm)) { 
            human.busy = false;
            yield break;
        }

        //Update data
        farm.data.pendingRows--;
        human.data.lastHarvested = farm.data;

        //Harvest farm
        yield return farm.Harvest(human, farm.data.pendingRows);

        //Update farm data
        if (--farm.data.rows == 0)
            farm.ResetFarm();

        human.busy = false;
    }
}
