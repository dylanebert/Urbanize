using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour, IWorldSelectable {

    public GameObject voxelInfoWindowObj;

    [HideInInspector]
    public bool visited;
    [HideInInspector]
    public VoxelData data;

    GameController gameController;
    VoxelInfoWindow voxelInfoWindow;
    bool selected;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void Initialize(VoxelData voxelData) {
        data = voxelData;
        transform.position = Util.CoordsToVector3(data.coords);
        gameObject.name = voxelData.coords.ToString();
        gameObject.layer = voxelData.isLand ? 8 : 4;
    }

    public void Select() {
        if (selected)
            return;
        voxelInfoWindow = Instantiate(voxelInfoWindowObj, Util.GroundVector3(transform.position), Quaternion.identity, this.transform).GetComponent<VoxelInfoWindow>();
        voxelInfoWindow.Initialize(this);
        gameController.pointer.SetSelectIndicatorPosition(Util.GroundVector2(transform.position), Vector2.one);
        selected = true;
    }

    public void Deselect() {
        if (!selected)
            return;
        StartCoroutine(voxelInfoWindow.CloseCoroutine());
        gameController.pointer.ShowSelectIndicator(false);
        selected = false;
    }

    public void Hover() {
        gameController.pointer.SetCursorIndicatorPosition(Util.GroundVector2(transform.position), Vector2.one);
    }

    public void Dehover() {
        gameController.pointer.ShowCursorIndicator(false);
    }
}

[System.Serializable]
public class VoxelData {
    public Vector2 coords;
    public bool isLand;
    public bool isOcean;
    public bool isLake;
    public bool occupied;
    public bool navigable;
    public bool claimed;
    public bool hasTrees;
    public bool hasStorehouse;
    public bool hasFarm;
}