using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public enum TileTypeMaze {
    Empty = -1,
    Wall = 12,
    Ground = 21,
    OutWall = 16,
    StartPoint = 26,
    EndPoint = 26
}

public class Maze : AbstractMap {
    public TileM[] GroundTiles {
        get { return tiles.Where(t => t.Type == TileTypeMaze.Ground).ToArray(); }
    }

    public TileM StartTile {
        get { return tiles.FirstOrDefault(t => t.Type == TileTypeMaze.StartPoint); }
    }

    public TileM[] WallTiles {
        get { return tiles.Where(t => t.Type == TileTypeMaze.Wall).ToArray(); }
    }

    private List<List<TileM>> islandTiles = new List<List<TileM>>();

    public List<List<TileM>> IslandTiles {
        get { return islandTiles; }
        set { islandTiles = value; }
    }

    #region Create_Maze

    public override void NewMap(int columns, int rows) {
        this.columns = columns;
        this.rows = rows;
        tiles = new TileM[columns * rows];
        CreateTiles();
    }

    protected override void CreateTiles() {
        var total = tiles.Length;

        for (int i = 0; i < total; i++) {
            var tile = new TileM();
            tile.id = i;
            tiles[i] = tile;
        }

        FindNeighbors();
        FindNeighborsThroughCell();
    }

    protected override void FindNeighbors() {
        for (int r = 0; r < rows - 1; r++) {
            for (int c = 0; c < columns - 1; c++) {
                var tile = tiles[columns * r + c];
                if (r < rows - 1) {
                    tile.AddNeighbor(Sides.Bottom, tiles[columns * (r + 1) + c]);
                }

                if (c < columns - 1) {
                    tile.AddNeighbor(Sides.Right, tiles[columns * r + (c + 1)]);
                }

                if (c > 0) {
                    tile.AddNeighbor(Sides.Left, tiles[columns * r + (c - 1)]);
                }

                if (r > 0) {
                    tile.AddNeighbor(Sides.Top, tiles[columns * (r - 1) + c]);
                }
            }
        }
    }

    private void FindNeighborsThroughCell() {
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < columns; c++) {
                var tile = tiles[columns * r + c];
                if (r < rows - 2) {
                    tile.AddNeighborThroughCell(Sides.Bottom, tiles[columns * (r + 2) + c]);
                }

                if (c < columns - 2) {
                    tile.AddNeighborThroughCell(Sides.Right, tiles[columns * r + (c + 2)]);
                }

                if (c > 1) {
                    tile.AddNeighborThroughCell(Sides.Left, tiles[columns * r + (c - 2)]);
                }

                if (r > 1) {
                    tile.AddNeighborThroughCell(Sides.Top, tiles[columns * (r - 2) + c]);
                }
            }
        }
    }

    #endregion

    public void RandomiseTileArray(TileM[] tiles) {
        for (int i = 0; i < tiles.Length; i++) {
            var r = Random.Range(i, tiles.Length);
            var tmp = tiles[i];
            tiles[i] = tiles[r];
            tiles[r] = tmp;
        }
    }
}