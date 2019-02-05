using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileType {
    Empty = -1,
    Grass = 15,
    Tree = 16,
    Hills = 17,
    Mountains = 18,
    Towns = 19,
    Castle = 20,
    Monster = 21
}

public class MapM : AbstractMap {
    public TileM[] CoastTiles {
        get { return tiles.Where(t => t.autotileID < (int) TileType.Grass).ToArray(); }
    }
    public TileM[] LandTiles {
        get { return tiles.Where(t => t.autotileID == (int) TileType.Grass).ToArray(); }
    }
    public TileM CastleTile {
        get { return tiles.FirstOrDefault(t => t.autotileID == (int) TileType.Castle); }
    }
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
    }
    protected override void FindNeighbors() {
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < columns; c++) {
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
    public void CreateIsland(
        float erodePercent,
        int erodeIterations,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPErcent,
        float lakePercent) {

        DecorateTIles(LandTiles, lakePercent, TileType.Empty);

        for (int i = 0; i < erodeIterations; i++) {
            DecorateTIles(CoastTiles, erodePercent, TileType.Empty);
        }

        var openTiles = LandTiles;
        RandomiseTileArray(openTiles);
        openTiles[0].autotileID = (int) TileType.Castle;

        DecorateTIles(LandTiles, treePercent, TileType.Tree);
        DecorateTIles(LandTiles, hillPercent, TileType.Hills);
        DecorateTIles(LandTiles, mountainPercent, TileType.Mountains);
        DecorateTIles(LandTiles, townPercent, TileType.Towns);
        DecorateTIles(LandTiles, monsterPErcent, TileType.Monster);
    }
    public void DecorateTIles(TileM[] tiles, float percent, TileType type) {
        var total = Mathf.FloorToInt(tiles.Length * percent);
        RandomiseTileArray(tiles);
        for (int i = 0; i < total; i++) {
            var tile = tiles[i];
            if (type == TileType.Empty) {
                tile.ClearNeighbors();
            }
            tile.autotileID = (int) type;
        }
    }

    /// <summary>
    /// Fisher-Yates shuffle
    /// </summary>
    /// <param name="tiles">tiles for shuffle</param>
    public void RandomiseTileArray(TileM[] tiles) {
        for (int i = 0; i < tiles.Length; i++) {
            var r = Random.Range(i, tiles.Length);
            var tmp = tiles[i];
            tiles[i] = tiles[r];
            tiles[r] = tmp;
        }
    }
}