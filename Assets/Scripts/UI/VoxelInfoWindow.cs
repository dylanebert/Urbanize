using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoxelInfoWindow : WorldWindow {

    public Text title;

    Voxel voxel;

    private void Start() {
        transform.position += Vector3.up;
    }

    public override void Initialize(object voxel) {
        this.voxel = (Voxel)voxel;
        Vector2 coords = this.voxel.data.coords;
        if (gameController.worldData.voxels[coords].isLand) {
            title.text = "Land";
        } else if(gameController.worldData.voxels[coords].isOcean) {
            title.text = "Ocean";
        } else if(gameController.worldData.voxels[coords].isLake) {
            title.text = "Lake";
        }      
        StartCoroutine(Show());
    }

    public override void Close() {
        voxel.Deselect();
    }
}
