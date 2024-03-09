using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magikill : MonoBehaviour
{
    public GlobalVariables gv;
    public Vector2 toMove;
    public Rigidbody2D rb;
    public Animator anim;
    public int team;
    public GameObject minions;
    public List<GameObject> minionsList;

    public GameObject spell;

    public HPSystem enemy;

    public bool isAttacking;
    public bool isAttackingReloading;

    public bool isSummoning;
    public bool isSummoningReloading;

    public LayerMask enemyLayerMask;
    public string Etag;
    public string target;
    public float damage;

    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("MagikillWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();

        Etag = (gameObject.tag == "Team1") ? "Team2" : "Team1";
        target = (gameObject.tag == "Team1") ? "Team2" : "Team1";
    }

    private void Awake()
    {
        StartCoroutine(ReloadSummon());
        rb = GetComponent<Rigidbody2D>();
        anim.Play("MagikillWalk");
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
        for (int i = 0; i < minionsList.Count; i++)
        {
            if (minionsList[i].GetComponentInChildren<HPSystem>() == null || minionsList[i].GetComponentInChildren<HPSystem>().currentHP <= 0)
            {
                minionsList.Remove(minionsList[i]);
            }
        }

        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("MagikillIdle");
            isSummoningReloading = false;
            isSummoning = false;
            isAttackingReloading = false;
            isAttacking = false;
            StopAllCoroutines();
            return;
        }

        if(minionsList.Count < 2 && !isSummoningReloading && !isSummoning)
        {
            StartCoroutine(summonAnimation());
        }

        Vector2 moveDirection = new Vector2(0, 0);
        float curSpeed = moveSpeed;

        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if ((target == "Team2" && (gv.team1 == 2 || toMove == (Vector2)gv.garrison1.position)) || (target == "Team1" && (gv.team2 == 2 || toMove == (Vector2)gv.garrison2.position)))
            {
                anim.Play("MagikillIdle");
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


        if (Vector2.Distance(transform.position, toMove) < 4f && (((gv.team1 != 2 && gv.team1 != 1 && target == "Team2") || (gv.team2 != 2 && gv.team2 != 1 && target == "Team1"))))
        {
            if (!isAttacking && !isAttackingReloading)
            {
                StartCoroutine(AttackAnimation());
                return;
            }
            else if (isAttacking)
            {
                return;
            }
        }

        if (!isAttacking && !isSummoning) anim.Play("MagikillWalk");
        if (Vector2.Distance(transform.position, toMove) < 4f && (((gv.team1 != 2 && gv.team1 != 1 && target == "Team2") || (gv.team2 != 2 && gv.team2 != 1 && target == "Team1"))))
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
        isAttacking = true;
        anim.Play("MagikillAttack");
        yield return new WaitForSeconds(1);
        string facing = GetComponent<SpriteRenderer>().flipX ? "left" : "right";
        Vector2 posOfAttack = !GetComponent<SpriteRenderer>().flipX ? new Vector2(transform.position.x + 4f, transform.position.y + -0.2f) : new Vector2(transform.position.x + -4f, transform.position.y + -0.2f);
        GameObject attack = Instantiate(spell, posOfAttack, Quaternion.identity);
        attack.GetComponent<MagikillSpell>().Etag = Etag;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        StartCoroutine(ReloadAttack());
    }

    IEnumerator summonAnimation()
    {
        isSummoning = true;
        anim.Play("MagikillAttack");
        yield return new WaitForSeconds(1);
        string facing = GetComponent<SpriteRenderer>().flipX ? "left" : "right";
        GameObject newMinion = Instantiate(minions, transform.position, Quaternion.identity);
        newMinion.SetActive(false);
        minionsList.Add(newMinion);
        newMinion.tag = tag;
        newMinion.SetActive(true);
        newMinion.GetComponent<Minion>().SetAbilities();
        yield return new WaitForSeconds(1f);
        isSummoning = false;
        StartCoroutine(ReloadSummon());
    }

    IEnumerator ReloadAttack()
    {
        isAttackingReloading = true;
        yield return new WaitForSeconds(5);
        isAttackingReloading = false;
    }

    IEnumerator ReloadSummon()
    {
        isSummoningReloading = true;
        yield return new WaitForSeconds(10);
        isSummoningReloading = false;
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
        }else if((gv.team2 == 1 && (target == "Team1")))
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
        if(target == "Team1")
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
