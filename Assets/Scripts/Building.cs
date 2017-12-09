using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IWorldSelectable {

    public Transform front;
    public GameObject buildingInfoWindowObj;

    protected GameController gameController;

    WorldWindow buildingInfoWindow;
    bool selected;

    protected virtual void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public virtual void Initialize() {
        gameController.worldData.voxels[Util.GroundVector2(transform.position)].occupied = true;
        gameController.worldData.voxels[Util.GroundVector2(transform.position)].navigable = false;
        gameController.worldData.voxels[Util.GroundVector2(transform.position) + new Vector2(transform.forward.x, transform.forward.z)].claimed = true;
    }

    public virtual void Select() {
        if (selected)
            return;
        gameController.pointer.SetSelectIndicatorPosition(Util.GroundVector2(transform.position), Vector2.one);
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
        gameController.pointer.SetCursorIndicatorPosition(Util.GroundVector2(transform.position), Vector2.one);
    }

    public virtual void Dehover() {
        gameController.pointer.ShowCursorIndicator(true);
    }
}

[System.Serializable]
public class BuildingData {
    public int rotation;
    public Vector2 coords;

    public BuildingData(Vector2 coords, int rotation) {
        this.coords = coords;
        this.rotation = rotation;
    }
}