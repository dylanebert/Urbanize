using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storehouse : Building {

    [HideInInspector]
    public Inventory inventory = new Inventory();

    private IEnumerator Start() {
        while (!initialized)
            yield return null;
        gameController.AddStorehouse(this);
    }
}
