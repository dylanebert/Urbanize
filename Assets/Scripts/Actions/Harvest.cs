using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest : ActionReward {

    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public override float GetReward(Human human) {
        return -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        human.busy = true;
        if (gameController.worldData.storehouseData.Count > 0) {
            foreach (FarmData farmData in gameController.worldData.farmData) {
                if (farmData.yield == Farm.MaxYield) {
                    farmData.yield = 0;
                    gameController.worldData.storehouseData[0].food += (int)Farm.MaxYield;
                    break;
                }
            }
        }
        yield return new WaitForSeconds(5f);
        human.busy = false;
    }
}
