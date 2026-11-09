using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    public static ItemDatabase instance;

    public List<ItemData> allItems;

    private HashSet<string> items = new HashSet<string>();

    void Awake() {
        instance = this;
    }

    public ItemData getItemID(string id) {
        return allItems.Find(i => i.itemID == id);
    }

    public bool identified(string id) {
        return items.Contains(id);
    }

    public void identify(string id) {
        items.Add(id);
    }

    public string name(ItemData item) {
        if (identified(item.itemID))
            return item.displayName;

        return "this is nothing";
    }
}
