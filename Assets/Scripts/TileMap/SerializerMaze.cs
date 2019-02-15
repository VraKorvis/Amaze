using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Tilemaps;

public class SerializerMaze : MonoBehaviour {

    public string maze;
    public GameObject mazeGO;

    public void MazeToJson() {
        JsonUtility.ToJson(maze);
        Debug.Log(maze);
    }

    public void JsonToMaze() {
        mazeGO = JsonUtility.FromJson<GameObject>(maze);
        Instantiate(mazeGO);
    }

//#if UNITY_EDITOR
//    [MenuItem("Assets/Create/Maze/SaveLoadMaze")]
//    public static void CreateSerializerMaze() {
//        string path =
//            EditorUtility.SaveFilePanelInProject("Save SerializerMaze", "SerializerMaze", "asset", "Save SerializerMaze", "Assets");
//        if (path == "") {
//            return;
//        }

//        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SerializerMaze>(), path);
//    }
//#endif
}