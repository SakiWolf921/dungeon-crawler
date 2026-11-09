using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : EnemyAI {

    private NavMeshAgent agent;
    private Animator anim;
    public BossScene bs; //lmao its bs


    public Transform bossTranform;
    public Transform[] waypoints;
    public float chaseRange = 6f;

    private int tp = 3;
    private int tpTurn = 0;

    private int currentWP = 0;
    private int wpLeft;

    public GameObject proj;
    public float projSpeed = 1f;


/*
attack cubes scrapped

lad is a bit like tengen
will chase you or shoot a projectile (cube honestly)
once it takes x num hits (3 hits maybe?)
it teleports to a random waypoint
*/


    public override void Awake() {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //disable auto move
        agent.updatePosition = false;
        agent.updateRotation = false;

        //grid snap
        transform.position = new Vector3(gridPos.x, 0, gridPos.y);

        maxHealth = 75; //make 50 or smth when player health inc//

        wpLeft = waypoints.Length;
    }

    //main turn - override base
    public override void takeTurn(Vector2Int playerPos, DungeonData dungeon) {
        if (moving)
            return;

        //chase or shott projectile
        //attack if next to you
        //tp if hit 3 turns

        /*
        if (tpTurn >= tp) {
            tpWaypoint();
            tpTurn = 0;
            Debug.Log(tpTurn);
            return;
        } else {
            Debug.Log(tpTurn);
        }
        */

        if (Vector2Int.Distance(gridPos, playerPos) == 1) {
            attack();
            return;
        }

        int die = Random.Range(0, 4);
        if (die == 3) {
            projectile(playerPos);
        } else {
            chase(playerPos, dungeon);
        }
    }


    void move(Vector3 targetWorld, DungeonData dungeon) {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetWorld, path);

        if (path.corners.Length < 2)
            return;

        Vector3 next = path.corners[1];

        Vector2Int dir = new Vector2Int(
            Mathf.RoundToInt(next.x - gridPos.x),
            Mathf.RoundToInt(next.z - gridPos.y)
        );

        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.y = Mathf.Clamp(dir.y, -1, 1);

        //stops it teleporting diagonal
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir.y = 0;
        else
            dir.x = 0;

        Vector2Int target = gridPos + dir;
        gridPos = target;
        faceThing(gridPos);

        anim.SetBool("moving", true);
        StartCoroutine(moveAnim(new Vector3(gridPos.x, 0, gridPos.y))); //in enemyai
        //anim.SetBool("moving", false); //runs at same time >:/
            //happens in enemyai coroutine
    }


    void tpWaypoint() {
        if (waypoints.Length == 0)
            return;
        //(checks if youre already there)

        int rand = Random.Range(0, waypoints.Length);
        Vector3 wp = waypoints[rand].position;

        int x = Mathf.RoundToInt(wp.x);
        int z = Mathf.RoundToInt(wp.z);

        gridPos = new Vector2Int(x, z);
        transform.position = new Vector3(x, 0, z);

        Debug.Log("boss tp");
    }


    void chase(Vector2Int playerPos, DungeonData dungeon) {
        Vector3 playerWorld = new Vector3(playerPos.x, 0, playerPos.y);

        faceThing(playerPos);
        move(playerWorld, dungeon);
    }


    public override void attack() {
        anim.SetTrigger("attack");

        PlayerControl p = FindObjectOfType<PlayerControl>();
        p.damage(3); //make it range 9 - 22 when player health inc happens//

        Debug.Log("boss attack");
    }


    public override void damage(int x) {
        base.damage(x);

        tpTurn++;
        Debug.Log(tpTurn);
        if (tpTurn >= tp) {
            tpWaypoint();
            tpTurn = 0;
            return;
        }

        Debug.Log("boss: ouch");
    }


    void projectile(Vector2Int pos) {
        //chuck smth at you
        faceThing(pos);
        anim.SetTrigger("cast");

        for (int i = 0; i < 3; i++) {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

            GameObject p = Instantiate(proj, spawnPos, default);

            Vector3 target = new Vector3(pos.x, 0.5f, pos.y);
            Vector3 dir = (target - spawnPos).normalized;

            p.GetComponent<Rigidbody>().velocity = dir * projSpeed;
        }

        //game isnt stopping while this happens :/ - to sort that
        //making it fire three for now so its hard to dodge
    }



}
