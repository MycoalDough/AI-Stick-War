using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : MonoBehaviour
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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("GiantWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if (toMove == (Vector2)gv.centerPoint1.position || toMove == (Vector2)gv.garrison1.position)
            {
                isAttacking = false;
                anim.Play("GiantIdle");
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


        if (Vector2.Distance(transform.position, toMove) < 3f && toMove != (Vector2)gv.centerPoint1.position)
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

        anim.Play("GiantWalk");
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
        int randomAttack = Random.Range(0, 2);
        if(randomAttack == 0)
        {
            anim.Play("GiantAttackClub");
        }
        else
        {
            anim.Play("GiantAttackHuman");
        }
        yield return new WaitForSeconds(2);
        Attack(damage);

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        AI();
    }

    public void AI()
    {
        if (gv.team1 == 1 || gv.team1 == 2)
        {
            toMove = gv.garrison1.transform.position;
            //move to formation
        }
        else if (gv.team1 == 3)
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
            enemy.Daze();
            return true;
        }
        else if (!GetComponent<SpriteRenderer>().flipX && right.IsTargetWithinRange(enemy.gameObject))
        {
            enemy.Damage(dmg);
            enemy.Daze();
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
        if (saved != new Vector2(-100, -100))
        {
            toMove = saved;
            enemy = hp;
        }
    }
}
