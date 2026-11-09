using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

    public Slider slider;
    public bool en = true; //is enemy

    public void setMaxHealth(int h) {
        slider.maxValue = h;
        slider.value = h;
    }

    public void setHealth(int h) {
        slider.value = h;
    }

    //make face cam (stops it from spinning w the cam)
    void LateUpdate() {
        if (!en) return; //doesnt run this for player
        //breaks when save loaded (runs for player idk why) - to fix//
        //also breaks when go to boss scene

        if (Camera.main != null) {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
