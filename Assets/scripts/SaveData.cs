using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//note: everything in here make it serialized or it kills itself//

[System.Serializable]
public class SaveData {
    public int level;

    public PlayerSave player;

    public List<EnemySave> enemies;
    public List<ItemSave> items;
    public List<ChestSave> chests;

    public int width;
    public int height;
    public TileType[] tiles;

    public List<string> inventoryItemIDs;
    public List<int> inventoryCounts;

    public StairSave stairsUp;
    public StairSave stairsDown;
}


[System.Serializable]
public class PlayerSave {
    public int x;
    public int y;
    public int health;
}


[System.Serializable]
public class EnemySave {
    public string enemyID;
    public int x;
    public int y;
    public int health;
}



[System.Serializable]
public class ItemSave {
    public string itemID;
    public int x;
    public int y;
}


[System.Serializable]
public class ChestSave {
    public int x;
    public int y;
    public bool opened;
}

//todo save up n down stairs
[System.Serializable]
public class StairSave {
    public int x;
    public int y;
}






