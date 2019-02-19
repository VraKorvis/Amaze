using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class MazeMap : MonoBehaviour{
    private Tilemap map;

    void Start() {
        Tilemap tilemap = GetComponent<Tilemap>();
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
        tilemap.SetTransformMatrix(new Vector3Int(0, 0, 0), matrix);
    }

}
