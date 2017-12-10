using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : Agent {

    public static float DeathHunger = 10f;

    public TestAcademy academy;

    [HideInInspector]
    public float hunger;

	public override List<float> CollectState()
	{
		List<float> state = new List<float>();

        //Hunger
        state.Add(hunger);

        foreach (float f in academy.GetWorldState())
            state.Add(f);

		return state;
	}

	public override void AgentStep(float[] act)
	{
        int action = Mathf.FloorToInt(act[0]);
        reward = -.01f;
        hunger += .1f;

        //0 - build storehouse, 1 - build farm, 2 - harvest, 3 - eat
        switch(action) {
            case 0:
                TestStorehouse storehouse = new TestStorehouse();
                academy.storehouses.Add(storehouse);
                reward = -5f;
                break;
            case 1:
                TestFarm farm = new TestFarm();
                academy.farms.Add(farm);
                reward = -5f;
                break;
            case 2:
                if(academy.storehouses.Count > 0) {
                    foreach(TestFarm f in academy.farms) {
                        if(f.yield >= TestFarm.MaxYield) {
                            f.yield = 0;
                            academy.storehouses[0].food += (int)TestFarm.MaxYield;
                            break;
                        }
                    }
                }
                reward = -1f;
                break;
            case 3:
                bool ate = false;
                foreach(TestStorehouse s in academy.storehouses) {
                    if(s.food > 0) {
                        s.food--;
                        reward = hunger;
                        hunger = 0f;
                        ate = true;
                        break;
                    }
                }
                if (!ate)
                    reward = -1f;
                break;
        }

        if(hunger >= DeathHunger) {
            reward = -100f;
            done = true;
        }
	}

	public override void AgentReset()
	{
        hunger = 5f;
	}

	public override void AgentOnDone()
	{

	}
}
