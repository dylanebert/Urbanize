using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IWorldSelectable {

    public Transform front;
    public GameObject buildingInfoWindowObj;

    protected GameController gameController;
    protected bool initialized;

    Voxel voxel;
    WorldWindow buildingInfoWindow;
    bool selected;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public virtual void Initialize(Voxel voxel) {
        initialized = true;
        voxel.occupied = true;
        voxel.navigable = false;
        this.voxel = voxel;
        gameController.grid[voxel.coords + new Vector2(transform.forward.x, transform.forward.z)].buildingFront = true;
    }

    public virtual void Select() {
        if (selected)
            return;
        gameController.pointer.SetSelectIndicatorPosition(voxel.coords, Vector2.one);
        buildingInfoWindow = Instantiate(buildingInfoWindowObj, Util.GroundVector3(transform.position), Quaternion.identity, this.transform).GetComponent<WorldWindow>();
        buildingInfoWindow.Initialize(this);
        selected = true;
    }

    public virtual void Deselect() {
        if (!selected)
            return;
        StartCoroutine(buildingInfoWindow.CloseCoroutine());
        gameController.pointer.ShowSelectIndicator(false);
        selected = false;
    }

    public virtual void Hover() {
        gameController.pointer.SetCursorIndicatorPosition(voxel.coords, Vector2.one);
    }

    public virtual void Dehover() {
        gameController.pointer.ShowCursorIndicator(true);
    }
}
