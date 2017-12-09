using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour, IWorldSelectable {

    public GameObject rockInfoWindowObj;

    RockInfoWindow rockInfoWindow;
    GameController gameController;
    bool selected;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        GetComponentInChildren<MeshRenderer>().transform.Rotate(Vector3.up, Random.Range(0, 360));
        Vector2 coords = Util.GroundVector2(transform.position);
        gameController.worldData.voxels[coords].occupied = true;
        gameController.worldData.voxels[coords].navigable = false;
    }

    public void Dehover() {
        gameController.pointer.ShowCursorIndicator(true);
    }

    public void Deselect() {
        if (!selected)
            return;
        StartCoroutine(rockInfoWindow.CloseCoroutine());
        gameController.pointer.ShowSelectIndicator(false);
        selected = false;
    }

    public void Hover() {
        gameController.pointer.SetCursorIndicatorPosition(Util.GroundVector2(transform.position), Vector2.one);
    }

    public void Select() {
        if (selected)
            return;
        gameController.pointer.SetSelectIndicatorPosition(Util.GroundVector2(transform.position), Vector2.one);
        rockInfoWindow = Instantiate(rockInfoWindowObj, Util.GroundVector3(transform.position), Quaternion.identity, this.transform).GetComponent<RockInfoWindow>();
        rockInfoWindow.Initialize(this);
        selected = true;
    }
}
