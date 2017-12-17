using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    [HideInInspector]
    public RockData data;

    GameController gameController;
    bool selected;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void Initialize(RockData rockData) {
        this.data = rockData;
        UpdateDict();
    }

    public void UpdateDict() {
        if (!gameController.rockDict.ContainsKey(data))
            gameController.rockDict.Add(data, this);
        else
            gameController.rockDict[data] = this;
    }
}

[System.Serializable]
public class RockData {
    public Coords coords;
    public int type;
    public float rotation;

    public RockData(Coords coords, int type, float rotation) {
        this.coords = coords;
        this.type = type;
        this.rotation = rotation;
    }
}
