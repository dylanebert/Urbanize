using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ploppable : MonoBehaviour {

    public LayerMask hitLayer;
    public LayerMask validLayer;
    public Material ploppableMat;
    public Material treeMat;
    public MeshRenderer meshRenderer;

    TerrainGenerator terrainGenerator;
    GameController gameController;
    UIController uiController;
    Material originalMat;
    Vector2 prevCoords;
    float mouseDownTime;
    bool valid;

    private void Awake() {
        terrainGenerator = GameObject.FindGameObjectWithTag("GameController").GetComponent<TerrainGenerator>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0))
            mouseDownTime = Time.time;
        if(Input.GetMouseButtonUp(0))
            if(Time.time - mouseDownTime < .25f)
                if (valid)
                    Plop();
        if(Input.GetButtonDown("Rotate")) {
            transform.Rotate(Vector3.up, 90f);
            UpdateValid(prevCoords);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray.origin, ray.direction, out hit, 100f, hitLayer, QueryTriggerInteraction.Ignore)) {
            if (hitLayer == (hitLayer | (1 << hit.transform.gameObject.layer))) {
                Vector2 coords = new Vector2((int)hit.point.x, (int)hit.point.z);
                if (coords != prevCoords) {
                    foreach(Tree tree in terrainGenerator.grid[prevCoords].trees) {
                        tree.meshRenderer.material = treeMat;
                    }
                    prevCoords = coords;
                    transform.position = new Vector3(coords.x + .5f, 0, coords.y + .5f);
                    UpdateValid(coords);
                }
            }
        }
    }

    private void OnEnable() {
        originalMat = meshRenderer.material;
        meshRenderer.material = ploppableMat;
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = false;
    }

    private void OnDisable() {
        meshRenderer.material = originalMat;
        GetComponent<Collider>().enabled = true;
        GetComponent<NavMeshObstacle>().enabled = true;
    }

    public void Cancel() {
        Destroy(this.gameObject);
    }

    public void Plop() {
        Voxel voxel = terrainGenerator.grid[prevCoords];
        Stack<Tree> trees = new Stack<Tree>(voxel.trees);
        while(trees.Count > 0) {
            Tree tree = trees.Pop();
            tree.Destroy();
        }
        Building building = GetComponent<Building>();
        if(building != null)
            building.Initialize(voxel);
        uiController.ResetPloppable();
        this.enabled = false;
    }

    void UpdateValid(Vector2 coords) {
        Voxel voxel = terrainGenerator.grid[coords];
        if (validLayer == (validLayer | (1 << voxel.gameObject.layer))) {
            if (!voxel.occupied && !voxel.buildingFront) {
                if (terrainGenerator.grid[coords + new Vector2(transform.forward.x, transform.forward.z)].navigable) {
                    foreach (Tree tree in voxel.trees) {
                        tree.meshRenderer.material = ploppableMat;
                        tree.meshRenderer.material.color = Palette.FadeWarning;
                    }
                    ploppableMat.color = Palette.FadeValid;
                    valid = true;
                    return;
                }
            }
        }
        ploppableMat.color = Palette.FadeInvalid;
        valid = false;
    }
}
