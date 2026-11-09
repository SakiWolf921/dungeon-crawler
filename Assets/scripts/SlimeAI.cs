using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : EnemyAI {

    //idle
    public float wobbleSpeed = 1.5f;
    public float wobbleAmount = 0.1f;
    public float jumpHeight = 0.5f;

    private Vector3 baseScale;

    public AudioSource sound;
    public AudioClip clip;


    void Awake() {
        base.Awake(); //grid snap from parent
        baseScale = transform.localScale;
    }

    void Update() {
        if(!moving){
            //facePlayer(); //is in parent
            wobble(Time.time * wobbleSpeed);
        }

    }

    void wobble(float tValue) {
        float t = Mathf.Sin(tValue);
        float y = 1f + t * wobbleAmount;
        float xz = 1f - t * wobbleAmount * 0.5f;

        transform.localScale = new Vector3( baseScale.x * xz, baseScale.y * y, baseScale.z * xz);
    }

    protected override IEnumerator moveAnim(Vector3 targetWorldPos) {
        moving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0;

        //its too annoying every turn
        int x = Random.Range(0, 3);
        if (x == 2 && noise == true) {
            sound.PlayOneShot(clip);
        }

        while (elapsed < 1f) {
            elapsed += Time.deltaTime / moveDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, targetWorldPos, elapsed);
            currentPos.y = Mathf.Sin(elapsed * Mathf.PI) * jumpHeight;
            transform.position = currentPos;

            wobble(elapsed * Mathf.PI);

            yield return null;
        }

        transform.position = targetWorldPos;
        moving = false;
    }


}
