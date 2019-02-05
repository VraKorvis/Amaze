using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour {

    [Header("Map Dimensions")]
    public int mapWidth = 20;
    public int mapHeight = 20;

    public MapM map;

    [Space] [Header("View Map")] public GameObject mapContainer;
    public GameObject tilePrefab;
    public Vector2 tileSize = new Vector2(16, 16);

    [Space] [Header("Map Sprites")] public Texture2D mazeTexure;

    [Space] [Header("Decorate Map")] [Range(0, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;
    [Range(0, 0.9f)]
    public float treePercent = 0.3f;
    [Range(0, 0.9f)]
    public float hillsPercent = 0.2f;
    [Range(0, 0.9f)]
    public float mountainsPercent = 0.1f;
    [Range(0, 0.9f)]
    public float townPercent = 0.05f;
    [Range(0, 0.9f)]
    public float monsterPercent = 0.1f;
    [Range(0, 0.9f)]
    public float lakePercent = 0.05f;

    // Use this for initialization
    void Start () {
		map = new MapM();
    }

    public void MakeMap() {
        ClearMap();
        map.NewMap(mapWidth, mapHeight);
        map.CreateIsland(erodePercent, erodeIterations, treePercent, hillsPercent, mountainsPercent, townPercent, monsterPercent, lakePercent);
        CreateGrid();
        CenterMapCamera(map.CastleTile.id);
    }

    private void ClearMap() {
        var tilesOnScene = mapContainer.transform.GetComponentsInChildren<Transform>();
        for (int i = tilesOnScene.Length-1; i>0; i--) {
            Destroy(tilesOnScene[i].gameObject);
        }
    }

    void CreateGrid() {
        Sprite[] sprites = Resources.LoadAll<Sprite>(mazeTexure.name);
        var total = map.tiles.Length;  
        var maxColumns = map.columns; 
        var column = 0;
        var row = 0;

        for (int i = 0; i < total; i++) {
            column = i % maxColumns;
            var newX = column * tileSize.x;
            var newY = -(row * tileSize.y);

            var go = Instantiate(tilePrefab);
            go.name = "Tile " + i;
            go.transform.SetParent(mapContainer.transform);
            go.transform.position = new Vector3(newX, newY, 0);

            TileM tile = map.tiles[i];
            var spriteID = tile.autotileID;

            if (spriteID >= 0) {
                var spriteRenderer = go.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[spriteID];
            }

            if (column == (maxColumns-1)) {
                row++;
            }
        }
    }

    public void CenterMapCamera(int index) {
        var camPos = Camera.main.transform.position;
        var width = map.columns;
        camPos.x = (index % width) * tileSize.x;
        camPos.y = -((index / width) * tileSize.y);
        Camera.main.transform.position = camPos;
    }

}
