using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Human : Agent {

    public static float MaxMoveTime = 60f;
    public static float DeathHunger = 10f;
    public static float ChopSpeed = 5f;
    public static float HarvestSpeed = 5f;

    public MeshRenderer shirtRenderer;
    public float hunger;

    [HideInInspector]
    public GameController gameController;
    [HideInInspector]
    public HumanData data;
    [HideInInspector]
    public Animator animator;

    TinyverseAcademy academy;
    ActionReward[] actions;
    ActionReward currentAction;
    NavMeshAgent navAgent;
    Vector3 startPos;

    private void Awake() {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        gameController = Util.FindGameController();
        if (gameController.mode == GameController.Mode.Reinforce) {
            academy = gameController.academy;
            brain = academy.GetComponentInChildren<Brain>();
        }
        gameController.AddHuman(this);
        actions = GameObject.FindGameObjectWithTag("GameController").GetComponentsInChildren<ActionReward>();
        this.gameObject.name = Util.Names[Random.Range(0, Util.Names.Count)];
        shirtRenderer.material.color = Util.ShirtColors[Random.Range(0, Util.ShirtColors.Count)];
        data = new HumanData();
    }

    private void Start() {
        NavMeshHit hit;
        if(NavMesh.SamplePosition(transform.position, out hit, 200f, NavMesh.AllAreas)) {
            transform.position = startPos = new Vector3(hit.position.x, 0, hit.position.z);
        } else {
            throw new System.Exception("Couldn't find navmesh for " + gameObject.name);
        }
        navAgent.enabled = true;

        if (gameController.mode == GameController.Mode.Basic) {
            StartCoroutine(BasicAct());
        }
    }

    private void Update() {
        hunger += Time.deltaTime * .01667f;
    }

    IEnumerator BasicAct() {
        while (true) {
            ActionReward bestAction = null;
            float maxReward = float.MinValue;
            foreach(ActionReward action in actions) {
                float reward = action.GetReward(this);
                if(reward > maxReward) {
                    bestAction = action;
                    maxReward = reward;
                }
            }
            if(maxReward >= 0) {
                yield return bestAction.PerformAction(this);
            } else {
                yield return MoveTo(transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));
                yield return Wait(5f);
            }
        }
    }

    public override List<float> CollectState() {
        List<float> state = new List<float> {
            hunger
        };

        foreach (float f in academy.GetWorldState())
            state.Add(f);

        return state;
    }

    public override void AgentStep(float[] act) {
        int action = Mathf.FloorToInt(act[0]);
        if(!busy) { 
            currentAction = actions[action];
            StartCoroutine(currentAction.PerformAction(this));
            //Debug.Log(currentAction.ToString());
        }
    }

    public override void AgentReset() {
        if (busy)
            currentAction.StopAllCoroutines();            
        busy = false;
        hunger = 5f;
        transform.position = startPos;
        if (!academy.isInference)
            academy.AcademyReset();
    }

    IEnumerator Wait(float duration) {
        busy = true;
        yield return new WaitForSeconds(duration);
        busy = false;
    }

    public IEnumerator MoveTo(Vector3 tarPos) {
        if (!CanReach(tarPos))
            yield break;
        animator.SetBool("Walking", true);
        navAgent.SetDestination(tarPos);
        navAgent.isStopped = false;
        float totalTime = 0f;
        while (true) {
            totalTime += Time.deltaTime;
            if (!navAgent.pathPending && navAgent.remainingDistance < .1f) {
                navAgent.isStopped = true;
                break;
            }
            if(totalTime > MaxMoveTime) {
                transform.position = tarPos;
                break;
            }
            yield return null;
        }
        animator.SetBool("Walking", false);
    }

    public IEnumerator MoveToPrecise(Vector3 tarPos, float maxDistance) {
        if (!CanReach(tarPos))
            yield break;
        navAgent.SetDestination(tarPos);
        navAgent.isStopped = false;
        float totalTime = 0f;
        while (true) {
            totalTime += Time.deltaTime;
            if (!navAgent.pathPending && navAgent.remainingDistance < maxDistance) {
                navAgent.isStopped = true;
                break;
            }
            if (totalTime > MaxMoveTime) {
                transform.position = tarPos;
                break;
            }
            yield return null;
        }
    }

    public bool CanReach(Vector3 pos) {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
            return true;
        return false;
    }

    public Coords FindUnoccupied() {
        Dictionary<VoxelData, bool> visited = new Dictionary<VoxelData, bool>();
        foreach (VoxelData v in gameController.data.voxelData)
            visited.Add(v, false);
        Stack<VoxelData> stack = new Stack<VoxelData>();
        Coords coords = Util.Vector3ToCoords(transform.position);
        stack.Push(gameController.data.voxelData[coords.x, coords.y]);
        while(stack.Count > 0) {
            VoxelData v = stack.Pop();
            visited[v] = true;
            if (!v.occupied && !v.claimed && CanReach(Util.CoordsToVector3(v.coords)))
                return v.coords;
            foreach (Coords adj in Util.GetAdjacent(coords, true)) {
                VoxelData vAdj = gameController.data.voxelData[adj.x, adj.y];
                if (vAdj.navigable && !visited[vAdj])
                    stack.Push(vAdj);
            }
        }

        return Coords.Invalid;
    }

    public BuildingLocationData FindUnoccupiedBuildingSpace() {
        Dictionary<VoxelData, bool> visited = new Dictionary<VoxelData, bool>();
        foreach (VoxelData v in gameController.data.voxelData)
            visited.Add(v, false);
        Stack<VoxelData> stack = new Stack<VoxelData>();
        Coords coords = Util.Vector3ToCoords(transform.position);
        stack.Push(gameController.data.voxelData[(int)coords.x, (int)coords.y]);
        while (stack.Count > 0) {
            VoxelData v = stack.Pop();
            visited[v] = true;
            if (!v.occupied && !v.claimed)
                foreach(Coords adj in Util.GetAdjacent(v.coords, false)) {
                    VoxelData vAdj = gameController.data.voxelData[adj.x, adj.y];
                    if (vAdj.navigable && CanReach(Util.CoordsToVector3(vAdj.coords)))
                        return new BuildingLocationData(v.coords, new Coords(1, 1), vAdj);
                }
            foreach (Coords adj in Util.GetAdjacent(v.coords, true)) {
                VoxelData vAdj = gameController.data.voxelData[adj.x, adj.y];
                if (vAdj.navigable && !visited[vAdj])
                    stack.Push(vAdj);
            }
        }

        return null;
    }

    public Farm FindFarm(bool harvestable) {
        Farm farm = null;
        float minDist = float.MaxValue;
        Vector3 startPos = transform.position;
        if(data.lastHarvested != null) {
            if(data.lastHarvested.pendingRows > 0 && data.lastHarvested.pendingYield == 1f) {
                if (gameController.farmDict.ContainsKey(data.lastHarvested))
                    return gameController.farmDict[data.lastHarvested];
            } else {
                startPos = Util.CoordsToVector3(data.lastHarvested.coords);
                data.lastHarvested = null;
            }
        }
        foreach (FarmData fd in gameController.data.farmData) {
            if (gameController.farmDict.ContainsKey(fd) && (!harvestable || (fd.pendingYield == 1f && fd.pendingRows > 0))) {
                float dist = (Util.CoordsToVector3(fd.coords) - startPos).sqrMagnitude;
                if (dist < minDist) {
                    farm = gameController.farmDict[fd];
                    minDist = dist;
                }
            }
        }
        return farm;
    }

    public Food FindFood() {
        Food food = null;
        float dist = float.MaxValue;
        foreach(Resource resource in gameController.looseResources) {
            if(resource is Food) {
                float d = (resource.transform.position - transform.position).sqrMagnitude;
                if(d < dist) {
                    dist = d;
                    food = (Food)resource;
                }
            }
        }
        return food;
    }

    public Granary FindGranary(bool requireFood) {
        Granary granary = null;
        float dist = float.MaxValue;
        Coords coords = Util.Vector3ToCoords(transform.position);
        foreach(GranaryData sd in gameController.data.granaryData) {
            if (gameController.granaryDict.ContainsKey(sd) && (!requireFood || (sd.food > 0 && sd.pendingFood > 0))) {
                Granary s = gameController.granaryDict[sd];
                float d = (new Vector2(coords.x, coords.y) - new Vector2(sd.buildingData.loc.coords.x, sd.buildingData.loc.coords.y)).sqrMagnitude;
                if (d < dist && CanReach(s.front.transform.position)) {
                    dist = d;
                    granary = s;
                }
            }
        }
        return granary;
    }

    public Tree FindNearestTree() {
        Tree tree = null;
        Coords coords = Util.Vector3ToCoords(transform.position);
        float dist = float.MaxValue;
        foreach (TreeData td in this.gameController.data.treeData) {
            float d = (new Vector2(coords.x, coords.y) - new Vector2(td.voxelCoords.x, td.voxelCoords.y)).sqrMagnitude;
            Tree t = gameController.treeDict[td];
            if (d < dist) {
                if (!t.claimed && this.CanReach(Util.CoordsToVector3(td.voxelCoords))) {
                    dist = d;
                    tree = t;
                }
            }
        }
        if(tree != null)
            tree.claimed = true;
        return tree;
    }
}

[System.Serializable]
public class HumanData {
    public FarmData lastHarvested;
}