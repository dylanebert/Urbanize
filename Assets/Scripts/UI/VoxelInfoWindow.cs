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
        Coords coords = this.voxel.data.coords;
        if (gameController.data.voxelData[(int)coords.x, (int)coords.y].isLand) {
            title.text = "Land";
        } else if(gameController.data.voxelData[(int)coords.x, (int)coords.y].isOcean) {
            title.text = "Ocean";
        } else if(gameController.data.voxelData[(int)coords.x, (int)coords.y].isLake) {
            title.text = "Lake";
        }      
        StartCoroutine(Show());
    }

    public override void Close() {
        voxel.Deselect();
    }
}
