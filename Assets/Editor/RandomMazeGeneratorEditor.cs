using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomMazeGenerator))]
public class RandomMazeGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var script = (RandomMazeGenerator) target;

        if (GUILayout.Button("Generate Maze(Recurse Backtracking)")) {
            if (Application.isPlaying) {
                script.MakeMaze();
            }
        }
    }
}
