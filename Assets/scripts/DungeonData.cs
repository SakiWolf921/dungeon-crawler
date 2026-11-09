using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData {
    public int width;
    public int height;
    public TileType[,] tiles;
    public bool[,] explored;

    public DungeonData(int w, int h) {
        width = w;
        height = h;
        tiles = new TileType[w, h];
        explored = new bool[w, h];
    }
}
