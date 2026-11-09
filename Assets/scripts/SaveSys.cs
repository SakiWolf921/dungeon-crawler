using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class SaveSys : MonoBehaviour {
    string path => Application.persistentDataPath + "/save.json";

    public void SaveGame(DungeonGen dungeon, PlayerControl player) {

        //was losing refs so
        if (player == null) {
            Debug.Log("player is null");
            return;
        }

        //get the inventory ref
        if (player.inventory == null) {
            Debug.LogWarning("inventory null - recovering...");
            player.inventory = player.GetComponent<Inventory>();
        }

        if (player.inventory == null) {
            Debug.LogError("inventory recover failed - still null");
            return;
        }


        SaveData data = new SaveData();

        //player
        data.player = new PlayerSave {
            x = player.gridPos.x,
            y = player.gridPos.y,
            health = player.currHealth
        };

        data.level = dungeon.level;

        //dungeon
        data.width = dungeon.dungeon.width;
        data.height = dungeon.dungeon.height;

        data.tiles = new TileType[data.width * data.height];

        for (int x = 0; x < data.width; x++)
        for (int y = 0; y < data.height; y++) {
            data.tiles[x + y * data.width] = dungeon.dungeon.tiles[x, y];
        }

        //enemies
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        data.enemies = new System.Collections.Generic.List<EnemySave>();

        foreach (var e in enemies) {
            data.enemies.Add(new EnemySave {
                enemyID = e.name,
                x = Mathf.RoundToInt(e.transform.position.x),
                y = Mathf.RoundToInt(e.transform.position.z),
                health = e.currHealth
            });
        }

        //items
        ItemPickup[] items = FindObjectsOfType<ItemPickup>();
        data.items = new System.Collections.Generic.List<ItemSave>();

        foreach (var i in items) {
            data.items.Add(new ItemSave {
                itemID = i.item.itemID,
                x = i.gridPos.x,
                y = i.gridPos.y
            });
        }

        //chests
        ChestOpen[] chests = FindObjectsOfType<ChestOpen>();
        data.chests = new List<ChestSave>();

        foreach (var c in chests) {
            data.chests.Add(new ChestSave {
                x = Mathf.RoundToInt(c.transform.position.x),
                y = Mathf.RoundToInt(c.transform.position.z),
                opened = c.open
            });
        }

        //inventory
        data.inventoryItemIDs = player.inventory.itemIDs();
        data.inventoryCounts = player.inventory.itemCount();


        //save teh up n down stairs//
        data.stairsUp = new StairSave {
            x = dungeon.stairsPos.x,
            y = dungeon.stairsPos.y
        };

        data.stairsDown = new StairSave {
            x = dungeon.exitPos.x,
            y = dungeon.exitPos.y
        };


        //write to json
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("game saved :p");
    }
}
