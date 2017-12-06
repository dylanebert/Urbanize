using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public Transform front;

    protected GameController gameController;
    protected bool initialized;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public virtual void Initialize(Voxel voxel) {
        initialized = true;
        voxel.occupied = true;
        voxel.navigable = false;
        gameController.grid[voxel.coords + new Vector2(transform.forward.x, transform.forward.z)].buildingFront = true;
    }
}
