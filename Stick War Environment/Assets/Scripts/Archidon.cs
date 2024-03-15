using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Archidon : MonoBehaviour
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
        anim.Play("ArchidonWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("ArchidonWalk");
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
        Etag = (gameObject.tag == "Team1") ? "Team2" : "Team1";
        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";
    }

    private void FixedUpdate()
    {
        Etag = (gameObject.tag == "Team1") ? "Team2" : "Team1";
        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";

        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("ArchidonIdle");
            isReloading = false;
            isAttacking = false;
            StopAllCoroutines();
            return;
        }

        Vector2 moveDirection = new Vector2(0, 0);
        float curSpeed = (isReloading ? moveSpeed / 3 : moveSpeed);

        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if ((target == "Team2" && (gv.team1 == 2 || toMove == (Vector2)gv.garrison1.position)) || (target == "Team1" && (gv.team2 == 2 || toMove == (Vector2)gv.garrison2.position)))
            {
                isAttacking = false;
                anim.Play("ArchidonIdle");
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


        if (Vector2.Distance(transform.position, toMove) < 10f && (((gv.team1 != 2 && gv.team1 != 1 && target == "Team2") || (gv.team2 != 2 && gv.team2 != 1 && target == "Team1"))))
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
        }

        if(!isReloading) anim.Play("ArchidonWalk");
        if (Vector2.Distance(transform.position, toMove) < 8f && (((gv.team1 != 2 && gv.team1 != 1 && target == "Team2") || (gv.team2 != 2 && gv.team2 != 1 && target == "Team1"))) && (isAttacking || isReloading))
        {
            moveDirection = ((Vector2)transform.position - toMove).normalized;
            rb.MovePosition(rb.position + moveDirection * curSpeed * Time.fixedDeltaTime);
            return;
        }
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
        anim.Play("ArchidonShoot");
        yield return new WaitForSeconds(1);
        string facing = GetComponent<SpriteRenderer>().flipX ? "left" : "right";
        bow.Shoot(Etag, toMove);
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;
        anim.Play("ArchidonReload");
        yield return new WaitForSeconds(1);
        isReloading = false;
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

    public void Attack(float dmg)
    {
        if (!enemy) return;

    }

    public void findEnemy()
    {
        float closestDistance = Mathf.Infinity;
        int index = -1;
        HPSystem hp = null;
        Vector2 saved = new Vector2(-100, -100);
        string flyingTag = (tag == "Team1") ? "Team2Flying" : "Team1Flying";
        if (target == "Team1")
        {
            for (int i = 0; i < gv.team1units.Count; i++)
            {
                if (gv.team1units[i] && (gv.team1units[i].gameObject.tag == target || gv.team1units[i].gameObject.tag == flyingTag) && !gv.garrisonDetector1.IsTargetWithinRange(gv.team1units[i].GetComponentInChildren<HPSystem>().gameObject))
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
                if (gv.team2units[i] && (gv.team2units[i].gameObject.tag == target || gv.team2units[i].gameObject.tag == flyingTag) && !gv.garrisonDetector2.IsTargetWithinRange(gv.team2units[i].GetComponentInChildren<HPSystem>().gameObject))
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
