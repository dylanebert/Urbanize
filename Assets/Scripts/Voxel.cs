using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour, IWorldSelectable {

    public GameObject voxelInfoWindowObj;

    [HideInInspector]
    public List<Voxel> adjacent;
    [HideInInspector]
    public List<Voxel> neighbors;
    [HideInInspector]
    public List<Tree> trees;
    [HideInInspector]
    public Vector2 coords;
    [HideInInspector]
    public int type; //0 - ocean, 1 - land, 2 - lake
    [HideInInspector]
    public bool navigable;
    [HideInInspector]
    public bool occupied;
    [HideInInspector]
    public bool buildingFront;
    [HideInInspector]
    public bool visited;

    GameController gameController;
    VoxelInfoWindow voxelInfoWindow;
    bool selected;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void Initialize(Vector2 coords, int type) {
        this.coords = coords;
        this.type = type;
        this.gameObject.layer = type == 0 ? 4 : 8;
        this.navigable = type == 1;
    }

    public void AddTree(Tree tree) {
        trees.Add(tree);
        tree.Initialize(this);
    }

    public bool IsTypeNeighboringType(int a, int b) {
        if(type == a) {
            foreach(Voxel n in neighbors) {
                if (n.type == b)
                    return true;
            }
        }
        return false;
    }

    public void Select() {
        if (selected)
            return;
        voxelInfoWindow = Instantiate(voxelInfoWindowObj, Util.GroundVector3(transform.position), Quaternion.identity, this.transform).GetComponent<VoxelInfoWindow>();
        voxelInfoWindow.Initialize(this);
        gameController.pointer.SetSelectIndicatorPosition(coords, Vector2.one);
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
        gameController.pointer.SetCursorIndicatorPosition(coords, Vector2.one);
    }

    public void Dehover() {
        gameController.pointer.ShowCursorIndicator(false);
    }
}
