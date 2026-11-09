using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemControl : MonoBehaviour {
    public Inventory inventory;
    public PlayerControl player;
    public Image wIcon;

    public void useItem(ItemData item) {
        idItem(item);

        switch (item.type) {
            case ItemType.melee:
                equip(item);
                break;

            case ItemType.ranged:
                ranged(item);
                break;

            case ItemType.armour:
                armour(item);
                break;

            case ItemType.consumable:
                consume(item);
                inventory.removeItem(item, 1);
                break;
        }
    }

    void idItem(ItemData item) {
        //she keeps nulling herself man
        if (ItemDatabase.instance == null) {
            Debug.LogError("no item db");
            return;
        }
        //will run when she stops nulling herself
        if (!ItemDatabase.instance.identified(item.itemID))
            ItemDatabase.instance.identify(item.itemID);
    }

    void consume(ItemData item) {
        Debug.Log("eaten: " + item.displayName);
    }

    void equip(ItemData item) {
        player.currWeapon = item;
        //wIcon.sprite = item.icon;
        //wIcon.enabled = true;
        //comes back as wicon null not sure why - to fix

        Debug.Log("equipped: " + item.displayName);
    }

    void ranged(ItemData item) {
        Debug.Log("used: " + item.displayName);
    }

    void armour(ItemData item) {
        Debug.Log("equipped (armour)" + item.displayName);
    }
}
