using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockInfoWindow : WorldWindow {

    Rock rock;

    private void Start() {
        transform.position += Vector3.up * 1.5f;
    }

    public override void Initialize(object rock) {
        this.rock = (Rock)rock;
        StartCoroutine(Show());
    }

    public override void Close() {
        rock.Deselect();
    }
}
