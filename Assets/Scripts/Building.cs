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

    public virtual void Initialize(object buildingData) {

    }

    public virtual void Select() {
        if (selected)
            return;
        gameController.pointer.SetSelectIndicatorPosition(Util.Vector3ToCoords(transform.position), new Coords(1, 1));
        buildingInfoWindow = Instantiate(buildingInfoWindowObj, transform.position, Quaternion.identity, this.transform).GetComponent<WorldWindow>();
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
        gameController.pointer.SetCursorIndicatorPosition(Util.Vector3ToCoords(transform.position), new Coords(1, 1));
    }

    public virtual void Dehover() {
        gameController.pointer.ShowCursorIndicator(true);
    }
}

[System.Serializable]
public class BuildingData {
    public BuildingLocationData loc;

    public BuildingData(BuildingLocationData loc) {
        this.loc = loc;
    }
}

[System.Serializable]
public class BuildingLocationData {
    public Coords coords;
    public Coords size;
    public VoxelData front;

    public BuildingLocationData(Coords coords, Coords size, VoxelData front) {
        this.coords = coords;
        this.size = size;
        this.front = front;
    }
}