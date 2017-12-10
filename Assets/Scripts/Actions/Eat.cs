using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : ActionReward {

    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public override float GetReward(Human human) {
        bool canEat = false;
        foreach (StorehouseData storehouseData in gameController.worldData.storehouseData) {
            if (storehouseData.food > 0) {
                canEat = true;
                break;
            }
        }
        return canEat ? human.hunger : -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        human.busy = true;
        foreach(StorehouseData storehouseData in gameController.worldData.storehouseData) {
            if(storehouseData.food > 0) {
                storehouseData.food--;
                human.hunger = 0;
                break;
            }
        }
        yield return new WaitForSeconds(5f);
        human.busy = false;
    }
}
