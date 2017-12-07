using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ploppable : MonoBehaviour {

    public LayerMask validLayer;
    public Material ploppableMat;
    public Material treeMat;
    public MeshRenderer meshRenderer;

    GameController gameController;
    UIController uiController;
    Material originalMat;
    Vector2 prevCoords;
    float mouseDownTime;
    bool valid;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1))
            Cancel();
        if(Input.GetButtonDown("Rotate")) {
            transform.Rotate(Vector3.up, 90f);
            UpdateValid(prevCoords);
        }

        if(gameController.pointer.hover is Voxel) {
            Vector2 coords = ((Voxel)gameController.pointer.hover).coords;
            if (coords != prevCoords) {
                foreach (Tree tree in gameController.grid[prevCoords].trees)
                    tree.meshRenderer.material = treeMat;
                prevCoords = coords;
                transform.position = new Vector3(coords.x + .5f, 0, coords.y + .5f);
                UpdateValid(coords);
            }
        }
    }

    private void OnEnable() {
        originalMat = meshRenderer.material;
        meshRenderer.material = ploppableMat;
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = false;
        gameController.pointer.SetCursorIndicatorType(1);
        gameController.pointer.pointerClick += Plop;
        gameController.pointer.raycastLayer = gameController.pointer.groundCastLayer;
        gameController.pointer.Deselect();
    }

    private void OnDisable() {
        foreach (Tree tree in gameController.grid[prevCoords].trees)
            tree.meshRenderer.material = treeMat;
        meshRenderer.material = originalMat;
        GetComponent<Collider>().enabled = true;
        GetComponent<NavMeshObstacle>().enabled = true;
        gameController.pointer.SetCursorIndicatorType(0);
        gameController.pointer.pointerClick -= Plop;
        gameController.pointer.raycastLayer = gameController.pointer.allCastLayer;
    }

    public void Cancel() {
        Destroy(this.gameObject);
    }

    public void Plop() {
        if (!valid) return;
        Voxel voxel = gameController.grid[prevCoords];
        Stack<Tree> trees = new Stack<Tree>(voxel.trees);
        while (trees.Count > 0) {
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
        Voxel voxel = gameController.grid[coords];
        if (validLayer == (validLayer | (1 << voxel.gameObject.layer))) {
            if (!voxel.occupied && !voxel.buildingFront) {
                if (gameController.grid[coords + new Vector2(transform.forward.x, transform.forward.z)].navigable) {
                    foreach (Tree tree in voxel.trees) {
                        tree.meshRenderer.material = ploppableMat;
                        tree.meshRenderer.material.color = Palette.FadeWarning;
                    }
                    meshRenderer.material.color = Palette.FadeValid;
                    gameController.pointer.SetCursorIndicatorColor(Palette.FadeValid);
                    valid = true;
                    return;
                }
            }
        }
        meshRenderer.material.color = Palette.FadeInvalid;
        gameController.pointer.SetCursorIndicatorColor(Palette.FadeInvalid);
        valid = false;
    }
}
