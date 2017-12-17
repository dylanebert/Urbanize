using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : Agent {

    public static float DeathHunger = 10f;

    public TestAcademy academy;
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
        
        switch(action) {
            case 0: //Build granary
                academy.granaries.Add(new GranaryData(null, ""));
                reward = Rewards.Build;
                break;
            case 1: //Build farm
                academy.farms.Add(new FarmData(null, ""));
                reward = Rewards.Build;
                break;
            case 2: //Harvest
                bool harvested = false;
                foreach(FarmData farm in academy.farms)
                    if(farm.yield == 1f && farm.rows > 0) {
                        if (--farm.rows == 0) {
                            farm.yield = 0;
                            farm.rows = 4;
                        }
                        academy.looseFood++;
                        harvested = true;
                        break;
                    }
                reward = harvested ? Rewards.HarvestSuccess : Rewards.ActionFail;
                break;
            case 3: //Eat
                bool ate = false;
                foreach(GranaryData granary in academy.granaries)
                    if(granary.food > 0) {
                        granary.food--;
                        hunger--;
                        ate = true;
                        break;
                    }
                if (ate) {
                    reward = Rewards.EatSuccess;
                    if (hunger <= 0)
                        done = true;
                }
                else
                    reward = Rewards.ActionFail;
                break;
            case 4: //Gather food
                if (academy.looseFood > 0 && academy.granaries.Count > 0) {
                    academy.looseFood--;
                    academy.granaries[0].food++;
                    reward = Rewards.GatherSuccess;
                }
                else
                    reward = Rewards.ActionFail;
                break;
            default: //Do nothing
                reward = Rewards.DoNothing;
                break;
        }

        hunger += .25f;
	}

	public override void AgentReset()
	{
        hunger = 0f;
	}

	public override void AgentOnDone()
	{

	}
}
