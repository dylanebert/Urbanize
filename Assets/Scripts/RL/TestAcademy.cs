using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAcademy : Academy {

    public bool logState;
    public List<GranaryData> granaries;
    public List<FarmData> farms;
    public int looseFood;

    public override void InitializeAcademy() {
        
    }

    public override void AcademyReset()
	{
        granaries = new List<GranaryData>();
        farms = new List<FarmData>();
        looseFood = 0;
	}

	public override void AcademyStep()
	{
        foreach (FarmData farm in farms)
            farm.yield = Mathf.Min(1f, farm.yield + .1f);
	}

    public List<float> GetWorldState() {
        List<float> state = new List<float>();

        //Number of granaries
        state.Add(granaries.Count);

        //Amount of food in granaries
        int food = 0;
        foreach (GranaryData granary in granaries)
            food += granary.food;
        state.Add(food);
        
        //Number of farms
        state.Add(farms.Count);

        //Number of harvestable farms
        int harvestable = 0;
        foreach (FarmData farm in farms)
            if (farm.yield == 1f && farm.rows > 0)
                harvestable++;
        state.Add(harvestable);

        //Amount of loose food
        state.Add(looseFood);

        string log = "[";
        foreach (float f in state) {
            log += " " + f.ToString("f2") + " ";
        }
        log += "]";
        if (logState)
            Debug.Log(log);

        return state;
    }
}
