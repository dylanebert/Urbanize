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
        int x = (int)this.voxel.transform.position.x;
        int y = (int)this.voxel.transform.position.z;
        if (gameController.world.GetProperty(x, y, "isLand")) {
            title.text = "Land";
        } else if(gameController.world.GetProperty(x, y, "isOcean")) {
            title.text = "Ocean";
        } else if(gameController.world.GetProperty(x, y, "isLake")) {
            title.text = "Lake";
        }      
        StartCoroutine(Show());
    }

    public override void Close() {
        voxel.Deselect();
    }
}
