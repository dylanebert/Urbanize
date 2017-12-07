using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoxelInfoWindow : WorldWindow {

    public Text title;

    Voxel voxel;

    protected override void Awake() {
        base.Awake();

        transform.position += Vector3.up;
    }

    public override void Initialize(object voxel) {
        this.voxel = (Voxel)voxel;
        switch(this.voxel.type) {
            case 0:
                title.text = "Ocean";
                break;
            case 1:
                title.text = "Land";
                break;
            case 2:
                title.text = "Lake";
                break;
            default:
                break;
        }        
        StartCoroutine(Show());
    }

    public override void Close() {
        voxel.Deselect();
    }
}
