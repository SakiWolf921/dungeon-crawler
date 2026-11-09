using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public int visRange = 5;
    public float moveDuration = 0.2f;

    protected Vector2Int gridPos;
    protected bool moving = false;
    protected Transform playerTransform;

    public GameObject healthbar;
    private Healthbar barVal;
    private GameObject bar;

    public int maxHealth = 3; //gotta make it inc with level
    public int currHealth;

    public bool noise = false;

    public PlayerControl p;
    private Animator anim; //only for the boss
    public DungeonGen dg;


    public virtual void Awake() {
        //snap to grid
        int x = Mathf.RoundToInt(transform.position.x);
        int z = Mathf.RoundToInt(transform.position.z);

        transform.position = new Vector3(x, 0, z);
        gridPos = new Vector2Int(x, z);

        if (playerTransform == null) {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player"); //this line dont work (nothing uses tags anymore) but it causes an error if i remove so its still here
            if (pObj != null) {
                playerTransform = pObj.transform;
                p = pObj.GetComponent<PlayerControl>();
            }
        }

        anim = GetComponent<Animator>();
        dg = FindObjectOfType<DungeonGen>();
    }

    public virtual void Start() {
        //set health
        if (dg != null) {
            maxHealth = 3 * dg.level;
        }
        currHealth = maxHealth;

        //spawn health bar//
        bar = Instantiate(healthbar, transform);

        //reset to zero (otherwise it floats horribly)
        bar.transform.localPosition = Vector3.zero;
        //make if float on top
        Transform visual = bar.transform.GetChild(0);
        visual.localPosition = new Vector3(0, 1f, 0);

        barVal = bar.GetComponent<Healthbar>();
        barVal.setMaxHealth(maxHealth);
        bar.SetActive(false); //hide bar initially
    }


    public virtual void takeTurn(Vector2Int playerPos, DungeonData dungeon) {
        if (moving)
            return;

        float dist = Vector2Int.Distance(gridPos, playerPos); //gets the distance between player n mob
        int die = Random.Range(0, 4); //roll a d4 to move anyways
        //Debug.Log(die);
        if (dist <= visRange) {
            //transform.LookAt(playerTransform);
            faceThing(playerPos);
            move(playerPos, dungeon);
            noise = true;
        } else if (die == 3) {
            move(playerPos, dungeon);
            noise = false;
        } else {
            noise = false;
        }
    }


    void move(Vector2Int playerPos, DungeonData dungeon) {

        if (dungeon == null) {
            Debug.Log("(enemy ai) dungeon null - skipping turn");
            return;
        }

        Vector2Int diff = playerPos - gridPos;
        Vector2Int step = Vector2Int.zero;

        //move when gap
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            step.x = diff.x > 0 ? 1 : -1;
        else
            step.y = diff.y > 0 ? 1 : -1;

        Vector2Int target = gridPos + step;

        //if player, attack instead of move
        if (target == playerPos) {
            attack();
            return;
        }

        //anotehr enemy on tile?
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI e in enemies) {
            if (e == this) continue; //skip self in list

            Vector2Int otherPos = new Vector2Int(
                Mathf.RoundToInt(e.transform.position.x),
                Mathf.RoundToInt(e.transform.position.z)
            );

            if (otherPos == target) {
                return;
            }
        }

        //wall?
        if (target.x < 0 || target.x >= dungeon.width ||
            target.y < 0 || target.y >= dungeon.height)
            return;

        //move
        if (dungeon.tiles[target.x, target.y] == TileType.Floor) {
            gridPos = target;
            StartCoroutine(moveAnim(new Vector3(gridPos.x, 0, gridPos.y)));
        }
    }


    //virtual so diff enemies can have diff move styles
    protected virtual IEnumerator moveAnim(Vector3 targetWorldPos) {
        moving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0;

        while (elapsed < 1f) {
            elapsed += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(startPos, targetWorldPos, elapsed);
            yield return null;
        }

        transform.position = targetWorldPos;
        moving = false;

        if (dg == null || dg.level == 5) { //or lvl 5 cuz i think it grabs dg at some point in boss
            anim.SetBool("moving", false);
            Debug.Log("aaaaaaa");
        }
    }


    protected void faceThing(Vector2Int pos) {
        Vector3 t = new Vector3(pos.x, transform.position.y, pos.y);
        transform.LookAt(t);
    }



    public virtual void damage(int x) {
        currHealth -= x;
        bar.SetActive(true); //show bar

        if (barVal != null) {
            barVal.setHealth(currHealth);
        }

        if (currHealth <= 0) {
            die();
        }
    }

    public virtual void die(){
        Destroy(gameObject);

        if (p == null) {
            p = FindObjectOfType<PlayerControl>();
        }

        if (p.count >= 4) {
            p.count = 0;
            p.exp++;

            //25 is the base max
            p.maxHealth = 25 * p.exp;
            p.barVal.setMaxHealth(p.maxHealth);

            if (p.currHealth < 25) {
                p.currHealth = 25;
                p.barVal.setHealth(p.currHealth);
            }

            Debug.Log("exp = " + p.exp);
        } else {
            p.count++;
            Debug.Log("count = " + p.count);
        }

        p.score = p.score + 100;
        //Debug.Log("score = " + p.score);
        p.sc.text = "score: " + p.score;

        PlayerPrefs.SetInt("score", p.score);
        PlayerPrefs.Save();
    }


    public virtual void attack(){
        Debug.Log(gameObject.name + " attacks you!");

        if (p == null) {
            p = FindObjectOfType<PlayerControl>();
        }

        if (p != null) {
            p.damage(1);
        }
    }
}
