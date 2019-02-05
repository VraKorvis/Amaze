﻿using UnityEngine;

public class PixelPerfectCamera : MonoBehaviour {
    public static float pixelToUnits = 1f;
    public static float scale = 1f;

    public Vector2 nativeResolution = new Vector2(160, 144);

    void Awake() {
        var camera = GetComponent<Camera>();
        nativeResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        if (camera.orthographic) {
            var height = Screen.height;
            // Debug.Log("Screen.height  " + height);
            var res = nativeResolution.y;
            scale = height / res;
            pixelToUnits *= scale;
            // Debug.Log("pixelToUnits  " + pixelToUnits);

            camera.orthographicSize = ((height / 2.0f) / pixelToUnits);
            // Debug.Log("height / 2.0f  " + height / 2.0f + " / pixelToUnits  " + pixelToUnits);

        }
    }

    public void CorrectCamera(int columns) {
        pixelToUnits = 1f;
        scale = 1f;
        var camera = GetComponent<Camera>();
        nativeResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        if (camera.orthographic) {
            var height = Screen.height;
            var res = nativeResolution.y;
            scale = height / res;
            pixelToUnits *= scale;
            var index = 50f / columns;
            camera.orthographicSize = ((height / 2.0f) / pixelToUnits /index );
        }
    }
}