using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public delegate void ChangeSizeCameraEvent(int columns);

public delegate void CreateMazeEvent();

public class RandomMazeGenerator : MonoBehaviour {
    [SerializeField] [Header("PerfectPixelCamera")]
    private PixelPerfectCamera camera;

    [Header("Maze Sizes")] public int mazeWidth;
    public int mazeHeight;

    public Maze maze;
    private Sprite[] sprites;
    [Space] [Header("View Maze")] public GameObject mazeContainer;
    public GameObject tilePrefab;
    public Vector2 tileSize = new Vector2(16, 16);

    [Space] [Header("Map Sprite Texture")] public Texture2D mazeTexure;
    [Space] [Header("Count Loop")] public int countLoop;

    public event ChangeSizeCameraEvent OnChangeCameraSize;
    public event CreateMazeEvent OnCreateMaze;

    [SerializeField] [Header("Delay draw one moving")]
    private float delay = 1.5f;

    [SerializeField] private Slider slide;

    void Start() {
        maze = new Maze();
        sprites = Resources.LoadAll<Sprite>(mazeTexure.name);
        OnChangeCameraSize += camera.CorrectCamera;
    }

    public void MakeMaze() {
        StopAllCoroutines();
        ClearMap();
        if (OnChangeCameraSize != null) {
            OnChangeCameraSize(mazeWidth);
        }

        maze.NewMap(mazeWidth, mazeHeight);
        CreateGrid();
        //CenterMapCamera(mazeWidth * mazeHeight /2);
    }

    private void CreateLoop() {
        var wallTiles = maze.WallTiles;
        maze.RandomiseTileArray(wallTiles);
        for (int i = 0; i < countLoop; i++) {
            if (IsCorner(wallTiles[i])) continue;
            DrawTile(wallTiles[i], TileTypeMaze.Ground, true);
        }
    }

    private bool IsCorner(TileM tile) {
        var neighbors = tile.neighbors;
        if ((neighbors[(int) Sides.Bottom].Type == TileTypeMaze.Wall ||
             neighbors[(int) Sides.Top].Type == TileTypeMaze.Wall) &&
            (neighbors[(int) Sides.Right].Type == TileTypeMaze.Wall ||
             neighbors[(int) Sides.Left].Type == TileTypeMaze.Wall)) {
            return true;
        }
        return false;
    }

    private void AddStartEndPoint() {
        var grounds = maze.GroundTiles;
        maze.RandomiseTileArray(grounds);
        TileM start = grounds[0];
        var sr = start.Tile.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[(int) TileTypeMaze.StartPoint];
        start.Type = TileTypeMaze.StartPoint;
        start.Tile.GetComponent<BoxCollider2D>().enabled = false;
        TileM end = grounds[1];
        sr = end.Tile.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[(int) TileTypeMaze.EndPoint];
        end.Type = TileTypeMaze.EndPoint;
        end.Tile.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void CreateGrid() {
        var total = maze.tiles.Length;
        var maxColumns = maze.columns;
        var column = 0;
        var row = 0;

        for (int i = 0; i < total; i++) {
            column = i % maxColumns;
            var newX = column * tileSize.x;
            var newY = -(row * tileSize.y);

            var go = Instantiate(tilePrefab);
            maze.tiles[i].Tile = go;
            TileM tile = maze.tiles[i];
            tile.Tile.name = "Tile " + i;
            tile.Tile.transform.SetParent(mazeContainer.transform);
            tile.Tile.transform.position = new Vector3(newX, newY, 0);
            var bc = go.AddComponent<BoxCollider2D>();
            bc.size = tileSize;

            var spriteID = tile.autotileID;
            if (!tile.isVisited) {
                if (column == 0 || row == 0 || column == (maxColumns - 1) || row == (maxColumns - 1)) {
                    DrawTile(tile, TileTypeMaze.OutWall, true);
                } else {
                    DrawTile(tile, TileTypeMaze.Wall, false);
                }
            }

            if (column == (maxColumns - 1)) {
                row++;
            }
        }

        StartCoroutine(GenerateMaze());
    }

    private IEnumerator GenerateMaze() {
        TileM[] tiles = maze.tiles;
        Stack<TileM> stack = new Stack<TileM>();
        TileM focusTile = tiles[InitFirstCell()];
        DrawTile(focusTile, TileTypeMaze.Ground, true);
        while (true) {
            bool canVisited;
            do {
                yield return new WaitForSeconds(slide.value);
                canVisited = false;
                TileM[] shuffleNeighbors = focusTile.neighborsThroughCell;
                maze.RandomiseTileArray(shuffleNeighbors);
                for (int i = 0; i < shuffleNeighbors.Length; i++) {
                    TileM neighborThCell = shuffleNeighbors[i];
                    if (neighborThCell != null && !neighborThCell.isVisited && !IsWall(neighborThCell.id)) {
                        canVisited = true;
                        stack.Push(neighborThCell);
                        int index = DirectionDetermination(focusTile, neighborThCell);
                        DrawTile(focusTile.neighbors[index], TileTypeMaze.Ground, true);
                        focusTile = neighborThCell;
                        DrawTile(focusTile, TileTypeMaze.Ground, true);
                        break;
                    }
                }
            } while (canVisited);

            DrawTile(focusTile, TileTypeMaze.Ground, true);
            if (stack.Count != 0) {
                focusTile = stack.Pop();
            } else {
                break;
            }
        }

        yield return null;
        CreateLoop();
        AddStartEndPoint();
        FindIsland();
        if (OnCreateMaze != null) {
            OnCreateMaze();
        }
    }

    private void FindIsland() {

        var tiles = maze.WallTiles;
        maze.IslandTiles = new List<List<TileM>>();
        var list = maze.IslandTiles;
        var newIsland = new List<TileM>();

        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i].IsBorderingOuterWall) continue;
            //if (tiles[i].IsIsland) continue;

            Stack<TileM> stack = new Stack<TileM>();
            stack.Push(tiles[i]);
            newIsland = new List<TileM>();
            do {
                TileM focus = stack.Pop();
                focus.IsIsland = true;
                newIsland.Add(focus);

                var neighbors = focus.neighbors;
                for (int j = 0; j < neighbors.Length; j++) {
                    if (neighbors[j].Type == TileTypeMaze.OutWall || neighbors[j].IsBorderingOuterWall) {
                        focus.IsBorderingOuterWall = true;
                        foreach (var t in newIsland) {
                            t.IsBorderingOuterWall = true;
                        }
                        stack.Clear();
                        newIsland.Clear();
                        break;
                    }

                    if (neighbors[j].Type == TileTypeMaze.Wall) {
                        if (!neighbors[j].IsIsland) {
                            if (!neighbors[j].IsBorderingOuterWall) {
                                stack.Push(neighbors[j]);
                            }
                        }
                        
                    }
                }
            } while (stack.Count > 0);

            if (newIsland.Count > 0) {
                list.Add(newIsland);
            }
        }

        HighLightIslands();
    }

    private void HighLightIslands() {
        foreach (var island in maze.IslandTiles) {
            foreach (var tile in island) {
                DrawTile(tile, TileTypeMaze.OutWall, true);
            }
        }
    }

    private int DirectionDetermination(TileM focusTile, TileM tile) {
        if (focusTile.id / maze.rows < tile.id / maze.rows) return (int) Direction.Bot;
        if (focusTile.id % maze.rows < tile.id % maze.columns) return (int) Direction.Right;
        if (focusTile.id % maze.rows > tile.id % maze.columns) return (int) Direction.Left;
        if (focusTile.id / maze.rows > tile.id / maze.rows) return (int) Direction.Top;
        return 0;
    }

    private void DrawTile(TileM tile, TileTypeMaze type, bool markVisited) {
        tile.isVisited = markVisited;
        tile.Type = type;
        var sr = tile.Tile.GetComponent<SpriteRenderer>();
        sr.sprite = type == TileTypeMaze.Ground ? null : sprites[(int) type];
        var bc = tile.Tile.GetComponent<BoxCollider2D>();
        bc.enabled = type == TileTypeMaze.Ground ? false : true;
        tile.CalculateAutotileID();
    }

    private int InitFirstCell() {
        while (true) {
            int index = Random.Range(0, maze.columns * maze.rows);
            if (!IsWall(index) && !maze.tiles[index].isVisited) return index;
        }
    }

    private bool IsWall(int id) {
        if ((id / maze.columns == 0) || (id % maze.columns == maze.columns - 1) || (id % maze.rows == 0) ||
            (id / maze.rows == maze.rows - 1)) {
            return true;
        }

        return false;
    }

    private void ClearMap() {
        var tilesOnScene = mazeContainer.transform.GetComponentsInChildren<Transform>();
        for (int i = tilesOnScene.Length - 1; i > 0; i--) {
            Destroy(tilesOnScene[i].gameObject);
        }
    }

    public void CenterMapCamera(int index) {
        var camPos = Camera.main.transform.position;
        var width = maze.columns;
        camPos.x = (index % width) * tileSize.x;
        camPos.y = -((index / width) * tileSize.y);
        Camera.main.transform.position = camPos;
    }

    private enum Direction {
        Bot,
        Right,
        Left,
        Top
    }
}