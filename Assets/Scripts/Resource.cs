using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour {

    public abstract IEnumerator PickUp(Human human);
    public abstract IEnumerator Deposit(Human human);
}
