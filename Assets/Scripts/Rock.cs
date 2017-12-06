using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    private void Start() {
        GetComponentInChildren<MeshRenderer>().transform.Rotate(Vector3.up, Random.Range(0, 360));
    }
}
