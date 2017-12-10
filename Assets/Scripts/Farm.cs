using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour {

    public static float MaxYield = 5f;

    public FarmData farmData;

    GameController gameController;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void Initialize() {
        farmData = new FarmData(Util.GroundVector2(transform.position));
        gameController.worldData.farmData.Add(farmData);
        gameController.farmDict.Add(farmData, this);
        gameController.worldData.voxels[Util.GroundVector2(transform.position)].hasFarm = true;
        this.gameObject.name = "Farm " + gameController.worldData.farmData.Count;
    }

    private void Update() {
        farmData.yield = Mathf.Min(MaxYield, farmData.yield + Time.deltaTime * .1f);
    }
}

[System.Serializable]
public class FarmData {
    public Vector2 coords;
    public float yield;

    public FarmData(Vector2 coords) {
        this.coords = coords;
    }
}
