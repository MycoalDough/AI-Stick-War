using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public string type; //"crystal" / "gold"
    public GlobalVariables gv;
    public Vector2 toMove;
    public Resource myMine;
    public Rigidbody2D rb;

    public bool isMining;
    public Animator anim;
    public int team;

    public float moveSpeed;

    public int maxStorage;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim.Play("MinerWalk");
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
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
        if (Vector2.Distance(transform.position, gv.miner1pos.position) < 0.1f)
        {
            if(type == "crystal")
            {
                gv.crystal1 += maxStorage * 10;
                maxStorage = 0;
            }
            else
            {
                gv.gold1 += maxStorage * 20;
                maxStorage = 0;
            }
        }
    }

    public void AI()
    {
        if(gv.team1miners == 1)
        {
            isMining = false;
            toMove = gv.garrison1.transform.position;
            if (myMine)
            {
                myMine.queue.Remove(this);
                myMine = null;
            }
        }
        else if(gv.team1miners == 2)
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

    public void Mine()
    {
        if(Vector2.Distance(transform.position, toMove) < 0.07f && (toMove != (Vector2)gv.garrison1.transform.position && toMove != (Vector2)gv.miner1pos.transform.position))
        {
            if (!isMining)
            {
                StartCoroutine(MineAnimation());
            }

            if(maxStorage == 4)
            {
                anim.Play("MinerWalk");
                isMining = false;
                if (myMine != null)
                {
                    myMine.queue.Remove(this);
                    myMine = null;
                }

                toMove = gv.miner1pos.transform.position;
            }
        }
        else if(maxStorage < 4)
        {
            StopCoroutine(MineAnimation());
            findMine();
            isMining = false;
            anim.Play("MinerWalk");
        }
    }

    private IEnumerator MineAnimation()
    {
        anim.Play("MinerMine");
        isMining = true;
        yield return new WaitForSeconds(2);
        if(myMine) myMine.GetComponent<Resource>().durability--;

        isMining = false;
        Debug.Log("+whatever");
        if(myMine && myMine.GetComponent<Resource>().durability <= 0)
        {
            Destroy(myMine);
            findMine();
        }
        maxStorage++;
    }

    public void findMine()
    {
        if(myMine != null)
        {
            return;
        }
        Resource closest = null;
        float closestDistance = Mathf.Infinity;
        int index = -1;
        for(int i = 0; i < gv.mines.Length; i++)
        {
            if (gv.mines[i].type == type && gv.mines[i].queue.Count < 2)
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
        }
        anim.Play("MinerWalk");
    }
}
