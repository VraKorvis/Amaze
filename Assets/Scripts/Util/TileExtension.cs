using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public static class TileExtension  {

    public static TileBase HasObstacleTile(this Tilemap tilemap, Vector2 targetTile) {
        return tilemap.GetTile(tilemap.WorldToCell(targetTile));
    }
}
