using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    public delegate void DropWood(Wood wood);
    public DropWood dropWood;

    public static float ShakeInterval = .05f;

    [HideInInspector]
    public TreeData data;
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public bool claimed;

    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(TreeData treeData) {
        this.data = treeData;
        UpdateDict();
    }

    public void UpdateDict() {
        if (!gameController.treeDict.ContainsKey(data))
            gameController.treeDict.Add(data, this);
        else
            gameController.treeDict[data] = this;
    }

    public void Destroy() {
        gameController.data.treeData.Remove(data);
        gameController.treeDict.Remove(data);
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        if(Physics.OverlapBox(transform.position, Vector3.one * .5f, Quaternion.identity, LayerMask.GetMask("Tree")).Length == 0) {
            gameController.data.voxelData[data.voxelCoords.x, data.voxelCoords.y].occupied = false;
            gameController.data.voxelData[data.voxelCoords.x, data.voxelCoords.y].hasTrees = false;
        }
        Destroy(this.gameObject);
    }
}

[System.Serializable]
public class TreeData {
    public Coords treeCoords;
    public Coords voxelCoords;
    public int type;
    public float rotation;

    public TreeData(Coords treeCoords, Coords voxelCoords, int type, float rotation) {
        this.treeCoords = treeCoords;
        this.voxelCoords = voxelCoords;
        this.type = type;
        this.rotation = rotation;
    }
}
