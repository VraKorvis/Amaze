using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.Tilemaps;

public class SerializerMaze : ScriptableObject {
    public string jsonData = string.Empty;
    public string path = string.Empty;

    public TileBase ruleTile;

    public void MazeToJson() {
        GameData gd = new GameData();
        gd.layers = new List<Layer>();
        var grid = FindObjectOfType<GridLayout>();
        var layer = GameObject.FindGameObjectsWithTag("Layer");
        for (int i = 0; i < layer.Length; i++) {
            List<TileD> layerSer = new List<TileD>();
            Tilemap tm = layer[i].GetComponent<Tilemap>();
            BoundsInt bounds = tm.cellBounds;

            for (int n = tm.origin.x; n < bounds.xMax; n++) {
                for (int p = tm.origin.y; p < bounds.yMax; p++) {
                    Vector3Int localPlace = new Vector3Int(n, p, (int) tm.transform.position.y);
                    if (tm.HasTile(localPlace)) {
                        TileBase tb = tm.GetTile<RuleTile>(localPlace);
                        TileD td = new TileD(localPlace, tb.name);
                        layerSer.Add(td);
                    }
                }
            }

            Layer l = new Layer();
            l.tiles = layerSer.ToArray();
            gd.layers.Add(l);
            gd.layers[i].type = GetLayer(tm);
        }

        jsonData = JsonUtility.ToJson(gd);
        // path = FileRWPlatformExtension.GetFriendlyFilesPath() + "/level/01.txt";
        path = Application.dataPath + "/level/01.txt";
        if (!File.Exists(path)) {
            File.Create(path);
        }

        File.WriteAllText(path, jsonData);
    }

    private LayerType GetLayer(Tilemap tm) {
        LayerType type;
        try {
            Enum.TryParse(tm.name, out type);
        } catch (ArgumentException e) {
            var values = Enum.GetValues(typeof(LayerType));
            Console.WriteLine(
                "Layers of grid do not match the name of LayerType. Please make sure what layer name must match one of the LayerType:   " +
                values);
            Console.WriteLine(e);
            throw;
        }
        return type;
    }

    public void JsonToMaze() {
        //path = FileRWPlatformExtension.GetFriendlyFilesPath() + "/level/01";
        path = Application.dataPath + "/level/01.txt";
        GameData gd;
        if (File.Exists(path)) {
            jsonData = File.ReadAllText(path);
            gd = JsonUtility.FromJson<GameData>(jsonData);
        } else {
            return;
        }

        var grid = FindObjectOfType<GridLayout>();
        if (grid == null) {
            Instantiate<GameObject>(Resources.Load<GameObject>("GridBlank"));
        }

        for (int i = 0; i < gd.layers.Count; i++) {
            var layer = GameObject.Find(gd.layers[i].type.ToString());
            Tilemap tilemap = layer?.GetComponent<Tilemap>();
           
            if (tilemap != null) {
                tilemap.ClearAllTiles();
                
                TileD[] tiles = gd.layers[i].tiles;

                tilemap.gameObject.GetComponent<TilemapRenderer>().sortingOrder = (int) gd.layers[i].type;
                
                for (int k = 0; k < tiles.Length; k++) {
                    tilemap.SetTile(tiles[k].coord, ruleTile);
                }
                tilemap.RefreshAllTiles();
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Maze/SaveLoadMaze")]
    public static void CreateSerializerMaze() {
        string path =
            EditorUtility.SaveFilePanelInProject("Save SerializerMaze", "SerializerMaze", "asset",
                "Save SerializerMaze", "Assets");
        if (path == "") {
            return;
        }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SerializerMaze>(), path);
    }
#endif
    public void Clear() {
        Tilemap[] layer = FindObjectsOfType<Tilemap>();
        for (int i = 0; i < layer.Length; i++) {
            layer[i].ClearAllTiles();
        }
    }
}