﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStorehouse : ActionReward {

    public GameObject storehouseObj;

    public override float GetReward(Human human) {
        return -5f;
    }

    public override IEnumerator PerformAction(Human human) {
        Storehouse storehouse = Instantiate(storehouseObj).GetComponent<Storehouse>();
        storehouse.Initialize();
        human.busy = true;
        yield return new WaitForSeconds(5f);
        human.busy = false;
    }
}
