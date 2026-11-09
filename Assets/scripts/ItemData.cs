using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType {
    melee,
    ranged,
    armour,
    consumable
}


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject {
    public string itemID;
    public string displayName;
    public Sprite icon;

    public ItemType type;

    public bool stackable = false;
    public int maxStack = 1;

    public GameObject worldPrefab;

    //for weapons
    public int minDmg;
    public int maxDmg;
}


//lets me make a new item
//just so i dont forget:
    //assets - create - inventory - item


