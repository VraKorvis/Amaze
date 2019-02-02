using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfectCamera : MonoBehaviour {

    public static float pixelToUnits = 1f;
    public static float scale = 1f;

    public Vector2 nativeResolution = new Vector2(160, 144);

    void Awake() {
        var camera = GetComponent<Camera>();

        if (camera.orthographic) {
            var height = Screen.height;
           // Debug.Log("Screen.height  " + height);
            var res = nativeResolution.y;
            scale = height / res;
            pixelToUnits *= scale;
           // Debug.Log("pixelToUnits  " + pixelToUnits);
            
            camera.orthographicSize = (height / 2.0f) / pixelToUnits;
           // Debug.Log("height / 2.0f  " + height / 2.0f + " / pixelToUnits  " + pixelToUnits);

        }
    }
}
