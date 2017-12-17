using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ploppable : MonoBehaviour {

    public Material ploppableMat;
    public MeshRenderer meshRenderer;
    public bool hasFront;

    GameController gameController;
    UIController uiController;
    Material originalMat;
    Coords prevCoords;
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
            Coords coords = Util.Vector3ToCoords(((Voxel)gameController.pointer.hover).transform.position);
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
        building.Initialize(null);
        this.enabled = false;
    }

    void UpdateValid(Coords coords) {
        if (gameController.data.voxelData[coords.x, coords.y].isLand) {
            if (!gameController.data.voxelData[coords.x, coords.y].occupied && !gameController.data.voxelData[coords.x, coords.y].claimed) {
                if (!hasFront || gameController.data.voxelData[(int)transform.position.x + (int)transform.forward.x, (int)transform.position.z + (int)transform.forward.z].navigable) {
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
