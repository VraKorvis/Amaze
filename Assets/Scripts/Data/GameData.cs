using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class GameData {
    public List<Layer> layers;
}

[Serializable]
public class Layer {
    public TileD[] tiles;
}

[Serializable]
public class TileD {
    public Vector3Int coord;
    public string nameRuleTile;

    public TileD(Vector3Int coord, string nameRuleTile) {
        this.coord = coord;
        this.nameRuleTile = nameRuleTile;
    }
}

public enum LayerType {
    Ground =0,
    Wall = 5,
    Trap,
    Shadow = 10

}
