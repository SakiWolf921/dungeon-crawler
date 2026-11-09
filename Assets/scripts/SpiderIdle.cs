using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderIdle : EnemyAI {
    public Transform legsLeft;
    public Transform legsRight;

    private float left = 5f;
    private float right = -5f;

    public AudioSource sound;
    public AudioClip clip;

    private Vector3 baseScale;

    public override void Awake() {
        base.Awake();
        baseScale = transform.localScale;
        InvokeRepeating(nameof(scuttle), 0.5f, 0.5f);
    }

    void scuttle() {
        float l = left * -1;
        float r = right * -1;

        legsLeft.Rotate(0, l, 0);
        legsRight.Rotate(0, r, 0);

        left = l;
        right = r;
    }

    public override void attack(){
        base.attack();

        if (p != null) {
            p.damage(1);
        }
    }

    protected override IEnumerator moveAnim(Vector3 targetWorldPos) {
        //base.moveAnim; //cant do this i think

        int x = Random.Range(0, 3);
        if (x == 2 && noise == true) {
            sound.PlayOneShot(clip);
        }

        yield return StartCoroutine(base.moveAnim(targetWorldPos));
    }

    public override void die(){
        base.die();

        p.score += 50;
        p.sc.text = "score: " + p.score;

        PlayerPrefs.SetInt("score", p.score);
        PlayerPrefs.Save();
    }
}
