using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearton : MonoBehaviour
{
    public GlobalVariables gv;
    public Vector2 toMove;
    public Rigidbody2D rb;
    public Animator anim;
    public int team;

    public HPSystem enemy;

    public bool isAttacking;
    public bool defence;

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
        anim.Play("SpeartonWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("SpeartonWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();

        StartCoroutine(teamAdd());
        fluctuation();
    }

    public void fluctuation()
    {
        float fluc = UnityEngine.Random.Range(-0.05f, 0.05f);
        transform.localScale = new Vector2(transform.localScale.x + fluc, transform.localScale.y + fluc);
        moveSpeed = moveSpeed + UnityEngine.Random.Range(-0.1f, 0.1f);
    }
    IEnumerator teamAdd()
    {
        yield return new WaitForEndOfFrame();
        if (tag == "Team1")
        {
            gv.team1units.Add(gameObject);
        }
        else
        {
            gv.team2units.Add(gameObject);
        }
        target = (tag == "Team1") ? "Team2" : "Team1";
    }

    private void FixedUpdate()
    {
        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";

        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            defence = false;
            isAttacking = false;
            StopAllCoroutines();
            anim.Play("SpeartonIdle");

            return;
        }

        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if (((target == "Team2") && (toMove == (Vector2)gv.garrison1.position || gv.team1 == 2)) || ((target == "Team1") && (toMove == (Vector2)gv.garrison2.position || gv.team2 == 2)))
            {
                isAttacking = false;
                anim.Play("SpeartonIdle");

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

        if (Vector2.Distance(transform.position, toMove) < 5f && (((target == "Team2") && (toMove != (Vector2)gv.garrison1.position && gv.team1 != 2)) || ((target == "Team1") && (toMove != (Vector2)gv.garrison2.position && gv.team2 != 2))))
        {
            anim.Play("SpeartonShield");
            defence = true;
        }else if(Vector2.Distance(transform.position, toMove) > 5f)
        {
            defence = false;
        }


        if (Vector2.Distance(transform.position, toMove) < 1.2f && (((target == "Team2") && (toMove != (Vector2)gv.garrison1.position && gv.team1 != 2)) || ((target == "Team1") && (toMove != (Vector2)gv.garrison2.position && gv.team2 != 2))))
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

        if (!defence) anim.Play("SpeartonWalk");
        Vector2 moveDirection = (toMove - (Vector2)transform.position).normalized;
        moveDirection.y *= UnityEngine.Random.Range(0.2f, 0.7f);
        float curSpeed = !defence ? moveSpeed : moveSpeed / 2;

        rb.MovePosition(rb.position + moveDirection * curSpeed * Time.fixedDeltaTime);

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
        anim.Play("SpeartonAttack");
        yield return new WaitForSeconds(2/3f);
        Attack(damage);
        yield return new WaitForSeconds(1 + 1/3f);
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
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
