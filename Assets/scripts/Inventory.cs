using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public int maxSlots = 20;

    public List<InventorySlot> items = new List<InventorySlot>();

    public bool addItem(ItemData item, int amount = 1) {
        //stack if possible (defined when item made)
        if (item.stackable) {
            foreach (var slot in items) {
                if (slot.item == item && slot.count < item.maxStack) {
                    slot.count += amount;
                    return true;
                }
            }
        }

        //add new slot
        if (items.Count >= maxSlots)
            return false;

        items.Add(new InventorySlot(item, amount));
        return true;
    }


    public void removeItem(ItemData item, int amount = 1) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i].item == item) {
                items[i].count -= amount;

                if (items[i].count <= 0)
                    items.RemoveAt(i);

                return;
            }
        }
    }


    public List<string> itemIDs() {
        List<string> ids = new List<string>();

        foreach (var slot in items) {
            ids.Add(slot.item.itemID);
        }

        return ids;
    }

    public List<int> itemCount() {
        List<int> c = new List<int>();

        foreach (var slot in items) {
            c.Add(slot.count);
        }

        return c;
    }


    public void itemLoad(List<string> ids, List<int> c) {
        items.Clear();

        for (int i = 0; i < ids.Count; i++) {
            ItemData item = ItemDatabase.instance.getItemID(ids[i]);
            if (item == null) continue;

            items.Add(new InventorySlot(item, c[i]));
        }
    }

}


//too lazy to make another file and doing this hasnt broken anything yet
public class InventorySlot {
    public ItemData item;
    public int count;

    public InventorySlot(ItemData newItem, int amount) {
        item = newItem;
        count = amount;
    }
}
