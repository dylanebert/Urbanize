using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : ActionReward {

    GameController gameController;

    private void Awake() {
        gameController = Util.FindGameController();
    }

    public override bool CanDoAction(Human human) {
        Granary granary = human.FindGranary(true);
        if (granary != null)
            return true;
        return false;
    }

    public bool CanDoAction(Human human, ref Granary granary) {
        granary = human.FindGranary(true);
        if (granary != null)
            return true;
        return false;
    }

    public override float GetReward(Human human) {
        if (human.hunger >= 1f && CanDoAction(human)) {
            return 1f;
        }
        return -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        human.busy = true;

        //Find storehouse
        Granary granary = null;
        if (!CanDoAction(human, ref granary)) {
            human.busy = false;
            yield break;
        }

        //Update food data
        granary.data.pendingFood--;

        //Go to storehouse
        yield return human.MoveTo(granary.front.position);

        //Eat
        while (granary.data.food == 0)
            yield return null;

        granary.data.food--;
        human.hunger = Mathf.Max(human.hunger - 1, 0);

        human.busy = false;
    }
}
