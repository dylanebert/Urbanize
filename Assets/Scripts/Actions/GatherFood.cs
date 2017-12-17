using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherFood : ActionReward {

    public override bool CanDoAction(Human human) {
        Granary granary = human.FindGranary(false);
        Food food = human.FindFood();
        if (granary != null && food != null)
            return true;
        return false;
    }

    public bool CanDoAction(Human human, ref Granary granary, ref Food food) {
        granary = human.FindGranary(false);
        food = human.FindFood();
        if (granary != null && food != null)
            return true;
        return false;
    }

    public override float GetReward(Human human) {
        if(CanDoAction(human))
            return .5f;
        return -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        human.busy = true;

        Granary granary = null;
        Food food = null;
        if(!CanDoAction(human, ref granary, ref food)) {
            human.busy = false;
            yield break;
        }

        //Update data
        granary.data.pendingFood++;

        //Pick up food
        yield return food.PickUp(human);

        //Deposit in granary
        yield return food.Deposit(human, granary);

        granary.data.food++;

        human.busy = false;
    }
}
