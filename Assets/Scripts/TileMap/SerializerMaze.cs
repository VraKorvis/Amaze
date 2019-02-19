using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Tilemaps;

public class SerializerMaze : MonoBehaviour {
    public string maze;
    public GameObject mazeGO;
    public Tilemap tilemap;
    public List<Vector3> availablePlaces;

    public void MazeToJson() {
        JsonUtility.ToJson(maze);
        Debug.Log(maze);

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        availablePlaces = new List<Vector3>();

        for (int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++) {
            for (int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++) {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)tilemap.transform.position.y));
                Vector3 place = tilemap.CellToWorld(localPlace);
                if (tilemap.HasTile(localPlace)) {
                    //Tile at "place"
                    availablePlaces.Add(place);
                  //  Debug.Log(place + "     " + tilemap.GetTile(localPlace).name);
                } else {
                    //No tile at "place"
                }
            }
        }

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