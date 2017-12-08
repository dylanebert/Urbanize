using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionReward : MonoBehaviour {

    public abstract float GetReward(HumanState state, World world);
    public abstract IEnumerator PerformAction(Human human, float duration);
}
