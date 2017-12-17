using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChopTrees : ActionReward {

    GameController gameController;

    private void Awake() {
        gameController = Util.FindGameController();
    }

    public override bool CanDoAction(Human human) {
        return false;
    }

    public override float GetReward(Human human) {
        return -1f;
    }

    public override IEnumerator PerformAction(Human human) {
        /*human.busy = true;

        //Find storehouse
        if (human.FindNearestStorehouse() == null)
            yield break;

        //Find tree
        Tree tree = human.FindNearestTree();
        if (tree == null)
            yield break;

        //Move to tree
        Vector3 target = tree.transform.position;
        yield return human.MoveTo(target);

        //Chop tree
        yield return ChopTree(human, tree);

        //Pick up dropped wood
        yield return StartCoroutine(human.targetResource.PickUp(human));

        //Deposit to storehouse
        yield return StartCoroutine(human.holding.Deposit(human));

        human.busy = false;*/
        yield return null;
    }

    public IEnumerator ChopTree(Human human, Tree tree) {
        float duration = Human.ChopSpeed;
        ParticleSystem dust = Instantiate(gameController.dustParticleObj, tree.transform).GetComponentInChildren<ParticleSystem>();
        float t = 0f;
        float t2 = 0f;
        while (t < duration - 1.5f) {
            t += Time.deltaTime;
            t2 += Time.deltaTime;
            if (t2 > Tree.ShakeInterval) {
                t2 = 0f;
                transform.rotation = Quaternion.Euler(Random.Range(-2f, 2f), transform.rotation.eulerAngles.y, Random.Range(-2f, 2f));
            }
            yield return null;
        }
        dust.Stop();

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(85f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        t = 0f;
        while (t < 1f) {
            t += Time.deltaTime;
            float v = Mathf.Pow(t, 3);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, v);
            yield return null;
        }

        while (t < 1.5f) {
            t += Time.deltaTime;
            yield return null;
        }

        Wood wood = Instantiate(gameController.woodObj, transform.position + Vector3.up * .1f, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).GetComponent<Wood>();

        tree.Destroy();
    }
}
