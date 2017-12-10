using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFarm : ActionReward {

    public GameObject farmObj;

    public override float GetReward(Human human) {
        return -5f;
    }

    public override IEnumerator PerformAction(Human human) {
        Farm farm = Instantiate(farmObj).GetComponent<Farm>();
        farm.Initialize();
        human.busy = true;
        yield return new WaitForSeconds(5f);
        human.busy = false;
    }
}
