using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.IO;

public class DungeonGen : MonoBehaviour {
/*
controls dungeon making
so uhh
floor n wall making
rooms n corridors
spawning things
*/


    [SerializeField] private int w = 50;
    [SerializeField] private int h = 50;

    public int rooms;
    [SerializeField] private int minRoom = 4;
    [SerializeField] private int maxRoom = 8;
    private List<RectInt> roomlist = new List<RectInt>();

    public GameObject floor;
    public GameObject wall;

    public DungeonData dungeon;

    public GameObject exit;
    public GameObject stairsUp;
    public Vector2Int exitPos;
    public Vector2Int stairsPos;

    public int level = 1; //CHANGE BACK TO 1 LATER//
    public TMP_Text lvl;

    public bool ready = false;
    //public bool awakeGen = true;
    public static bool blockGen = false; //var to track if dungeon should be generated (should not be for the boss)


    //spawning things
    public Vector2Int playerSpawn;
    private RectInt playerRoom;
    public GameObject chest;
    public GameObject[] enemy;

    private int numChests = 5;
    private int numEnemies = 5;

    //items
    private Dictionary<Vector2Int, ItemPickup> itemMap = new Dictionary<Vector2Int, ItemPickup>();

    public List<ItemData> itemPool;
    public GameObject itemPickupPrefab;
    public int minItems = 3;
    public int maxItems = 8;

    //store where stuff already is
    private List<Vector2Int> occupiedTiles = new List<Vector2Int>();
    public static Stack<DungeonData> floorHistory = new Stack<DungeonData>();
    public static Stack<Vector2Int> playerPositions = new Stack<Vector2Int>();

    void Awake() {

        //block generation if entering boss scene
        if (blockGen) {
            blockGen = false;
            return;
        }

        /*
        if (!awakeGen)
            return;
        */

        Debug.Log("dungeongen awake: " + gameObject.name + " - scene " + SceneManager.GetActiveScene().buildIndex);

        //if loading a save - make dungeon from data
        if (GameState.loadGame) {
            GameState.loadGame = false;
            loadGame(FindObjectOfType<PlayerControl>());

            return;
        }

        //normal generation//
        rooms = 8;

        genDungeon();
        genCorridors();
        buildWorld();
        spawnObjs();

        if (lvl == null)
            lvl = FindObjectOfType<TMP_Text>();

        lvl.text = "floor: " + level.ToString();

        ready = true; //dungeon built yay!
    }


    void Start() {
        //ignores making dungeon for boss (lvl 5)
        if (level == 5) {
            //rooms = 2;
            //Debug.Log("aaaaa");

            SceneManager.LoadScene(2, LoadSceneMode.Single);
                return;

        } /*else {
            Debug.Log("bbbbb");
        }*/
    }




    /* make the room cutouts */
    void genDungeon() {
        dungeon = new DungeonData(w, h);


        //wall time - just make everything walls first
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                dungeon.tiles[x, y] = TileType.Wall; //make wall
            }
        }

        //make random rect rooms - cut through walls
        for (int i = 0; i < rooms; i++) {
            int rw = Random.Range(minRoom, maxRoom);
            int rh = Random.Range(minRoom, maxRoom);
            int rx = Random.Range(1, w - rw - 1);
            int ry = Random.Range(1, h - rh - 1);

            //add to our roomlist
            RectInt room = new RectInt(rx, ry, rw, rh);
            roomlist.Add(room);

            for (int x = rx; x < rx + rw; x++){
                for (int y = ry; y < ry + rh; y++){
                    dungeon.tiles[x, y] = TileType.Floor;
                }
            }
        }
    }


    /* codde for corridors */

    //generate corridors
    void genCorridors() {

        //loop thru list
        for (int i = 1; i < roomlist.Count; i++){

            Vector2Int a = roomCentre(roomlist[i - 1]);
            Vector2Int b = roomCentre(roomlist[i]);

            makeCorridor(a, b);
        }
    }

    //find centre
    Vector2Int roomCentre(RectInt room) {
        return new Vector2Int(
            room.x + room.width / 2,
            room.y + room.height / 2
        );
    }


    void makeCorridor(Vector2Int a, Vector2Int b) {
        Vector2Int pos = a;

        while (pos.x != b.x) {
            dungeon.tiles[pos.x, pos.y] = TileType.Floor;
            pos.x += pos.x < b.x ? 1 : -1;
        }

        while (pos.y != b.y) {
            dungeon.tiles[pos.x, pos.y] = TileType.Floor;
            pos.y += pos.y < b.y ? 1 : -1;
        }
    }



    void buildWorld() {
        for (int x = 0; x < dungeon.width; x++) {
            for (int y = 0; y < dungeon.height; y++) {

                Vector3 pos = new Vector3(x, 0.01f, y);

                if (dungeon.tiles[x, y] == TileType.Floor){
                    GameObject f = Instantiate(floor, pos, default, transform);
                    f.transform.localEulerAngles = Vector3.zero;
                } else {
                    Vector3 wallPos = new Vector3(x, 0.5f, y);
                    GameObject w = Instantiate(wall, wallPos, default, transform);
                    w.transform.localEulerAngles = Vector3.zero;
                }
            }
        }
    }


    //TO SCRAP?? dont think its in use anymore//
    public Vector2Int randomRoomTile() {
        if (roomlist.Count == 0)
            return new Vector2Int(1, 1); //incase it bugs out

        //pick random room
        RectInt room = roomlist[Random.Range(0, roomlist.Count)];

        //random floor tile in room
        int x = Random.Range(room.x, room.x + room.width);
        int y = Random.Range(room.y, room.y + room.height);

        return new Vector2Int(x, y);
    }


    public RectInt randRoom() {
        RectInt room;
        do {
            room = roomlist[Random.Range(0, roomlist.Count)];
        } while (room.Equals(playerRoom));

        return room;
    }

    public Vector2Int randTile(RectInt room) {
        int x = Random.Range(room.x, room.x + room.width);
        int y = Random.Range(room.y, room.y + room.height);
        return new Vector2Int(x, y);
    }


    //spawning other stuff
    void spawnObjs() {
        playerRoom = roomlist[Random.Range(0, roomlist.Count)];

        //tile from that room
        int x = Random.Range(playerRoom.x, playerRoom.x + playerRoom.width);
        int y = Random.Range(playerRoom.y, playerRoom.y + playerRoom.height);

        playerSpawn = new Vector2Int(x, y);
        occupiedTiles.Add(playerSpawn);

        spawnUp();

        for (int i = 0; i < numChests; i++) {
            spawnTile(chest);
        }

        for (int i = 0; i < (numEnemies+level); i++) {
            GameObject randEnemy = enemy[Random.Range(0, enemy.Length)];
            spawnTile(randEnemy);
        }

        spawnExit();

        spawnItems();
    }


    void spawnTile(GameObject obj) {
        RectInt room = randRoom();
        Vector2Int tile = randTile(room);

        //check its valid
        bool isFloor = dungeon.tiles[tile.x, tile.y] == TileType.Floor;
        //bool inPlayerRoom = playerRoom.Contains(tile);

        if (!occupiedTiles.Contains(tile) && isFloor) {
            occupiedTiles.Add(tile);
            Vector3 spawnPos = new Vector3(tile.x, 0, tile.y);

            GameObject newObj = Instantiate(obj, spawnPos, default, transform);
            newObj.transform.localEulerAngles = Vector3.zero;

            return;
        }

    }


    void spawnItems() {
        //note some things arent spawnign
        //might make it iterate? idk

        if (itemPool == null || itemPool.Count == 0) {
            return;
        }

        int itemCount = Random.Range(minItems, maxItems + 1);

        for (int i = 0; i < itemCount; i++) {
            RectInt room = randRoom();
            Vector2Int tile = randTile(room);

            bool isFloor = dungeon.tiles[tile.x, tile.y] == TileType.Floor;

            if (!occupiedTiles.Contains(tile) && isFloor) {
                occupiedTiles.Add(tile);

                Vector3 pos = new Vector3(tile.x, 0, tile.y);

                ItemData item = itemPool[Random.Range(0, itemPool.Count)];

                GameObject obj = Instantiate(item.worldPrefab, pos, default, transform);

                ItemPickup pickup = obj.GetComponent<ItemPickup>();
                pickup.item = item;
                pickup.gridPos = tile;

                itemMap[tile] = pickup;
            }
        }
    }



    void spawnUp() {

        //not on lvl 1
        if (level <= 1)
            return;

        int x = Random.Range(playerRoom.x, playerRoom.x + playerRoom.width);
        int y = Random.Range(playerRoom.y, playerRoom.y + playerRoom.height);

        Vector2Int tile = new Vector2Int(x, y);

        bool isFloor = dungeon.tiles[tile.x, tile.y] == TileType.Floor;
        bool isOccupied = occupiedTiles.Contains(tile);

        //dont spawn on player!
        bool isPlayerTile = tile.Equals(playerSpawn);

        if (isFloor && !isOccupied && !isPlayerTile) {

            occupiedTiles.Add(tile);
            stairsPos = tile;

            Vector3 pos = new Vector3(tile.x, 0, tile.y);
            Instantiate(stairsUp, pos, default, transform);

            return;
        }
    }



    void spawnExit() {
        //i think its failing to spawn sometimes??

        //so get the furthest room first (or at least a room far enough doesnt have to be furthest)
        Vector2Int playerCentre = roomCentre(playerRoom);

        RectInt exitRoom = roomlist[0];
        float maxDist = 0f;

        foreach (RectInt r in roomlist) {

            //skip player room (was spawning in there for some reason?)
            if (r.Equals(playerRoom)) continue;

            Vector2Int centre = roomCentre(r);

            float dist = Vector2Int.Distance(playerCentre, centre);

            if (dist > maxDist) {
                maxDist = dist;
                exitRoom = r;
            }
        }


        //n then tile in that room to spawn the stair
        //trying a few times because it fails sometimes
        for (int i = 0; i < 10; i++) {
            int x = Random.Range(exitRoom.x, exitRoom.x + exitRoom.width);
            int y = Random.Range(exitRoom.y, exitRoom.y + exitRoom.height);

            Vector2Int tile = new Vector2Int(x, y);

            bool isFloor = dungeon.tiles[tile.x, tile.y] == TileType.Floor;
            bool isOccupied = occupiedTiles.Contains(tile);

            if (isFloor && !isOccupied) {

                occupiedTiles.Add(tile);
                exitPos = tile;

                Vector3 pos = new Vector3(tile.x, 0, tile.y);
                Instantiate(exit, pos, default, transform);

                return;
            }
        }
    }

    /*
    public void refreshUI() {
        if (lvl != null) {
            lvl.text = level.ToString();
        }
    }
    */


    public void loadGame(PlayerControl player) {
        string path = Application.persistentDataPath + "/save.json";

        if (!File.Exists(path)) {
            Debug.Log("no save found");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        //clear everything
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        occupiedTiles.Clear();
        roomlist.Clear();

        //get level
        level = data.level;

        //dungeon grid restore
        dungeon = new DungeonData(data.width, data.height);

        for (int x = 0; x < data.width; x++)
        for (int y = 0; y < data.height; y++) {
            dungeon.tiles[x, y] = data.tiles[x + y * data.width];
        }


        buildWorld();


        //enmies
        foreach (EnemySave e in data.enemies) {
            GameObject prefab = enemy[0];

            Vector3 pos = new Vector3(e.x, 0, e.y);
            GameObject obj = Instantiate(prefab, pos, default, transform);

            EnemyAI en = obj.GetComponent<EnemyAI>();
            en.currHealth = e.health;
        }

        //rebuild items
        foreach (ItemSave i in data.items) {
            ItemData itemData = itemPool.Find(x => x.itemID == i.itemID);
            if (itemData == null) continue;

            Vector3 pos = new Vector3(i.x, 0, i.y);
            GameObject obj = Instantiate(itemData.worldPrefab, pos, default, transform);

            ItemPickup pickup = obj.GetComponent<ItemPickup>();
            pickup.item = itemData;
            pickup.gridPos = new Vector2Int(i.x, i.y);
        }

        //player
        player.gridPos = new Vector2Int(data.player.x, data.player.y);
        player.transform.position = new Vector3(data.player.x, 0, data.player.y);

        player.currHealth = data.player.health;
        //player.hp.text = player.currHealth + "/" + player.maxHealth;

        player.score = PlayerPrefs.GetInt("score", 0);
        //player.sc.text = "score: " + player.score; //soo doing the text updates here fucks everything
        Debug.Log(player.score);

        //chests
        foreach (var c in data.chests) {
            if (c.opened)
                continue; //skip opened chests

            Vector3 pos = new Vector3(c.x, 0, c.y);
            Instantiate(chest, pos, default, transform);
        }

        //restpre inventory
        player.inventory.itemLoad(
            data.inventoryItemIDs,
            data.inventoryCounts
        );

        //restore stairs
        if (data.stairsUp != null) {
            Vector2Int pos = new Vector2Int(data.stairsUp.x, data.stairsUp.y);
            stairsPos = pos;

            Vector3 worldPos = new Vector3(pos.x, 0, pos.y);
            Instantiate(stairsUp, worldPos, default, transform);
        }
        if (data.stairsDown != null) {
            Vector2Int pos = new Vector2Int(data.stairsDown.x, data.stairsDown.y);
            exitPos = pos;

            Vector3 worldPos = new Vector3(pos.x, 0, pos.y);
            Instantiate(exit, worldPos, default, transform);
        }

        //rebind player dungeon ref (keeps losing it?)
        player.dungeonGen = this;
        player.dungeon = this.dungeon;

        if (lvl == null)
            lvl = FindObjectOfType<TMP_Text>();
        lvl.text = "floor: " + level.ToString();

        Debug.Log("game loaded :p");
    }

}
