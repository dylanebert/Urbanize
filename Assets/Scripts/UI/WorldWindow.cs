﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldWindow : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public float scaleMultiplier = .05f;

    protected GameController gameController;

    Transform cam;

    public virtual void Initialize(object o) {
        StartCoroutine(Show());
    }

    protected virtual void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        canvasGroup.GetComponent<Canvas>().worldCamera = gameController.uiCam;
        cam = Camera.main.transform;
    }

    protected virtual void Update() {
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        Plane plane = new Plane(cam.forward, cam.position);
        float dist = Mathf.Max(plane.GetDistanceToPoint(transform.position), .01f);
        transform.localScale = Vector3.one * Mathf.Sqrt(dist * scaleMultiplier);
    }

    public IEnumerator Show() {
        Vector3 startSize = canvasGroup.transform.localScale;
        float t = 0f;
        while(t < 1f) {
            t += Time.unscaledDeltaTime * 5f;
            float v = Mathf.Pow(t, 2);
            canvasGroup.transform.localScale = startSize * v;
            canvasGroup.alpha = v;
            yield return null;
        }
        canvasGroup.transform.localScale = startSize;
        canvasGroup.interactable = true;
    }

    public virtual void Close() {
        StartCoroutine(CloseCoroutine());
    }

    public IEnumerator CloseCoroutine() {
        canvasGroup.interactable = false;
        Vector3 startSize = canvasGroup.transform.localScale;
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * 5f;
            float v = 1f - Mathf.Pow(t, 2);
            canvasGroup.transform.localScale = startSize * v;
            canvasGroup.alpha = v;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
