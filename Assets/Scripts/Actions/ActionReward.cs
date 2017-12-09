using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionReward : MonoBehaviour {

    public abstract float GetReward(HumanData state, WorldData world);
    public abstract IEnumerator PerformAction(Human human);
}
