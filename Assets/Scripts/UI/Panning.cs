using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Panning : MonoBehaviour, IDragHandler {

    public OrbitCamera cam;

    public void OnDrag(PointerEventData eventData) {
        if(Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftAlt))
            cam.Pan(eventData.delta);
    }
}
