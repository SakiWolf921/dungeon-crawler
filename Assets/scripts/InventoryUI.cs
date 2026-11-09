using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour {
    public Inventory inventory;
    public Transform slotParent;
    public GameObject slotPrefab;

    void OnEnable() {
        StartCoroutine(Init());
    }

    IEnumerator Init() {
        //wait 1 frame so things exist
        yield return null;

        if (inventory == null) {
            PlayerControl player = FindObjectOfType<PlayerControl>();
            if (player != null) {
                inventory = player.inventory;
            }
        }

        refresh();
    }

    public void refresh() {
        if (inventory == null)
            return;

        //kill the old one
        foreach (Transform child in slotParent) {
            Destroy(child.gameObject);
        }

        //and make a new one
        for (int i = 0; i < inventory.maxSlots; i++) {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            InventorySlotUI ui = obj.GetComponent<InventorySlotUI>();
            ui.menu = FindObjectOfType<ItemMenu>();

            if (i < inventory.items.Count) {
                ui.setup(inventory.items[i], i);
            } else {
                ui.clear();
            }
        }
    }
}
