using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomMapGenerator))]
public class RandomMapGeneratorEditor : Editor {
 
    public override void OnInspectorGUI() {
       //DrawDefaultInspector();
        base.OnInspectorGUI();

        var script = (RandomMapGenerator) target;

        if (GUILayout.Button("Generate Maze")) {
            if (Application.isPlaying) {
                script.MakeMap();
            }
        }

    }
}
