using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChopTrees : ActionReward {
    public override float GetReward(HumanData state, WorldData world) {
        return 0;
    }

    public override IEnumerator PerformAction(Human human) {
        if (human.FindNearestStorehouse() == null)
            yield break;
        Tree tree = human.FindNearestTree();
        if (tree == null) yield break;
        Vector3 target = tree.transform.position;
        yield return human.MoveTo(target);
        yield return tree.Chop(human);
        if (human.data.targetResource != null) {
            yield return StartCoroutine(human.data.targetResource.PickUp(human));
            yield return StartCoroutine(human.data.holding.Deposit(human));
        }
    }
}
