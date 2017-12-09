using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStorehouse : ActionReward {

    public GameObject storehouseObj;

    public override float GetReward(HumanData state, WorldData world) {
        float reward = 0f;

        return reward;
    }

    public override IEnumerator PerformAction(Human human) {


        yield return null;
    }
}
