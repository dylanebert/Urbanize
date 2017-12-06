using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChopTrees : ActionReward {
    public override float GetReward(HumanState state) {
        float reward = 0f;

        return reward;
    }

    public override IEnumerator PerformAction(Human human, float duration) {
        float t = 0f;
        while (t < duration) {
            float time = Time.time;
            Vector3 startPos = human.transform.position;
            Tree tree = human.GetNextTree();
            if (tree == null) yield break;
            Vector3 target = tree.transform.position;
            human.transform.LookAt(target);
            yield return human.MoveTo(target);
            yield return StartCoroutine(tree.Chop(human, 5f));
            t += Time.time - time;
            time = Time.time;
            if (human.state.storehouse == null) {
                if (!human.FindStorehouse()) {
                    continue;
                }
            }
            while(human.GetNextWood() && t < duration) { 
                yield return StartCoroutine(human.state.targetResource.PickUp(human));
                yield return StartCoroutine(human.state.holding.Deposit(human));
                t += Time.time - time;
                time = Time.time;
            }
        }
    }
}
