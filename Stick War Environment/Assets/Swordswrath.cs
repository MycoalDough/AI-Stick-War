using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordswrath : MonoBehaviour
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

    public bool jumpAttacking;
    public bool canJumpAttack;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("SwordswrathWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if(toMove == (Vector2)gv.centerPoint1.position || toMove == (Vector2)gv.garrison1.position)
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

        if (Vector2.Distance(transform.position, toMove) < 5.8f && Vector2.Distance(transform.position, toMove) > 5.3f && toMove != (Vector2)gv.centerPoint1.position && toMove != (Vector2)gv.garrison1.position)
        {
            if (!jumpAttacking && canJumpAttack)
            {
                StartCoroutine(JumpAttack());
            }
        }


        if (Vector2.Distance(transform.position, toMove) < .7f && toMove != (Vector2)gv.centerPoint1.position && toMove != (Vector2)gv.garrison1.position)
        {
            if (!isAttacking && !jumpAttacking)
            {
                StartCoroutine(AttackAnimation());
                return;
            }
            else if (isAttacking)
            {
                return;
            }
        }

        if (!jumpAttacking) anim.Play("SwordswrathWalk");
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

    IEnumerator JumpAttack()
    {
        jumpAttacking = true;
        canJumpAttack = false;
        anim.Play("SwordswrathJumpAttack");
        yield return new WaitForSeconds(1f);
        if(Attack(damage * 2)) enemy.Daze();
        yield return new WaitForSeconds(0.7f);
        jumpAttacking = false;
        yield return new WaitForSeconds(5f);
        canJumpAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        AI();
    }

    public void AI()
    {
        if (gv.team1 == 1)
        {
            toMove = gv.garrison1.transform.position;
        }
        else if(gv.team1 == 3)
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
            if(!jumpAttacking) toMove = saved;
            enemy = hp;
        }
    }
}
