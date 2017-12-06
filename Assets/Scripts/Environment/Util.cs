using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {

    public static Vector2 GroundVector2(Vector3 v) {
        return new Vector2(v.x, v.z);
    }	

    public static Vector3 GroundVector3(Vector3 v) {
        return new Vector3(v.x, 0, v.z);
    }
}
