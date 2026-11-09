using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteract : MonoBehaviour {

    public float interactDistance = 3f;

    void Update() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance)) {
            ChestOpen chest = hit.collider.GetComponent<ChestOpen>();
            PrevLevel prev = hit.collider.GetComponent<PrevLevel>();

            if (chest != null) {
                chest.Popup(true);

                if (Input.GetKeyDown(KeyCode.C)) {
                    chest.Open();
                }

                return;
            }

            if (prev != null) {
                prev.ShowPopup(true);

                return;
            }
        }
    }
}
