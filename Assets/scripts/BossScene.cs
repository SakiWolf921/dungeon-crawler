using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossScene : MonoBehaviour {
    public Transform spawn;
    public TMP_Text lvl;
    public PlayerControl player;

    void Start() {
        player = FindObjectOfType<PlayerControl>();

        if (player != null) {
            Debug.Log("player found");

            //put player at the spawn
            player.transform.position = spawn.position; //not wORKING AAAAAAAAAA
        } else {
            Debug.Log("uh oh no player here");
        }


    }
}
