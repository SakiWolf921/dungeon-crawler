using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanels : MonoBehaviour {

    public GameObject map;
    //public GameObject playerInfo;
    public GameObject inventory;
    public GameObject help;
    public GameObject pause;
    public GameObject dead;

    private GameObject current;

    public void togglePanel(GameObject panel) {

        //if clicking same panel: close
        if (current == panel) {
            panel.SetActive(false);
            current = null;
            return;
        }

        //close current panel
        if (current != null) {
            current.SetActive(false);
        }

        //open new
        panel.SetActive(true);
        current = panel;
    }
}

