using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionReward : MonoBehaviour {

    public abstract bool CanDoAction(Human human);
    public abstract float GetReward(Human human);
    public abstract IEnumerator PerformAction(Human human);
}
