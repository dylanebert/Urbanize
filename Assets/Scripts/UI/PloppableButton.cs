using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PloppableButton : MonoBehaviour, IPointerClickHandler {

    public GameObject ploppableObj;

    UIController uiController;

    private void Awake() {
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        Ploppable ploppable = Instantiate(ploppableObj).GetComponent<Ploppable>();
        uiController.SetPloppable(ploppable);
    }
}
