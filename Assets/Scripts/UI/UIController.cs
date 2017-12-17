using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    Ploppable currentPloppable;

    public void CreatePloppable(GameObject ploppable) {
        if (currentPloppable != null)
            currentPloppable.Cancel();
        currentPloppable = Instantiate(ploppable).GetComponent<Ploppable>();
        currentPloppable.enabled = true;
    }

    public void ResetPloppable() {
        currentPloppable = null;
    }
}
