using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public GlobalVariables gv;
    public Vector2 toMove;
    public Rigidbody2D rb;
    public Animator anim;
    public int team;

    public HPSystem enemy;

    public bool isAttacking;

    public LayerMask enemyLayerMask;
    public string target;
    public float damage;

    public float moveSpeed;

    public EnemyDetector left;
    public EnemyDetector right;

    public bool ready;
    // Start is called before the first frame update

    public void SetAbilities()
    {
        ready = true;
        rb = GetComponent<Rigidbody2D>();
        anim.Play("SwordswrathWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();

        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";

        if (tag == "Team1")
        {
            gv.team1units.Add(gameObject);
        }
        else
        {
            gv.team2units.Add(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!ready) return;
        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";

        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("SwordswrathIdle");
            StopAllCoroutines();

            isAttacking = false;
            return;
        }
        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if (((target == "Team2") && (toMove == (Vector2)gv.garrison1.position || gv.team1 == 2)) || ((target == "Team1") && (toMove == (Vector2)gv.garrison2.position || gv.team2 == 2)))
            {
                isAttacking = false;
                anim.Play("SwordswrathIdle");
                return;
            }

        }
        else if (isAttacking)
        {
            return;
        }
        else
        {
            isAttacking = false;
        }


        if (Vector2.Distance(transform.position, toMove) < .7f && (((target == "Team2") && (toMove != (Vector2)gv.garrison1.position && gv.team1 != 2)) || ((target == "Team1") && (toMove != (Vector2)gv.garrison2.position && gv.team2 != 2))))
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackAnimation());
                return;
            }
            else if (isAttacking)
            {
                return;
            }
        }

        anim.Play("SwordswrathWalk");
        Vector2 moveDirection = (toMove - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        if (moveDirection.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }


    IEnumerator AttackAnimation()
    {
        isAttacking = true;
        anim.Play("SwordswrathAttack");
        yield return new WaitForSeconds(.5f);
        Attack(damage);
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready) return;
        AI();
    }

    public void AI()
    {
        if ((gv.team1 == 1 && (target == "Team2")))
        {
            toMove = gv.garrison1.transform.position;
            //move to formation
        }
        else if ((gv.team2 == 1 && (target == "Team1")))
        {
            toMove = gv.garrison2.transform.position;
        }
        else if ((gv.team1 == 3 && target == "Team2") || (gv.team2 == 3 && target == "Team1"))
        {
            findEnemy();
        }
    }

    public bool Attack(float dmg)
    {
        if (!enemy) return false;
        //check if sword is touching an enemy or something man idk 
        if (GetComponent<SpriteRenderer>().flipX && left.IsTargetWithinRange(enemy.gameObject))
        {
            enemy.Damage(dmg);
            return true;
        }
        else if (!GetComponent<SpriteRenderer>().flipX && right.IsTargetWithinRange(enemy.gameObject))
        {
            enemy.Damage(dmg);
            return true;
        }
        return false;
    }

    public void findEnemy()
    {
        float closestDistance = Mathf.Infinity;
        int index = -1;
        HPSystem hp = null;
        Vector2 saved = new Vector2(-100, -100);
        if (target == "Team1")
        {
            for (int i = 0; i < gv.team1units.Count; i++)
            {
                if (gv.team1units[i].gameObject.tag == target)
                {
                    if (closestDistance > Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team1units[i].transform.position)))
                    {
                        closestDistance = Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team1units[i].transform.position));
                        saved = gv.team1units[i].transform.position;
                        hp = gv.team1units[i].gameObject.GetComponentInChildren<HPSystem>();
                        index = i;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < gv.team2units.Count; i++)
            {
                if (gv.team2units[i].gameObject.tag == target)
                {
                    if (closestDistance > Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team2units[i].transform.position)))
                    {
                        closestDistance = Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team2units[i].transform.position));
                        saved = gv.team2units[i].transform.position;
                        hp = gv.team2units[i].gameObject.GetComponentInChildren<HPSystem>();
                        index = i;
                    }
                }
            }
        }
        if (saved != new Vector2(-100, -100))
        {
            toMove = saved;
            enemy = hp;
        }
    }
}
