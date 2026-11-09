using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UNFINISHED SCRIPT//
//NO LONGER IN USE (for now at least - i still like the attack cube idea for later)//

public class AttackCube : MonoBehaviour {
//couldnt inherit enemy because it kept bugging out when i tried that so ik some of this is repetitive but
//maybe ill find a way somehow

    public float moveDuration = 0.2f;

    private Vector2Int gridPos;
    private bool moving = false;

    //private Transform player;

    void Awake() {

        gridPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );
    }

    public void takeTurn(Vector2Int playerPos) {
        if (moving)
            return;

        Vector2Int diff = playerPos - gridPos;

        Vector2Int step;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y)) {
            step = new Vector2Int((int)Mathf.Sign(diff.x), 0);
        } else {
            step = new Vector2Int(0, (int)Mathf.Sign(diff.y));
        }

        Vector2Int target = gridPos + step;

        if (target == playerPos) {
            FindObjectOfType<PlayerControl>()?.damage(1);
            return;
        }

        gridPos = target;
        StartCoroutine(move(new Vector3(target.x, 0, target.y)));
    }

    IEnumerator move(Vector3 target) {
        moving = true;

        Vector3 start = transform.position;
        float t = 0;

        while (t < 1f) {
            t += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        moving = false;
    }
}
