using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TinyverseAcademy : Academy {

    public bool logState;
    GameController gameController;

    public override void InitializeAcademy() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public override void AcademyReset() {
        gameController.ResetWorld();
    }

    public override void AcademyStep() {
        gameController.Step();
    }

    public List<float> GetWorldState() {
        List<float> state = new List<float>();

        //Number of granaries
        state.Add(gameController.data.granaryData.Count);

        //Amount of food in granaries
        int food = 0;
        foreach (GranaryData granaryData in gameController.data.granaryData)
            food += granaryData.pendingFood;
        state.Add(food);

        //Number of farms
        state.Add(gameController.data.farmData.Count);

        //Number of harvestable farms
        int harvestable = 0;
        foreach (FarmData farmData in gameController.data.farmData)
            if (farmData.pendingYield == 1f && farmData.pendingRows > 0) {
                harvestable++;
            }
        state.Add(harvestable);

        //Amount of loose food
        state.Add(gameController.looseResources.Count(x => x is Food));

        string log = "[";
        foreach(float f in state) {
            log += " " + f.ToString("f2") + " ";
        }
        log += "]";
        if(logState)
            Debug.Log(log);

        return state;
    }
}
