using UnityEngine;
using System.Collections;

public abstract class AbstractMap {
    public TileM[] tiles;
    public int columns;
    public int rows;

    public abstract void NewMap(int columns, int rows);

    protected abstract void CreateTiles();

    protected abstract void FindNeighbors();
}