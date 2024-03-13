using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Shadowrath : MonoBehaviour
{
    public GlobalVariables gv;
    public Vector2 toMove;
    public Rigidbody2D rb;
    public Animator anim;

    public SpriteRenderer afterImage;
    public string savedTeam;
    public HPSystem enemy;

    public bool isAttacking;

    public LayerMask enemyLayerMask;
    public string target;
    public float damage;

    public float moveSpeed;
    public bool foundTeam;

    public EnemyDetector left;
    public EnemyDetector right;

    public bool invisible;
    public bool canInvisible;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("ShadowrathWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("ShadowrathWalk");
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
        savedTeam = tag;
        target = (savedTeam == "Team1") ? "Team2" : "Team1";
    }
    private void FixedUpdate()
    {
        target = (savedTeam == "Team1") ? "Team2" : "Team1";
        if(!foundTeam && (savedTeam != "Team1" || savedTeam != "Team2")) { savedTeam = tag; foundTeam = true; }
        float ms = (invisible) ? moveSpeed / 3 : moveSpeed;
        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("ShadowrathIdle");
            StopAllCoroutines();

            tag = savedTeam;
            invisible = false;
            canInvisible = false;
            StartCoroutine(invisibleCooldown());
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            isAttacking = false;
            return;
        }
        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            if (((target == "Team2") && (toMove == (Vector2)gv.garrison1.position || gv.team1 == 2)) || ((target == "Team1") && (toMove == (Vector2)gv.garrison2.position || gv.team2 == 2)))
            {
                isAttacking = false;
                anim.Play("ShadowrathIdle");
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

        if (Vector2.Distance(transform.position, toMove) < 7f && Vector2.Distance(transform.position, toMove) > 5.3f && (((target == "Team2") && (toMove != (Vector2)gv.garrison1.position && gv.team1 != 2)) || ((target == "Team1") && (toMove != (Vector2)gv.garrison2.position && gv.team2 != 2))))
        {
            if (!invisible && canInvisible)
            {
                Invisible();
            }
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

        anim.Play("ShadowrathWalk");
        Vector2 moveDirection = (toMove - (Vector2)transform.position).normalized;
        moveDirection.y *= UnityEngine.Random.Range(0.1f, 0.3f);
        rb.MovePosition(rb.position + moveDirection * ms * Time.fixedDeltaTime);

        if (moveDirection.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            afterImage.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            afterImage.flipX = true;
        }
    }


    IEnumerator AttackAnimation()
    {
        isAttacking = true;
        anim.Play("ShadowrathAttack");
        yield return new WaitForSeconds(1/3f);
        if (invisible)
        {
            tag = savedTeam;
            invisible = false;
            canInvisible = false;
            StartCoroutine(invisibleCooldown());
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            Attack(damage * 3f);
            enemy.Daze();

        }
        else
        {
            Attack(damage);
        }
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
    IEnumerator invisibleCooldown()
    {
        yield return new WaitForSeconds(10);
        canInvisible = true;
    }
    public void Invisible()
    {
        tag = "Untagged";
        invisible = true;
        canInvisible = false;
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 10);
    }

    // Update is called once per frame
    void Update()
    {
        AI();
    }

    public void AI()
    {
        if ((gv.team1 == 1 && (target == "Team2")) || (gv.team2 == 1 && (target == "Team1")))
        {
            tag = savedTeam;
            invisible = false;
            canInvisible = false;
            StartCoroutine(invisibleCooldown());
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            isAttacking = false;
            if(tag == "Team1")
            {
                toMove = gv.garrison1.transform.position;
            }
            else
            {
                toMove = gv.garrison2.transform.position;
            }
            tag = savedTeam;
            //move to formation
        }
        else if ((gv.team1 == 2 && (target == "Team2")) || (gv.team2 == 2 && (target == "Team1")))
        {
            tag = savedTeam;

            invisible = false;
            canInvisible = false;
            StartCoroutine(invisibleCooldown());
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            isAttacking = false;
            
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
        HPSystem hp = null;
        Vector2 saved = new Vector2(-100, -100);
        if (target == "Team1")
        {
            for (int i = 0; i < gv.team1units.Count; i++)
            {
                if (gv.team1units[i] && gv.team1units[i].gameObject.tag == target && !gv.garrisonDetector1.IsTargetWithinRange(gv.team1units[i].GetComponentInChildren<HPSystem>().gameObject))
                {
                    if (closestDistance > Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team1units[i].transform.position)))
                    {
                        closestDistance = Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team1units[i].transform.position));
                        saved = gv.team1units[i].transform.position;
                        hp = gv.team1units[i].gameObject.GetComponentInChildren<HPSystem>();
                    }

                    if(gv.team1units[i].gameObject.GetComponent<Archidon>() || gv.team1units[i].gameObject.GetComponent<Magikill>())
                    {
                        closestDistance = Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team1units[i].transform.position));
                        saved = gv.team1units[i].transform.position;
                        hp = gv.team1units[i].gameObject.GetComponentInChildren<HPSystem>();

                        enemy = hp;
                        toMove = saved;

                        return;
                    }
                }
            }
        }
        else if (target == "Team2")
        {
            for (int i = 0; i < gv.team2units.Count; i++)
            {
                if (gv.team2units[i] && gv.team2units[i].gameObject.tag == target && !gv.garrisonDetector2.IsTargetWithinRange(gv.team2units[i].GetComponentInChildren<HPSystem>().gameObject))
                {
                    if (closestDistance > Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team2units[i].transform.position)))
                    {
                        closestDistance = Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.team2units[i].transform.position));
                        saved = gv.team2units[i].transform.position;
                        hp = gv.team2units[i].gameObject.GetComponentInChildren<HPSystem>();
                    }

                    if (gv.team2units[i].gameObject.GetComponent<Archidon>() || gv.team2units[i].gameObject.GetComponent<Magikill>())
                    {
                        saved = gv.team2units[i].transform.position;
                        hp = gv.team2units[i].gameObject.GetComponentInChildren<HPSystem>();

                        enemy = hp;
                        toMove = saved;

                        return;
                    }
                }
            }
        }
        if (saved != new Vector2(-100, -100))
        {
            enemy = hp;
            toMove = saved;
        }
    }
}
