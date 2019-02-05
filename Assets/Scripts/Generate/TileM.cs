using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Sides {
    Bottom,
    Right,
    Left,
    Top
}

public class TileM {
    public int id = 0;
    public TileM[] neighbors = new TileM[4];
    public TileM[] neighborsThroughCell = new TileM[4];
    public int autotileID;   // 1010
    public bool isVisited = false;
    public TileTypeMaze Type { get; set; }

    public GameObject Tile { get; set; }

    public TileM[] CopyNeighbors {
        get { return neighborsThroughCell.ToArray(); }
    }

    public void AddNeighbor(Sides side, TileM tile) {
        neighbors[(int) side] = tile;
        CalculateAutotileID();
    }

    public void AddNeighborThroughCell(Sides side, TileM tile) {
        neighborsThroughCell[(int)side] = tile;
    }

    public void RemoveNeighbor(TileM tile) {
        var total = neighbors.Length;
        for (int i = 0; i < total; i++) {
            if (neighbors[i] != null) {
                if (neighbors[i].id == tile.id) {
                    neighbors[i] = null;
                }
            }
        }
        CalculateAutotileID();
    }

    public void ClearNeighbors() {
        var total = neighbors.Length;
        for (int i = 0; i < total; i++) {
            var tile = neighbors[i];
            if (tile != null) {
                tile.RemoveNeighbor(this);
                neighbors[i] = null;
            }
        }
        CalculateAutotileID();
    }

    private void CalculateAutotileID() {
        var sideValues = new StringBuilder();
        foreach (TileM tile in neighbors) {
            sideValues.Append(tile == null ? "0" : "1");
        }
        autotileID = Convert.ToInt32(sideValues.ToString(), 2);
    }
}