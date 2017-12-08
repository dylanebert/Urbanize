using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStorehouse : ActionReward {

    public GameObject storehouseObj;

    public override float GetReward(HumanState state, World world) {
        float reward = 0f;

        return reward;
    }

    public override IEnumerator PerformAction(Human human, float duration) {


        yield return null;
    }
}
