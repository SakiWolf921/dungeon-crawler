using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        PlayerControl p = other.GetComponent<PlayerControl>();

        if (p != null) {
            int dmg = Random.Range(0, 4); //max 3 dmg for now - to inc to 5 maybe?
            p.damage(dmg);
            Destroy(gameObject);
        }
    }
}

