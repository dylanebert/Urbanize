using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GranaryInfoWindow : WorldWindow {

    public Text title;
    public Text foodText;

    Granary granary;

    private void Start() {
        transform.position += Vector3.up * 2f;
    }

    public override void Initialize(object storehouse) {
        this.granary = (Granary)storehouse;
        title.text = this.granary.name;
        StartCoroutine(Show());
    }

    protected override void Update() {
        base.Update();
        foodText.text = granary.data.food.ToString();
    }

    public override void Close() {
        granary.Deselect();
    }
}
