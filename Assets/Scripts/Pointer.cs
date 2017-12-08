using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    public delegate void PointerClick();
    public PointerClick pointerClick;

    public LayerMask allCastLayer;
    public LayerMask groundCastLayer;
    public Transform selectIndicator;
    public Transform cursorIndicator;
    public MeshRenderer[] renderers;

    [HideInInspector]
    public LayerMask raycastLayer;
    [HideInInspector]
    public IWorldSelectable hover;

    MeshRenderer currentRenderer;
    GameController gameController;
    Vector2 prevCoords;
    Vector3 mouseDownPos;
    IWorldSelectable selected;
    IWorldSelectable prevHover;
    float mouseDownTime;

    private void Awake() {
        gameController = GetComponent<GameController>();
        raycastLayer = allCastLayer;
    }

    private void Start() {
        SetCursorIndicatorType(0);
    }

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, raycastLayer, QueryTriggerInteraction.Collide)) {
            hover = hit.transform.GetComponent<IWorldSelectable>();
            if(hover != null) { 
                if (prevHover != hover) {
                    prevHover = hover;
                    hover.Hover();
                }
            } else {
                if(prevHover != null) {
                    prevHover.Dehover();
                    prevHover = null;
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            mouseDownTime = Time.unscaledTime;
            mouseDownPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
            if (hover != null && Time.unscaledTime - mouseDownTime < .25f && (Input.mousePosition - mouseDownPos).sqrMagnitude < 25f) {
                if (pointerClick != null)
                    pointerClick();
                else
                    Click();
            }
    }

    void Click() {
        Deselect();
        selected = hover;
        hover.Select();
    }

    public void Deselect() {
        if (selected != null) {
            selected.Deselect();
            selected = null;
        }
    }

    public void SetCursorIndicatorType(int index) {
        if (currentRenderer != renderers[index]) {
            if (currentRenderer != null)
                currentRenderer.gameObject.SetActive(false);
            currentRenderer = renderers[index];
        }
    }

    public void SetCursorIndicatorPosition(Vector2 coords, Vector2 size) {
        ShowCursorIndicator(true);
        cursorIndicator.position = Util.CoordsToVector3WithSize(coords, size) + Vector3.up * .01f;
        cursorIndicator.localScale = new Vector3(size.x, 1, size.y);
    }

    public void SetCursorIndicatorColor(Color color) {
        currentRenderer.material.color = color;
    }

    public void ShowCursorIndicator(bool show) {
        currentRenderer.gameObject.SetActive(show);
    }

    public void SetSelectIndicatorPosition(Vector2 coords, Vector2 size) {
        ShowSelectIndicator(true);
        selectIndicator.position = Util.CoordsToVector3WithSize(coords, size) + Vector3.up * .01f;
        selectIndicator.localScale = new Vector3(size.x, 1, size.y);
    }

    public void ShowSelectIndicator(bool show) {
        selectIndicator.gameObject.SetActive(show);
    }
}
