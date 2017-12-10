using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAcademy : Academy {

    public Text storehouseText;
    public Text farmText;

    [HideInInspector]
    public List<TestStorehouse> storehouses;
    [HideInInspector]
    public List<TestFarm> farms;

    public override void InitializeAcademy() {
        
    }

    public override void AcademyReset()
	{
        storehouses = new List<TestStorehouse>();
        farms = new List<TestFarm>();
	}

	public override void AcademyStep()
	{
        foreach (TestFarm farm in farms) {
            farm.yield = Mathf.Min(TestFarm.MaxYield, farm.yield + 1);
        }
	}

    public List<float> GetWorldState() {
        List<float> state = new List<float>();

        int totalFood = 0;
        foreach (TestStorehouse storehouse in storehouses)
            totalFood += storehouse.food;
        float totalYield = 0f;
        foreach (TestFarm farm in farms)
            totalYield += farm.yield;

        storehouseText.text = "Storehouses: " + storehouses.Count.ToString() + ", Food: " + totalFood.ToString();
        farmText.text = "Farms: " + farms.Count.ToString() + ", Yield: " + totalYield.ToString();

        //Storehouse exists
        state.Add(storehouses.Count > 0 ? 1 : 0);

        //Total farm yield
        state.Add(totalYield);

        //Total food
        state.Add(totalFood);

        return state;
    }
}

public class TestStorehouse {
    public int food;
}

public class TestFarm {
    public static int MaxYield = 5;
    public float yield;
}
