using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Miner : MonoBehaviour
{
    public string type; //"crystal" / "gold"
    public GlobalVariables gv;
    public Vector2 toMove;
    public Resource myMine;
    public Rigidbody2D rb;

    public bool flip;
    public bool isMining;
    public Animator anim;
    public int team;

    public float moveSpeed;

    public int maxStorage;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("MinerWalk");
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
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.01f, 0.1f));
        if (tag == "Team1")
        {
            gv.team1units.Add(gameObject);
            type = "gold";
            int goldCount = 0;

            for (int i = 0; i < gv.team1units.Count; i++)
            {
                if (gv.team1units[i].GetComponent<Miner>() && gv.team1units[i].GetComponent<Miner>().type == "gold")
                {
                    goldCount++;
                }
                else if (gv.team1units[i].GetComponent<Miner>() && gv.team1units[i].GetComponent<Miner>().type == "crystal")
                {
                    goldCount -= 2; // Reset gold count after encountering a crystal miner
                }

                if (goldCount >= 2)
                {
                    type = "crystal"; // If two gold miners have been encountered, switch to "crystal"
                }
                else
                {
                    type = "gold"; // Otherwise, keep the type as "gold"
                }
            }
        }
        else
        {
            gv.team2units.Add(gameObject);
            type = "gold";
            int goldCount = 0;

            for (int i = 0; i < gv.team2units.Count; i++)
            {
                if (gv.team2units[i].GetComponent<Miner>() && gv.team2units[i].GetComponent<Miner>().type == "gold")
                {
                    goldCount++;
                }
                else if (gv.team2units[i].GetComponent<Miner>() && gv.team2units[i].GetComponent<Miner>().type == "crystal")
                {
                    goldCount -= 2; // Reset gold count after encountering a crystal miner
                }

                if (goldCount >= 2)
                {
                    type = "crystal"; // If two gold miners have been encountered, switch to "crystal"
                }
                else
                {
                    type = "gold"; // Otherwise, keep the type as "gold"
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if(gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("MinerIdle");
            return;
        }
        if (Vector2.Distance(transform.position, toMove) < 0.05f)
        {
            GetComponent<SpriteRenderer>().flipX = flip;
            return;
        }
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

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponentInChildren<HPSystem>() && gameObject.GetComponentInChildren<HPSystem>().dazed)
        {
            anim.Play("MinerIdle");
            return;
        }

        AI();
        if(tag == "Team1")
        {
            if (Vector2.Distance(transform.position, gv.miner1pos.position) < 0.1f)
            {
                if (type == "crystal")
                {
                    gv.crystal1 += maxStorage * 10;
                    maxStorage = 0;
                }
                else
                {
                    gv.gold1 += maxStorage * 15;
                    maxStorage = 0;
                }
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, gv.miner2pos.position) < 0.1f)
            {
                if (type == "crystal")
                {
                    gv.crystal2 += maxStorage * 10;
                    maxStorage = 0;
                }
                else
                {
                    gv.gold2 += maxStorage * 15;
                    maxStorage = 0;
                }
            }
        }
    }

    public void AI()
    {
        if(tag == "Team1")
        {
            if (gv.team1miners == 1)
            {
                isMining = false;
                toMove = gv.garrison1.transform.position;
                if (myMine)
                {
                    myMine.queue.Remove(this);
                    myMine = null;
                }
            }
            else if (gv.team1miners == 2)
            {
                Mine();
            }
            else
            {
                isMining = false;
                if (myMine)
                {
                    myMine.queue.Remove(this);
                    myMine = null;
                }
                //attack
            }
        }
        else
        {
            if (gv.team2miners == 1)
            {
                isMining = false;
                toMove = gv.garrison2.transform.position;
                if (myMine)
                {
                    myMine.queue.Remove(this);
                    myMine = null;
                }
            }
            else if (gv.team2miners == 2)
            {
                Mine();
            }
            else
            {
                isMining = false;
                if (myMine)
                {
                    myMine.queue.Remove(this);
                    myMine = null;
                }
                //attack
            }
        }
    }

    public void Mine()
    {
        if(tag == "Team1" && gv.team1miners == 2)
        {
            if (Vector2.Distance(transform.position, toMove) < 0.07f && (toMove != (Vector2)gv.garrison1.transform.position && toMove != (Vector2)gv.miner1pos.transform.position))
            {
                if (!isMining)
                {
                    StartCoroutine(MineAnimation());
                }

                if (maxStorage >= 4)
                {
                    anim.Play("MinerWalk");
                    isMining = false;
                    if (myMine != null)
                    {
                        myMine.queue.Remove(this);
                        myMine = null;
                    }

                    toMove = (tag == "Team1") ? gv.miner1pos.transform.position : gv.miner2pos.transform.position;
                }
            }
            else if (maxStorage < 4)
            {
                StopCoroutine(MineAnimation());
                findMine();
                isMining = false;
                anim.Play("MinerWalk");
            }

            if (maxStorage >= 4)
            {
                anim.Play("MinerWalk");
                isMining = false;
                if (myMine != null)
                {
                    myMine.queue.Remove(this);
                    myMine = null;
                }

                toMove = (tag == "Team1") ? gv.miner1pos.transform.position : gv.miner2pos.transform.position;
            }
        }
        else if(tag == "Team2" && gv.team2miners == 2)
        {
            if (Vector2.Distance(transform.position, toMove) < 0.07f && (toMove != (Vector2)gv.garrison2.transform.position && toMove != (Vector2)gv.miner2pos.transform.position))
            {
                if (!isMining)
                {
                    StartCoroutine(MineAnimation());
                }

                if (maxStorage >= 4)
                {
                    anim.Play("MinerWalk");
                    isMining = false;
                    if (myMine != null)
                    {
                        myMine.queue.Remove(this);
                        myMine = null;
                    }

                    toMove = (tag == "Team1") ? gv.miner1pos.transform.position : gv.miner2pos.transform.position;
                }
            }
            else if (maxStorage < 4)
            {
                StopCoroutine(MineAnimation());
                findMine();
                isMining = false;
                anim.Play("MinerWalk");
            }
            if (maxStorage >= 4)
            {
                anim.Play("MinerWalk");
                isMining = false;
                if (myMine != null)
                {
                    myMine.queue.Remove(this);
                    myMine = null;
                }

                toMove = (tag == "Team1") ? gv.miner1pos.transform.position : gv.miner2pos.transform.position;
            }
        }
    }

    private IEnumerator MineAnimation()
    {
        anim.Play("MinerMine");
        isMining = true;
        yield return new WaitForSeconds(5);
        if(myMine) myMine.GetComponent<Resource>().durability--;

        isMining = false;
        if(myMine && myMine.GetComponent<Resource>().durability <= 0)
        {
            findMine();
        }
        maxStorage++;
    }

    public void findMine()
    {
        if(myMine != null && myMine.GetComponent<Resource>().type == type)
        {
            return;
        }
        Resource closest = null;
        float closestDistance = Mathf.Infinity;
        int index = -1;
        for(int i = 0; i < gv.mines.Count; i++)
        {
            if (gv.mines[i].durability > 0 && gv.mines[i].type == type && gv.mines[i].queue.Count < 2)
            {
                if(closestDistance > Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.mines[i].transform.position)))
                {
                    closestDistance = Mathf.Abs(Vector2.Distance(gameObject.transform.position, gv.mines[i].transform.position));
                    closest = gv.mines[i];
                    index = i;
                }
            }
        }
        if(myMine) myMine.queue.Remove(this);
        myMine = closest;

        if (closest != null)
        {
            gv.mines[index].queue.Add(GetComponent<Miner>());
            toMove = gv.mines[index].minerSpot();
            flip = gv.mines[index].flip(tag);

        }
        else
        {
            if(tag == "Team1")
            {
                toMove = gv.miner2pos.position;
            }
            else
            {
                toMove = gv.miner1pos.position;
            }
        }
        anim.Play("MinerWalk");
    }
}
