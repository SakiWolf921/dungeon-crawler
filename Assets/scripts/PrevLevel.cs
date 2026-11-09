using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevLevel : MonoBehaviour {

    public float interactDistance = 3f;
    public GameObject popup;

    // Start is called before the first frame update
    void Start() {
        popup.SetActive(false);
    }

    public void ShowPopup(bool show) {
        popup.SetActive(show);
    }
}

