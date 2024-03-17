using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Meric : MonoBehaviour
{
    public GlobalVariables gv;
    public Vector2 toMove;
    public Rigidbody2D rb;
    public Animator anim;
    public int team;
    public Bow bow;

    public HPSystem enemy;

    public bool isAttacking;
    public bool isReloading;

    public LayerMask enemyLayerMask;
    public string Etag;
    public string target;
    public float damage;

    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("MericWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("MericWalk");
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
        target = (tag == "Team1") ? "Team1" : "Team2";
        Etag = (gameObject.tag == "Team1") ? "Team1" : "Team2";
        target = (gameObject.tag == "Team1") ? "Team1" : "Team2";
    }

    private void FixedUpdate()
    {
        Etag = (gameObject.tag == "Team1") ? "Team1" : "Team2";
        target = (gameObject.tag == "Team1") ? "Team1" : "Team2";

        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("MericWalk");
            isReloading = false;
            isAttacking = false;
            StopAllCoroutines();
            return;
        }

        Vector2 moveDirection = new Vector2(0, 0);
        float curSpeed = moveSpeed;

        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if ((target == "Team2" && (gv.team1 == 2 || toMove == (Vector2)gv.garrison1.position)) || (target == "Team1" && (gv.team2 == 2 || toMove == (Vector2)gv.garrison2.position)))
            {
                isAttacking = false;
                anim.Play("MericWalk");
                GetComponent<SpriteRenderer>().flipX = false;
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


        if (Vector2.Distance(transform.position, toMove) < 5f && (target == "Team1" && (gv.team2 == 2 || toMove == (Vector2)gv.garrison2.position)))
        {
            if (transform.position.x - toMove.x < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }

            if (!isAttacking && !isReloading)
            {
                StartCoroutine(AttackAnimation());
                return;
            }
            else if (isAttacking)
            {
                return;
            }
            return;
        }

        anim.Play("MericWalk");
        moveDirection = (toMove - (Vector2)transform.position).normalized;

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
        anim.Play("MericHeal");
        string facing = GetComponent<SpriteRenderer>().flipX ? "left" : "right";
        if(enemy) enemy.Heal(damage);
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;
        anim.Play("MericWalk");
        yield return new WaitForSeconds(2);
        isReloading = false;
    }
    // Update is called once per frame
    void Update()
    {
        AI();
    }

    public void AI()
    {
        findEnemy();
        if ((gv.team1 == 1 && (target == "Team2")))
        {
            toMove = gv.garrison1.transform.position;
            //move to formation
        }
        else if ((gv.team2 == 1 && (target == "Team1")))
        {
            toMove = gv.garrison2.transform.position;
        }
    }

    public void Attack(float dmg)
    {
        if (!enemy) return;

    }

    public void findEnemy()
    {
        float closestDistance = Mathf.Infinity;
        int index = -1;
        if((gv.team1units.Count <= 1 && tag == "Team1") || (gv.team2units.Count <= 1 && tag == "Team2")) { return; }
        HPSystem hp = (tag == "Team1") ? gv.team1units[1].gameObject.GetComponentInChildren<HPSystem>() : gv.team2units[1].gameObject.GetComponentInChildren<HPSystem>();
        Vector2 saved = new Vector2(-100, -100);
        string flyingTag = (tag == "Team1") ? "Team1Flying" : "Team2Flying";
        if (target == "Team1" && gv.team1units.Count > 1)
        {
            for (int i = 1; i < gv.team1units.Count; i++)
            {
                if (gv.team1units[i] && (gv.team1units[i].gameObject.tag == target || gv.team1units[i].gameObject.tag == flyingTag) && !gv.garrisonDetector1.IsTargetWithinRange(gv.team1units[i].GetComponentInChildren<HPSystem>().gameObject))
                {
                    if (gv.team1units[i].gameObject.GetComponentInChildren<HPSystem>() && (gv.team1units[i].gameObject.GetComponentInChildren<HPSystem>().maxHP - gv.team1units[i].gameObject.GetComponentInChildren<HPSystem>().currentHP > hp.maxHP - hp.currentHP) && !gv.team1units[i].gameObject.GetComponent<Meric>())
                    {
                        closestDistance = Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team1units[i].transform.position));
                        saved = gv.team1units[i].transform.position;
                        hp = gv.team1units[i].gameObject.GetComponentInChildren<HPSystem>();
                        index = i;
                    }
                }
            }
        }
        else if (gv.team2units.Count > 1)
        {
            for (int i = 1; i < gv.team2units.Count; i++)
            {
                if (gv.team2units[i].gameObject.GetComponentInChildren<HPSystem>() && (gv.team2units[i].gameObject.GetComponentInChildren<HPSystem>().maxHP - gv.team2units[i].gameObject.GetComponentInChildren<HPSystem>().currentHP > hp.maxHP - hp.currentHP) && !gv.team2units[i].gameObject.GetComponent<Meric>())
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
