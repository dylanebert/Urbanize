using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChopTrees : ActionReward {
    public override float GetReward(HumanState state, World world) {
        float reward = 0f;

        return reward;
    }

    public override IEnumerator PerformAction(Human human, float duration) {
        float t = 0f;
        while (t < duration) {
            float time = Time.time;
            yield return GatherWood(human, duration - t);
            yield return ChopTree(human);
            t += Time.time - time;
            time = Time.time;
        }
    }

    IEnumerator ChopTree(Human human) {
        Tree tree = human.GetNextTree();
        if (tree == null) yield break;
        Vector3 target = tree.transform.position;
        yield return human.MoveTo(target);
        yield return tree.Chop(human);
    }

    IEnumerator GatherWood(Human human, float duration) {
        if (human.FindNearestStorehouse() == null)
            yield break;
        float t = 0f;
        float time = Time.time;
        while (t < duration && human.GetNextWood()) {
            yield return StartCoroutine(human.state.targetResource.PickUp(human));
            yield return StartCoroutine(human.state.holding.Deposit(human));
            t += Time.time - time;
            time = Time.time;
        }
    }
}
