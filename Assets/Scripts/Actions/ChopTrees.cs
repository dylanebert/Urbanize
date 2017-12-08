using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChopTrees : ActionReward {
    public override float GetReward(HumanState state, World world) {
        return 0;
    }

    public override IEnumerator PerformAction(Human human) {
        yield return ChopTree(human);
        yield return GatherWood(human);
    }

    IEnumerator ChopTree(Human human) {
        Tree tree = human.FindNearestTree();
        if (tree == null) yield break;
        Vector3 target = tree.transform.position;
        yield return human.MoveTo(target);
        yield return tree.Chop(human);
    }

    IEnumerator GatherWood(Human human) {
        if (human.FindNearestStorehouse() == null)
            yield break;
        if(human.state.targetResource != null) { 
            yield return StartCoroutine(human.state.targetResource.PickUp(human));
            yield return StartCoroutine(human.state.holding.Deposit(human));
        }
    }
}
