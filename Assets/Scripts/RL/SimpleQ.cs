using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleQ : MonoBehaviour {

    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
}
