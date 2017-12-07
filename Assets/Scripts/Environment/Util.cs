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

    public static Vector3 CoordsToVector3(Vector2 coords) {
        return new Vector3(coords.x + .5f, 0, coords.y + .5f);
    }

    public static Vector3 CoordsToVector3WithSize(Vector2 coords, Vector2 size) {
        return new Vector3(coords.x + size.x / 2f, 0, coords.y + size.y / 2f);
    }
}
