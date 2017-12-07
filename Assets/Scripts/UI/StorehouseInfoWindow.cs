using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorehouseInfoWindow : WorldWindow {

    public Text title;
    public Text woodText;

    Storehouse storehouse;

    protected override void Awake() {
        base.Awake();

        transform.position += Vector3.up * 1.5f;
    }

    public override void Initialize(object storehouse) {
        this.storehouse = (Storehouse)storehouse;
        title.text = this.storehouse.name;
        StartCoroutine(Show());
    }

    protected override void Update() {
        base.Update();
        woodText.text = "Wood: " + storehouse.inventory.wood.ToString();
    }

    public override void Close() {
        storehouse.Deselect();
    }
}
