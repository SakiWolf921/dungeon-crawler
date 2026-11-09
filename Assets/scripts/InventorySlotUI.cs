using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour {
    public TMP_Text itemName;
    public Image icon;
    public ItemMenu menu;

    public InventorySlot slot;
    private int index;

    public void setup(InventorySlot s, int i) {
        slot = s;
        index = i;

        itemName.text = s.item.displayName;
        icon.sprite = s.item.icon;
        menu = FindObjectOfType<ItemMenu>();
    }

    public void useItem() {
        FindObjectOfType<PlayerControl>().itemControl.useItem(slot.item);
    }

    public void clear() {
        itemName.text = "";
        icon.sprite = null;
        icon.enabled = false;
    }


    //need to make it so a left click opens the menu
    //prolly make it buttons
    public void click() {
        if (slot == null)
            return;

        if (menu == null) {
            menu = FindObjectOfType<ItemMenu>(true);
        }

        if (menu == null) {
            Debug.Log("item menu lost aaaaaaa");
            return;
        }

        menu.open(slot);
    }
}
