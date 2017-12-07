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
        this.gameObject.name = GameController.Names[Random.Range(0, GameController.Names.Count)];
        shirtRenderer.material.color = GameController.ShirtColors[Random.Range(0, GameController.ShirtColors.Count)];
        state = new HumanState();
    }

    private IEnumerator Start() {
        while (!gameController.ready)
            yield return null;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(transform.position, out hit, 200f, NavMesh.AllAreas)) {
            Voxel voxel = gameController.grid[new Vector2((int)hit.position.x, (int)hit.position.z)];
            transform.position = new Vector3(voxel.transform.position.x, 0, voxel.transform.position.z);
        } else {
            throw new System.Exception("Couldn't find navmesh for " + gameObject.name);
        }
        navAgent.enabled = true;

        while(true)
            yield return StartCoroutine(actions[0].PerformAction(this, 60f));
    }

    public IEnumerator MoveTo(Vector3 tarPos) {
        navAgent.SetDestination(tarPos);
        navAgent.isStopped = false;
        while (true) {
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
        foreach (Building building in gameController.buildings) {
            if (building is Storehouse) {
                float d = (transform.position - building.transform.position).sqrMagnitude;
                if (d < dist && CanReach(building.front.transform.position)) {
                    dist = d;
                    storehouse = (Storehouse)building;
                }
            }
        }
        return storehouse;
    }

    public bool GetNextWood() {
        if (this.state.targetResource != null)
            return true;
        Wood wood = null;
        Vector2 coords = Vector2.zero;
        if (state.lastTreeChoppedCoords != Vector2.zero)
            coords = state.lastTreeChoppedCoords;
        else
            coords = new Vector2(this.transform.position.x, this.transform.position.z);
        float dist = float.MaxValue;
        foreach (Wood w in this.gameController.wood) {
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

    public Tree GetNextTree() {
        Tree tree = null;
        Vector2 coords = new Vector2(this.transform.position.x, this.transform.position.z);
        if (this.state.lastTreeChoppedCoords != Vector2.zero)
            coords = this.state.lastTreeChoppedCoords;
        float dist = float.MaxValue;
        foreach (Tree t in this.gameController.trees) {
            float d = (coords - new Vector2(t.transform.position.x, t.transform.position.z)).sqrMagnitude;
            if (d < dist) {
                if (!t.claimed && this.CanReach(t.transform.position)) {
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
    public Vector2 lastTreeChoppedCoords; //Coordinates of the last chopped tree (preference to return to same area)
    public Resource targetResource; //Resource currently going after
    public Resource holding; //Resource currently being held
    public float chopSpeed = 5f; //Time taken to chop a tree
}