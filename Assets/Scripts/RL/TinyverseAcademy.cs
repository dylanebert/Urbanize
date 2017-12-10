using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyverseAcademy : Academy {

    GameController gameController;

    public override void InitializeAcademy() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public override void AcademyReset() {
        
    }

    public override void AcademyStep() {
        
    }

    public List<float> GetWorldState() {
        List<float> state = new List<float>();

        int totalFood = 0;
        foreach (StorehouseData storehouseData in gameController.worldData.storehouseData)
            totalFood += storehouseData.wood;
        float totalYield = 0f;
        foreach(FarmData farmData in gameController.worldData.farmData) {
            totalYield += farmData.yield;
        }

        //Storehouse exists
        state.Add(gameController.worldData.storehouseData.Count > 0 ? 1 : 0);

        //Total farm yield
        state.Add(totalYield);

        //Total food
        state.Add(totalFood);

        string log = "[";
        foreach(float f in state) {
            log += " " + f.ToString("f2") + " ";
        }
        log += "]";
        //Debug.Log(log);

        return state;
    }
}
