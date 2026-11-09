using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemMenu : MonoBehaviour {
    public GameObject panel;
    private InventorySlot slot;
    public TMP_Text txt; //the switchy btn's text

    public ItemControl cont;
    public InventoryUI ui;

    //you can uhhhhhh open close equip/eat throw

    public void open(InventorySlot s) { //pos of mouse so ik which slot clicked?
        panel.SetActive(true);
        slot = s;

        if (s.item.type == ItemType.consumable) {
            txt.text = "eat";
        } else {
            txt.text = "equip";
        }

    }

    public void use() {
        cont.useItem(slot.item);
        ui.refresh();
        cancel(); //using that just to close the panel
    }

    public void throwI(){
        cont.inventory.removeItem(slot.item, 1);
        //need to actually spawn it in world again
        Debug.Log("threw " + slot.item.displayName);
        ui.refresh();
        cancel();
    }


    public void cancel() {
        panel.SetActive(false);
    }
}
