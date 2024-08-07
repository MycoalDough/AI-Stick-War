using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CastleArchidon : MonoBehaviour
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

    public bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        target = (tag == "Team1") ? "Team2" : "Team1";
        Etag = (gameObject.tag == "Team1") ? "Team2" : "Team1";
    }

    private void FixedUpdate()
    {
        Etag = (gameObject.tag == "Team1") ? "Team2" : "Team1";
        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";

        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            if (isDead)
            {
                anim.Play("DeadIdle");
            }
            else
            {
                anim.Play("ArchidonIdle");
            }
            isReloading = false;
            isAttacking = false;
            StopAllCoroutines();
            return;
        }

        Vector2 moveDirection = new Vector2(0, 0);
        float curSpeed = (isReloading ? moveSpeed / 3 : moveSpeed);

        if (isAttacking)
        {
            Vector2 moveY = new Vector2(0, toMove.y - transform.position.y).normalized;
            float slowSpeed = moveSpeed;
            rb.MovePosition(rb.position + moveY * slowSpeed * Time.fixedDeltaTime);
            return;
        }
        else
        {
            isAttacking = false;
        }


        if (Vector2.Distance(transform.position, toMove) < 40f)
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
        if (isDead)
        {
            anim.Play("DeadAttack");
        }
        else
        {
            anim.Play("ArchidonShoot");
        }
        yield return new WaitForSeconds(1);
        string facing = GetComponent<SpriteRenderer>().flipX ? "left" : "right";
        bow.Shoot(Etag, toMove, "Castle");
        yield return new WaitForSeconds(8f);
        isAttacking = false;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (!isDead)
        {
            anim.Play("ArchidonReload");
        }
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
        findEnemy();
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
