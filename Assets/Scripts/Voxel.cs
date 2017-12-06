using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour {

    public List<Voxel> adjacent;
    public List<Voxel> neighbors;
    public List<Tree> trees;
    public Vector2 coords;
    public int type; //0 - ocean, 1 - land, 2 - lake
    public bool navigable;
    public bool occupied;
    public bool buildingFront;

    [HideInInspector]
    public bool visited;

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
}
