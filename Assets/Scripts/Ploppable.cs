using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ploppable : MonoBehaviour {

    public Material ploppableMat;
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
            Vector2 coords = Util.GroundVector2(((Voxel)gameController.pointer.hover).transform.position);
            if (coords != prevCoords) {
                prevCoords = coords;
                transform.position = Util.CoordsToVector3(coords);
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
        Building building = GetComponent<Building>();
        uiController.ResetPloppable();
        building.Initialize();
        this.enabled = false;
    }

    void UpdateValid(Vector2 coords) {
        if (gameController.worldData.voxels[coords].isLand) {
            if (!gameController.worldData.voxels[coords].occupied && !gameController.worldData.voxels[coords].claimed) {
                if (gameController.worldData.voxels[coords + Util.GroundVector2(transform.forward)].navigable) {
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
