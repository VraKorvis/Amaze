using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SerializerMaze))]
public class SerializerMazeEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var script = (SerializerMaze)target;

        if (GUILayout.Button("Json to maze")) {
            script.JsonToMaze();
            //if (Application.isPlaying) {
            //    script.JsonToMaze();
            //}
        }

        if (GUILayout.Button("Maze to json")) {
            script.MazeToJson();
            //if (Application.isPlaying) {
            //    script.MazeToJson();
            //}
        }
    }

}
