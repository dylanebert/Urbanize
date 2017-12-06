using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    public delegate void PointerClick();
    public PointerClick pointerClick;

    public LayerMask castLayer;
    public LayerMask voxelLayer;
    public LayerMask buildingLayer;
    public Transform cursorIndicator;
    public MeshRenderer[] renderers;

    [HideInInspector]
    public Voxel mouseOverVoxel;
    [HideInInspector]
    public MeshRenderer currentRenderer;

    GameController gameController;
    Vector2 prevCoords;
    Vector3 mouseDownPos;
    bool mouseDownActive;
    float mouseDownTime;

    private void Awake() {
        gameController = GetComponent<GameController>();
    }

    private void Start() {
        SetIndicator(0);
    }

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, castLayer, QueryTriggerInteraction.Ignore)) {
            if (voxelLayer == (voxelLayer | (1 << hit.transform.gameObject.layer))) {
                currentRenderer.gameObject.SetActive(true);
                Vector2 coords = new Vector2((int)hit.point.x, (int)hit.point.z);
                if (coords != prevCoords) {
                    prevCoords = coords;
                    mouseOverVoxel = gameController.grid[coords];
                    cursorIndicator.transform.position = new Vector3(coords.x + .5f, 0, coords.y + .5f);
                }
            } else {
                currentRenderer.gameObject.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            mouseDownTime = Time.unscaledTime;
            mouseDownPos = Input.mousePosition;
            mouseDownActive = currentRenderer.gameObject.activeSelf;
        }
        if (Input.GetMouseButtonUp(0))
            if (Time.unscaledTime - mouseDownTime < .25f && (Input.mousePosition - mouseDownPos).sqrMagnitude < 25f) {
                if (pointerClick != null) {
                    pointerClick();
                }
                else
                    Click();
            }
    }

    void Click() {
        if(mouseDownActive)
            mouseOverVoxel.ShowInfo();
    }

    public void SetIndicator(int index) {
        if (currentRenderer != null)
            currentRenderer.gameObject.SetActive(false);
        currentRenderer = renderers[index];
        if(currentRenderer != null)
            currentRenderer.gameObject.SetActive(true);
    }
}
