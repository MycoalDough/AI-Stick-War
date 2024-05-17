using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Medusa : MonoBehaviour
{
    public GlobalVariables gv;
    public Vector2 toMove;
    public Rigidbody2D rb;
    public Animator anim;
    public int team;

    public GameObject rocks;
    public Material petrified;
    public GameObject acid;

    public EnemyDetector acidD;

    public HPSystem enemy;

    public bool isAttacking; //hell fist
    public bool isReloading;

    public bool isAttackingReaper;
    public bool isReloadingReaper;

    public LayerMask enemyLayerMask;
    public string Etag;
    public string target;
    public float damage;

    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("Medusa");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("Medusa");
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

        StartCoroutine(Reload());
        StartCoroutine(ReloadReaper());
    }

    private void FixedUpdate()
    {
        Etag = (gameObject.tag == "Team1") ? "Team2" : "Team1";
        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";

        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("Medusa");
            isReloading = false;
            isAttacking = false;
            StopAllCoroutines();
            return;
        }

        Vector2 moveDirection = new Vector2(0, 0);
        float curSpeed = moveSpeed;

        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if ((target == "Team2" && (gv.team1 == 2 || gv.team1 == 4 || toMove == (Vector2)gv.garrison1.position)) || (target == "Team1" && (gv.team2 == 2 && gv.team2 == 4 || toMove == (Vector2)gv.garrison2.position)))
            {
                isAttacking = false;
                anim.Play("Medusa");
                GetComponent<SpriteRenderer>().flipX = false;
                return;
            }

        }
        else if (isAttacking)
        {
            return;
        }

        if (Vector2.Distance(transform.position, toMove) < 10f && (((gv.team1 != 2 && gv.team1 != 1 && gv.team1 != 4 && target == "Team2") || (gv.team2 != 2 && gv.team2 != 4 && gv.team2 != 1 && target == "Team1"))))
        {
            if (transform.position.x - toMove.x < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }

            if (!isAttacking && !isReloading && !isAttackingReaper && enemy)
            {
                StartCoroutine(AttackAnimation());
                return;
            }
            else if (isAttacking)
            {
                return;
            }

            if (!isAttackingReaper && !isReloadingReaper && !isAttacking && acidD.IsTagWithinRange(Etag))
            {
                StartCoroutine(AttackAnimationReaper());
                return;
            }
            else if (isAttackingReaper)
            {
                return;
            }
        }

        anim.Play("Medusa");
        if (Vector2.Distance(transform.position, toMove) < 6f && (((gv.team1 != 2 && gv.team1 != 1 && gv.team1 != 4 && target == "Team2") || (gv.team2 != 2 && gv.team2 != 1 && gv.team2 != 4 && target == "Team1"))) && ((isReloading) || isAttackingReaper))
        {
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
        if (!enemy) { yield return null; }
        isAttacking = true;
        anim.Play("Medusa");
        yield return new WaitForSeconds(1.5f);
        Instantiate(rocks, enemy.transform.parent.position, Quaternion.identity);
        if (enemy)
        {
            enemy.Damage(damage, true);
        }
        enemy.transform.parent.GetComponent<SpriteRenderer>().material = petrified;
        yield return new WaitForSeconds(2.5f);
        isAttacking = false;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(30);
        isReloading = false;
    }

    IEnumerator AttackAnimationReaper()
    {
        isAttackingReaper = true;
        anim.Play("Medusa");
        yield return new WaitForSeconds(.5f);
        string facing = GetComponent<SpriteRenderer>().flipX ? "left" : "right";
        GameObject a = Instantiate(acid, transform.position, Quaternion.identity);
        a.tag = Etag;
        yield return new WaitForSeconds(1.5f);
        isAttackingReaper = false;
        StartCoroutine(ReloadReaper());
    }

    IEnumerator ReloadReaper()
    {
        isReloadingReaper = true;
        yield return new WaitForSeconds(17);
        isReloadingReaper = false;
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
                if (gv.team1units[i] && (gv.team1units[i].gameObject.tag == target) && !gv.garrisonDetector1.IsTargetWithinRange(gv.team1units[i].GetComponentInChildren<HPSystem>().gameObject)
                    && gv.team1units[i].GetComponentInChildren<HPSystem>().maxHP < 500)
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
                if (gv.team2units[i] && (gv.team2units[i].gameObject.tag == target) && !gv.garrisonDetector2.IsTargetWithinRange(gv.team2units[i].GetComponentInChildren<HPSystem>().gameObject)
                    && gv.team2units[i].GetComponentInChildren<HPSystem>().maxHP < 500)
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
