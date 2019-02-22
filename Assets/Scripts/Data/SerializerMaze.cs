using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.Tilemaps;

public class SerializerMaze : ScriptableObject  {
   
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
                    Vector3Int localPlace = new Vector3Int(n, p, (int)tm.transform.position.y);
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
            
        }


        jsonData = JsonUtility.ToJson(gd);
       // path = FileRWPlatformExtension.GetFriendlyFilesPath() + "/level/01.txt";
        path = Application.dataPath + "/level/01.txt";
        if (!File.Exists(path)) {
            File.Create(path);
        }
        File.WriteAllText(path, jsonData);
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
        Tilemap[] layer = FindObjectsOfType<Tilemap>();
        for (int i = 0; i < layer.Length; i++) {
            layer[i].ClearAllTiles();
            TileD[] td = gd.layers[i].tiles;
          //  TileBase tb = Resources.Load<>()
            for (int k = 0; k < td.Length; k++) {
                layer[i].SetTile(td[k].coord, ruleTile);
            }
            layer[i].RefreshAllTiles();
        }
    }

    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile) {
        //Clear the map (ensures we dont overlap)
        tilemap.ClearAllTiles();
        //Loop through the width of the map
        for (int x = 0; x < map.GetUpperBound(0); x++) {
            //Loop through the height of the map
            for (int y = 0; y < map.GetUpperBound(1); y++) {
                // 1 = tile, 0 = no tile
                if (map[x, y] == 1) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Maze/SaveLoadMaze")]
    public static void CreateSerializerMaze() {
        string path =
            EditorUtility.SaveFilePanelInProject("Save SerializerMaze", "SerializerMaze", "asset", "Save SerializerMaze", "Assets");
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