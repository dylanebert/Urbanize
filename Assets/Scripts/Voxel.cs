using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour, IWorldSelectable {

    public GameObject voxelInfoWindowObj;
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
        UpdateDict();
    }

    public void UpdateDict() {
        if (!gameController.voxelDict.ContainsKey(data))
            gameController.voxelDict.Add(data, this);
        else
            gameController.voxelDict[data] = this;
    }

    public void Select() {
        if (selected)
            return;
        voxelInfoWindow = Instantiate(voxelInfoWindowObj, transform.position, Quaternion.identity, this.transform).GetComponent<VoxelInfoWindow>();
        voxelInfoWindow.Initialize(this);
        gameController.pointer.SetSelectIndicatorPosition(Util.Vector3ToCoords(transform.position), new Coords(1, 1));
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
        gameController.pointer.SetCursorIndicatorPosition(Util.Vector3ToCoords(transform.position), new Coords(1, 1));
    }

    public void Dehover() {
        gameController.pointer.ShowCursorIndicator(false);
    }

    public void UpdateInfo() {

    }
}

[System.Serializable]
public class VoxelData {
    public Coords coords;
    public SerializableColor color;
    public bool isLand;
    public bool isOcean;
    public bool isLake;
    public bool occupied;
    public bool navigable;
    public bool claimed;
    public bool hasTrees;
    public bool hasGranary;
    public bool hasFarm;

    public void UpdateClaimed(VoxelData[,] voxelData) {
        List<VoxelData> adjNavigable = new List<VoxelData>();
        foreach (VoxelData v in Util.CoordsToVoxels(voxelData, Util.GetAdjacent(coords, false))) {
            if (v.navigable)
                adjNavigable.Add(v);
        }
        if (adjNavigable.Count == 1)
            adjNavigable[0].claimed = true;
    }
}