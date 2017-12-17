using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Resource {

    public override IEnumerator Deposit(Human human, object target) {
        Granary granary = (Granary)target;

        yield return human.MoveTo(granary.front.position);

        Destroy(this.gameObject);
    }
}
