using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {

    public static float DayNightRatio = 2f / 1f;
    public static float HourLength = 60f; //Seconds

    public Gradient dayNightColor;
    public float minPoint = -.3f;

    [HideInInspector]
    public TimeSpan worldTime;

    Light mainLight;
    float dayRotateSpeed;
    float nightRotateSpeed;
    float rotation;

    private void Awake() {
        mainLight = GetComponent<Light>();
        dayRotateSpeed = 15 * 1f / (1 + minPoint) / ((DayNightRatio + 1) / DayNightRatio) / HourLength;
        nightRotateSpeed = 15 * 1f / (1 - minPoint) * (DayNightRatio + 1) / DayNightRatio / HourLength;
        rotation = transform.rotation.eulerAngles.x + 90f;
    }

    private void Update() {
        float tRange = 1f - minPoint;
        float dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);

        mainLight.color = dayNightColor.Evaluate(dot);
        RenderSettings.ambientLight = mainLight.color;

        if (dot > 0) {
            float d = dayRotateSpeed * Time.deltaTime;
            transform.Rotate(Vector3.right * d);
            rotation += d;
        }
        else {
            float d = nightRotateSpeed * Time.deltaTime;
            transform.Rotate(Vector3.right * d);
            rotation += d;
        }

        UpdateWorldTime();
    }

    void UpdateWorldTime() {
        if(rotation > 360f)
            rotation %= 360f;
        worldTime = TimeSpan.FromHours(rotation / 15f);
    }
}
