using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRender : MonoBehaviour {

    public DungeonGen dungeonGen;
    public GameObject til;
    public Transform container;

    private Image[,] tiles;


    //dict to remember old maps
    private Dictionary<DungeonGen, Image[,]> mapCache = new Dictionary<DungeonGen, Image[,]>();


    void Start() {
        if (dungeonGen == null)
            dungeonGen = FindObjectOfType<DungeonGen>();

        StartCoroutine(dungeonWait());
    }

    IEnumerator dungeonWait() {
        while (dungeonGen == null || dungeonGen.dungeon == null || !dungeonGen.ready) {
            yield return null;
        }

        buildMap();
    }


    public void buildMap(){

        if (dungeonGen == null || dungeonGen.dungeon == null || !dungeonGen.ready)
            return;


        //if map exists, use that instead
        if (mapCache.ContainsKey(dungeonGen)) {
            tiles = mapCache[dungeonGen];
            return;
        }


        DungeonData dungeon = dungeonGen.dungeon;

        Image[,] tilesNew = new Image[dungeon.width, dungeon.height];

        for (int x = 0; x < dungeon.width; x++) {
            for (int y = 0; y < dungeon.height; y++) {

                GameObject obj = Instantiate(til, container);
                Image img = obj.GetComponent<Image>();

                tilesNew[x, y] = img;
            }
        }

        mapCache[dungeonGen] = tilesNew;
        tiles = tilesNew;
    }


    public void updateMap(Vector2Int playerPos) {

        if (tiles == null || !mapCache.ContainsKey(dungeonGen))
            return;

        tiles = mapCache[dungeonGen];

        DungeonData dungeon = dungeonGen.dungeon;

        for (int x = 0; x < dungeon.width; x++) {
            for (int y = 0; y < dungeon.height; y++) {

                Image img = tiles[x, y];

                if (!dungeon.explored[x, y]) {
                    img.color = Color.black; //unexplored = black
                } else if (playerPos.x == x && playerPos.y == y) {
                    img.color = Color.green; //player pos = green
                } else if (dungeon.tiles[x, y] == TileType.Floor) {
                    img.color = Color.white; //floors white
                } else {
                    img.color = Color.gray; //wall
                }
            }
        }
    }
}


