using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public Transform front;

    protected GameController gameController;
    protected TerrainGenerator terrainGenerator;
    protected bool initialized;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        terrainGenerator = GameObject.FindGameObjectWithTag("GameController").GetComponent<TerrainGenerator>();
    }

    public virtual void Initialize(Voxel voxel) {
        initialized = true;
        voxel.occupied = true;
        voxel.navigable = false;
        terrainGenerator.grid[voxel.coords + new Vector2(transform.forward.x, transform.forward.z)].buildingFront = true;
    }
}
