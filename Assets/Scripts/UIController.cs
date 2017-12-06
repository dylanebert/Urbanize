using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    Ploppable currentPloppable;

    public void SetPloppable(Ploppable ploppable) {
        if (currentPloppable != null)
            currentPloppable.Cancel();
        currentPloppable = ploppable;
    }

    public void ResetPloppable() {
        currentPloppable = null;
    }
}
