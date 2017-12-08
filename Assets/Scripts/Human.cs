using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour {

    public MeshRenderer shirtRenderer;

    [HideInInspector]
    public GameController gameController;
    [HideInInspector]
    public HumanState state;

    ActionReward[] actions;
    NavMeshAgent navAgent;

    private void Awake() {
        navAgent = GetComponent<NavMeshAgent>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameController.AddHuman(this);
        actions = GameObject.FindGameObjectWithTag("GameController").GetComponentsInChildren<ActionReward>();
        this.gameObject.name = Util.Names[Random.Range(0, Util.Names.Count)];
        shirtRenderer.material.color = Util.ShirtColors[Random.Range(0, Util.ShirtColors.Count)];
        state = new HumanState();
    }

    private IEnumerator Start() {
        NavMeshHit hit;
        if(NavMesh.SamplePosition(transform.position, out hit, 200f, NavMesh.AllAreas)) {
            transform.position = Util.GroundVector3(hit.position);
            state.coords = Util.GroundVector2(transform.position);
        } else {
            throw new System.Exception("Couldn't find navmesh for " + gameObject.name);
        }
        navAgent.enabled = true;

        while(true)
            yield return StartCoroutine(actions[0].PerformAction(this));
    }

    public IEnumerator MoveTo(Vector3 tarPos) {
        navAgent.SetDestination(tarPos);
        navAgent.isStopped = false;
        while (true) {
            state.coords = Util.GroundVector2(transform.position);
            if (!navAgent.pathPending && navAgent.remainingDistance < .5f) {
                navAgent.isStopped = true;
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

    public Storehouse FindNearestStorehouse() {
        Storehouse storehouse = null;
        float dist = float.MaxValue;
        Vector2 coords = Util.GroundVector2(transform.position);
        foreach(StorehouseData sd in gameController.world.storehouseData) {
            Storehouse s = gameController.storehouseDict[sd];
            float d = (coords - sd.buildingData.coords).sqrMagnitude;
            if(d < dist && CanReach(s.front.transform.position)) {
                dist = d;
                storehouse = s;
            }
        }
        return storehouse;
    }

    public bool FindNearestWood() {
        if (this.state.targetResource != null)
            return true;
        Wood wood = null;
        Vector2 coords = Vector2.zero;
        if (state.lastTreeChoppedCoords != Vector2.zero)
            coords = state.lastTreeChoppedCoords;
        else
            coords = new Vector2(this.transform.position.x, this.transform.position.z);
        float dist = float.MaxValue;
        foreach (Wood w in gameController.wood) {
            float d = (coords - new Vector2(w.transform.position.x, w.transform.position.z)).sqrMagnitude;
            if (d < dist) {
                if (!w.claimed && this.CanReach(w.transform.position)) {
                    dist = d;
                    wood = w;
                }
            }
        }
        if (wood != null) {
            this.state.targetResource = wood;
            wood.claimed = true;
            return true;
        }
        return false;
    }

    public Tree FindNearestTree() {
        Tree tree = null;
        Vector2 coords = new Vector2(this.transform.position.x, this.transform.position.z);
        if (this.state.lastTreeChoppedCoords != Vector2.zero)
            coords = this.state.lastTreeChoppedCoords;
        float dist = float.MaxValue;
        foreach (TreeData td in this.gameController.world.treeData) {
            float d = (coords - td.pos).sqrMagnitude;
            Tree t = gameController.treeDict[td];
            if (d < dist) {
                if (!t.claimed && this.CanReach(Util.CoordsToVector3(td.pos))) {
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
public class HumanState {
    public Vector2 coords;
    public Vector2 lastTreeChoppedCoords; //Coordinates of the last chopped tree (preference to return to same area)
    public Resource targetResource; //Resource currently going after
    public Resource holding; //Resource currently being held
    public float chopSpeed = 5f; //Time taken to chop a tree
}