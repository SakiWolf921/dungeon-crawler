using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour {

    //public GameObject chest;
    private Vector3 chestPos;
    public bool open; //has chest been opened?

    public Transform lid;
    public GameObject popup; //to show c
    //ItemControl item;

    public AudioSource sound;
    public AudioClip clip;


    void Awake() {
        chestPos = lid.localPosition;
        open = false;
        popup.SetActive(false);
    }


    public void Open() {

        //if(Input.GetKeyDown("c")){
            //Debug.Log("c click! open chest");
            if(!open) {
                lid.Rotate(-30, 0, 0);
                lid.localPosition = chestPos + new Vector3(0f, 0.15f, 0.3f);

                open = true;
                popup.SetActive(false);
                //random item pops up
                //item.randItem();

                sound.PlayOneShot(clip);
            }
        //}
    }

    public void Popup(bool show) {
        if(!open) {
            popup.SetActive(show);
        }
    }

}
