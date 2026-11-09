using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {

/*
just all the player stuff
so like
position, moving, health, score, inventory
*/

    public DungeonGen dungeonGen;
    public DungeonData dungeon;
    private bool notBoss; //this is horribly confusing isnt it...note it is NOT boss
    //so notBoss = true on lvl 1-4, false on lvl 5
    public EnemyAI en;

    public float moveDuration = 0.2f;
    public Vector2Int gridPos;
    private bool isBusy = false;

    public Transform cameraPivot;
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool isRealigning = false;
    private bool transitioningFloor = false;

    private Stack<DungeonGen> prevFloors = new Stack<DungeonGen>();
    private DungeonGen currFloor;
    public DungeonGen dungeonPrefab;

    public GameObject healthbar;
    public Healthbar barVal;
    public int maxHealth = 25;
    public int currHealth;
    public TMP_Text hp;
    public TMP_Text sc;

    public ItemData currWeapon;

    public int count = 0;
    public int score = 0;
    public int exp = 1; //inc every 5 kills //NOT DONE YET//
    //(n ens killed * 20)
    //- n hits
    //- n pots
    //smth like that idk

    public UIPanels ui;
    public MapRender mapRender;

    public Inventory inventory;
    public ItemControl itemControl;
    private List<ItemPickup> worldItems = new List<ItemPickup>();

    public static PlayerControl Instance;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        //kills the spare sometimes spawned in boss

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    IEnumerator Start() {
        yield return null;
        dungeonGen = FindObjectOfType<DungeonGen>();
        inventory = GetComponent<Inventory>();
        itemControl = GetComponent<ItemControl>();

        if (dungeonGen != null) {
            //normal game//
            notBoss = true;

            dungeon = dungeonGen.dungeon;
            currFloor = dungeonGen;

            //player//
            gridPos = dungeonGen.playerSpawn;
            transform.position = new Vector3(gridPos.x, 0, gridPos.y);
        } else {
            //boss scene//
            notBoss = false;

            //snap to grid anyway
            int x = Mathf.RoundToInt(transform.position.x);
            int z = Mathf.RoundToInt(transform.position.z);

            gridPos = new Vector2Int(x, z);
            transform.position = new Vector3(x, 0, z);
        }

        //this is to hide the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cacheItems();

        //health//
        barVal = healthbar.GetComponent<Healthbar>();
        if (currHealth <= 0) {
            currHealth = maxHealth; //* exp;
            barVal.setMaxHealth(maxHealth);
        } else {
            barVal.setHealth(currHealth);
        }
        barVal.en = false;
        hp.text = maxHealth.ToString() + "/" + maxHealth.ToString();

        sc.text = "score: " + score;

        if (notBoss) { //ie if lvl 1-4
            //map//
            showTiles(); //log them as discovered
            mapRender.updateMap(gridPos);
        }
    }


    void Update() {
        if (GameState.gameOver)
            return;
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        //inventory
        if (Input.GetKeyDown(KeyCode.I)) {
            ui.togglePanel(ui.inventory);

            //unlock the cursor when inv open
            bool isOpen = ui.inventory.activeSelf;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpen;
        }

        //stop everything else if the inv open
        if (ui.inventory.activeSelf) return;

        mouseCam();

        //mouse move if not realign
        if (!isRealigning) {
            mouseCam();
        }

        //help button - shows all the controls
        if (Input.GetKeyDown(KeyCode.H))
            ui.togglePanel(ui.help);

        //space fo realign
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2)) {
            if (!isRealigning) StartCoroutine(camRealign());
        }

        if (isBusy)
            return;

        if (Input.GetKeyDown(KeyCode.W)) tryMove(transform.forward);
        if (Input.GetKeyDown(KeyCode.S)) tryMove(-transform.forward);
        if (Input.GetKeyDown(KeyCode.A)) tryMove(-transform.right);
        if (Input.GetKeyDown(KeyCode.D)) tryMove(transform.right);

        //manual turn
        if (Input.GetKeyDown(KeyCode.E)) StartCoroutine(rotate(90f));
        if (Input.GetKeyDown(KeyCode.Q)) StartCoroutine(rotate(-90f));

        if (Input.GetKeyDown(KeyCode.F)) tryHit(); //prolly make this left click later

        if (Input.GetKeyDown(KeyCode.X)) {
            lvlTransition();
        }

        //wait turn
        if (Input.GetKeyDown(KeyCode.N)) {
            wait();
        }

        //player info panel TODO
        if (Input.GetKeyDown(KeyCode.Tab)) {
            //panel();
        }

        //map
        if (Input.GetKeyDown(KeyCode.M)) ui.togglePanel(ui.map);

        //quick slots TODO
        //shuffle through them to use certain item (like the one where you put wands in pd)
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            quickSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            quickSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            ui.togglePanel(ui.pause);
            bool isOpen = ui.pause.activeSelf;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpen;
        }
    }


    void mouseCam() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        yRotation += mouseX;

        //rotate the cam pivot
        cameraPivot.localEulerAngles = new Vector3(xRotation, yRotation, 0);
    }


    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (dungeonGen != null && dungeonGen.dungeon != null) {
            notBoss = true;
        } else {
            notBoss = false;
        }

        ui = FindObjectOfType<UIPanels>();
        mapRender = FindObjectOfType<MapRender>();

        //healthbar = GameObject.Find("health")?.GetComponent<Slider>();
        barVal = GameObject.FindGameObjectWithTag("hBar")?.GetComponent<Healthbar>();
        //hp = FindObjectOfType<TMP_Text>();
        hp = GameObject.FindGameObjectWithTag("hp")?.GetComponent<TMP_Text>();
        sc = GameObject.FindGameObjectWithTag("score")?.GetComponent<TMP_Text>();
        //sc.text = "score: " + score; //is in hpupdate now cuz idk it works there
        hpUpdate();

        dungeonGen = FindObjectOfType<DungeonGen>();

        if (dungeonGen != null) {
            dungeon = dungeonGen.dungeon;
            currFloor = dungeonGen;
        }

        inventory = GetComponent<Inventory>();
        itemControl = GetComponent<ItemControl>();

        cacheItems();
    }



    void tryMove(Vector3 worldDir) {
        if (isBusy || transitioningFloor)
            return;

        //world direction -> grid units
        Vector2Int dir = new Vector2Int(
            Mathf.RoundToInt(worldDir.x),
            Mathf.RoundToInt(worldDir.z)
        );

        Vector2Int target = gridPos + dir;

        if (notBoss) {
            //valid tile chekc//
            //bounds - ie youre not allowed to walk outside the walls
            if (target.x < 0 || target.x >= dungeon.width || target.y < 0 || target.y >= dungeon.height)
                return;

            //check its valid floor
            if (dungeon.tiles[target.x, target.y] != TileType.Floor)
                return;
        }

        //make sure no walk through enemy (note - en can still walk through each other FIX THAT)
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        if (enemies != null) {
            foreach (EnemyAI e in enemies) {
                if (e == null)
                    continue;

                Vector2Int enemyPos = new Vector2Int(
                    Mathf.RoundToInt(e.transform.position.x),
                    Mathf.RoundToInt(e.transform.position.z)
                );

                if (enemyPos == target) {
                    //no move
                    return;
                }
            }
        }

        //move
        gridPos = target;
        StartCoroutine(move(new Vector3(gridPos.x, 0, gridPos.y)));
    }


    IEnumerator move(Vector3 targetPos) {
        isBusy = true;
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }
        transform.position = targetPos;
        isBusy = false;
        heal();

        checkItems();

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        //update the slimes
        foreach (EnemyAI e in enemies) {
            if (notBoss)
                e.takeTurn(gridPos, dungeonGen.dungeon);
            else
                e.takeTurn(gridPos, null);
        }

        if (notBoss) {
            showTiles();
            mapRender.updateMap(gridPos);
        } else {
            AttackCube[] cubes = FindObjectsOfType<AttackCube>();

            foreach (AttackCube c in cubes) {
                c.takeTurn(gridPos);
            }
        }
    }


    void checkItems() {
        for (int i = 0; i < worldItems.Count; i++) {
            ItemPickup item = worldItems[i];

            if (item == null) continue;

            if (item.gridPos == gridPos) {
                if (inventory.addItem(item.item)) {
                    Debug.Log("picked up " + item.item.displayName);

                    Destroy(item.gameObject);
                    worldItems.RemoveAt(i);
                    i--;
                }
            }
        }
    }


    void showTiles(){
        int vis = 5; //visiblity

        //loop round player
        for (int dx = -vis; dx <= vis; dx++) {
            for (int dy = -vis; dy <= vis; dy++) {

                //make it real world coord
                int x = gridPos.x + dx;
                int y = gridPos.y + dy;

                //bounds check
                if (x < 0 || x >= dungeon.width || y < 0 || y >= dungeon.height)
                    continue;

                dungeon.explored[x, y] = true;
            }
        }
    }

    void cacheItems() {
        worldItems.Clear();
        worldItems.AddRange(FindObjectsOfType<ItemPickup>());
    }


    void tryHit() {

        //direction player facing
        Vector2Int dir = new Vector2Int(
            Mathf.RoundToInt(transform.forward.x),
            Mathf.RoundToInt(transform.forward.z)
        );

        Vector2Int target = gridPos + dir;

        //find all enemies
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI e in enemies) {

            Vector2Int enemyPos = new Vector2Int(
                Mathf.RoundToInt(e.transform.position.x),
                Mathf.RoundToInt(e.transform.position.z)
            );

            float dist = Vector2Int.Distance(gridPos, enemyPos);


            int dmg = 1;

            if (currWeapon != null) {
                dmg = Random.Range(currWeapon.minDmg, currWeapon.maxDmg + 1);
            }

            //allow hit if diagonal aswell
            //DOESNT WORK ATM
            if (dist <= 1.1f) {
                e.damage(dmg);
                Debug.Log("hit: " + e.name);
                break;
            }

            if (enemyPos == target) {
                e.damage(dmg);
                break;
            }
        }

        if (notBoss) {
            //enemy turn
            foreach (EnemyAI e in enemies) {
                e.takeTurn(gridPos, dungeonGen.dungeon);
            }
        } else {
            AttackCube[] cubes = FindObjectsOfType<AttackCube>();

            foreach (AttackCube c in cubes) {
                c.takeTurn(gridPos);
            }
        }
    }


    public void damage(int amount) {
        currHealth -= amount;

        score -= 1;
        sc.text = "score: " + score;
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();

        if (barVal != null) {
            barVal.setHealth(currHealth);
        }

        if (currHealth <= 0) {
            Debug.Log("DEAD");
            //todo make it end the game
            ui.togglePanel(ui.dead);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameState.gameOver = true;
        } else {
            hpUpdate();
        }
    }


    void heal() {
        //if not being attacked by enemy
        //roll d4 to heal
        int die = Random.Range(0, 10);

        if (die == 9 && currHealth < 20) {
            currHealth += 1;
            hpUpdate();
            barVal.setHealth(currHealth);
            Debug.Log("healed :>");
        } else {
            Debug.Log("bad roll");
        }

        //if potion: currHealth = maxHealth;
    }


    //todo
    //make a starve/eat system


    //level control//
    void lvlTransition() {
        if (!notBoss)
            return;

        Vector2Int playerPosGrid = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );

        if (playerPosGrid.Equals(currFloor.exitPos)) {
            nextLvl();
            return;
        }

        if (playerPosGrid.Equals(currFloor.stairsPos)) {
            prevLvl();
            return;
        }
    }


    void nextLvl() {
        transitioningFloor = true;
        Debug.Log("next level");

        prevFloors.Push(currFloor); //save floor
        currFloor.gameObject.SetActive(false); //disable it

        if (currFloor.level == 4) {
            DungeonGen.blockGen = true; //blocks generation if boss
        }

        DungeonGen newDungeon = Instantiate(dungeonPrefab);
        newDungeon.level = currFloor.level + 1;

        dungeonGen.lvl.text = "floor: " + currFloor.level.ToString();

        StartCoroutine(newFloor(newDungeon));
    }


    IEnumerator newFloor(DungeonGen newDungeon) {

        yield return null; //wait for Awake()

        cacheItems();

        currFloor = newDungeon;
        dungeonGen = newDungeon;
        dungeon = newDungeon.dungeon;

        Vector2Int spawn = newDungeon.playerSpawn;
        gridPos = spawn;
        transform.position = new Vector3(spawn.x, 0, spawn.y);

        transitioningFloor = false;
    }


    void prevLvl() {
        if (prevFloors.Count == 0)
            return;

        Debug.Log("prev lvl");

        //destroy current floor
        Destroy(currFloor.gameObject);

        //restore previous
        DungeonGen prev = prevFloors.Pop();

        prev.gameObject.SetActive(true);

        currFloor = prev;
        dungeonGen = prev;
        dungeon = prev.dungeon;

        Vector2Int spawn = prev.playerSpawn;
        gridPos = spawn;
        transform.position = new Vector3(spawn.x, 0, spawn.y);

        dungeonGen.lvl.text = "floor: " + currFloor.level.ToString();
    }


    //keybinds//
    void wait() {
        Debug.Log("player wait");

        //enemy turn
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI e in enemies) {
            if (notBoss) {
                e.takeTurn(gridPos, dungeonGen.dungeon);
            } else {
                e.takeTurn(gridPos, null);

                AttackCube[] cubes = FindObjectsOfType<AttackCube>();
                foreach (AttackCube c in cubes) {
                    c.takeTurn(gridPos);
                }
            }
        }
    }

/*
    void showMap() {
        Debug.Log("show map");
    }

    void showInv() {
        Debug.Log("show inv");
    }
*/

    //doesnt fucking WORK
    void quickSlot(int i) {
    if (isBusy) return;
        if (inventory == null)
            return;
        if (inventory.items == null)
            return;
        if (i < 0 || i >= inventory.items.Count)
            return;

        InventorySlot slot = inventory.items[i];

        if (slot == null || slot.item == null)
            return;

        itemControl.useItem(slot.item);
        wait(); //just so using items uses a turn
    }

    void pause() {
        Debug.Log("paused");
    }

    void hpUpdate() { //made to avoid the nulling because it kept panicing after scene switch n reload
        if (hp == null)
            return;
        hp.text = currHealth + "/" + maxHealth;

        if (sc == null)
            return;
        sc.text = "score: " + score;

        if (barVal == null)
            return;
        barVal.setHealth(currHealth);
        barVal.en = false;
    }


    //camera//
    IEnumerator rotate(float angle) {
        isBusy = true;

        yRotation = 0;
        cameraPivot.localEulerAngles = new Vector3(xRotation, 0, 0);

        float rotatedSoFar = 0f;
        float speed = angle / moveDuration;

        while (Mathf.Abs(rotatedSoFar) < Mathf.Abs(angle)) {
            float step = speed * Time.deltaTime;

            transform.Rotate(0, step, 0);

            rotatedSoFar += step;
            yield return null;
        }


        //snap to grid
        Vector3 finalEuler = transform.eulerAngles;
        finalEuler.y = Mathf.Round(finalEuler.y / 90f) * 90f;
        transform.eulerAngles = finalEuler;

        isBusy = false;
    }


    IEnumerator camRealign() {
        isRealigning = true;
        float t = 0;
        float duration = 0.2f;

        Vector3 startEuler = cameraPivot.localEulerAngles;

        //makes it go from 360 - 0 again
        Vector3 targetEuler = new Vector3(
            Mathf.DeltaAngle(0, startEuler.x) > 0 ? 0 : 0,
            Mathf.DeltaAngle(0, startEuler.y) > 0 ? 0 : 0,
            0
        );

        while (t < 1f) {
            t += Time.deltaTime / duration;

            float currX = Mathf.LerpAngle(startEuler.x, 0, t);
            float currY = Mathf.LerpAngle(startEuler.y, 0, t);

            cameraPivot.localEulerAngles = new Vector3(currX, currY, 0);
            yield return null;
        }

        cameraPivot.localEulerAngles = Vector3.zero;

        xRotation = 0f;
        yRotation = 0f;

        isRealigning = false;
    }

}
