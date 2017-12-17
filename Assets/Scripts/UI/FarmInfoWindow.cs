using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmInfoWindow : WorldWindow {

    public Text title;
    public Image yieldBar;

    Farm farm;

    private void Start() {
        transform.position += Vector3.up * 1.5f;
    }

    public override void Initialize(object farm) {
        this.farm = (Farm)farm;
        title.text = this.farm.name;
        StartCoroutine(Show());
    }

    public override void Close() {
        farm.Deselect();
    }

    protected override void Update() {
        base.Update();
        yieldBar.fillAmount = Mathf.Clamp01(farm.data.yield / 1f);
        if (farm.data.yield == 1f)
            yieldBar.color = Palette.YellowFade;
        else
            yieldBar.color = Palette.WhiteFade;
    }
}
