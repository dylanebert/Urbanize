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

    public void Initialize(Voxel voxel) {
        this.voxel = voxel;
        switch(voxel.type) {
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
        gameController.ShowIndicator(voxel.gameObject, voxel.coords, Vector2.one);
        StartCoroutine(Show());
    }

    public override void Close() {
        if (gameController.indicator.target == this.voxel.gameObject)
            gameController.HideIndicator();
        base.Close();
    }
}
