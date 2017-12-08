using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour, IWorldSelectable {

    public GameObject voxelInfoWindowObj;

    [HideInInspector]
    public bool visited;

    GameController gameController;
    VoxelInfoWindow voxelInfoWindow;
    bool selected;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
