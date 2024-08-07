using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchidonArrow : MonoBehaviour
{
    public EnemyDetector detector;
    public float yValue;
    public string Etag;
    public float damage;
    public Vector2 moveTo;

    public float speed;
    public Rigidbody2D rb;

    public bool canFire;
    public GameObject fireParticle;

    public float bonusLight;
    public float bonusArmored;

    public bool canPoison;

    public bool isCastleArcherArrow = false;
    public GlobalVariables gv;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }
    void Update()
    {
        if (Vector2.Distance(moveTo, transform.position) < 0.05f)
        {
            gameObject.AddComponent<Remove>();
            Destroy(this);
        }

        if (Mathf.Abs(transform.position.x) > 1000)
        {
            Destroy(gameObject);
        }

        Vector2 moveDirection = (moveTo - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);

        if (!isCastleArcherArrow)
        {

            GameObject hit = detector.IsTagWithinRange(Etag);
            if (hit != null)
            {
                if (canPoison)
                    hit.GetComponentInChildren<HPSystem>().poisonStacks = (hit.GetComponentInChildren<HPSystem>().isChaos) ? 5 : 999;


                hit.GetComponentInChildren<HPSystem>().Damage(damage);
                if (canFire)
                {
                    hit.GetComponentInChildren<HPSystem>().fireStacks = 8;
                }

                if (bonusArmored > 0 && hit.GetComponentInChildren<HPSystem>().isArmoured)
                {
                    hit.GetComponentInChildren<HPSystem>().Damage(bonusArmored);
                }
                else if (bonusLight > 0 && !hit.GetComponentInChildren<HPSystem>().isArmoured)
                {
                    hit.GetComponentInChildren<HPSystem>().Damage(bonusLight);
                }
                Destroy(gameObject);
            }

            List<GameObject> hitFlying = detector.IsTagWithinRangeList(Etag + "Flying", 1);
            if (hitFlying.Count != 0)
            {
                foreach (GameObject go in hitFlying)
                {
                    go.GetComponentInChildren<HPSystem>().Damage(damage);

                    if (canPoison)
                        go.GetComponentInChildren<HPSystem>().poisonStacks = (go.GetComponentInChildren<HPSystem>().isChaos) ? 5 : 999;

                    if (canFire)
                    {
                        go.GetComponentInChildren<HPSystem>().fireStacks = 8;
                    }

                    if (bonusArmored > 0 && go.GetComponentInChildren<HPSystem>().isArmoured)
                    {
                        go.GetComponentInChildren<HPSystem>().Damage(bonusArmored);
                    }
                    else if (bonusLight > 0 && !go.GetComponentInChildren<HPSystem>().isArmoured)
                    {
                        go.GetComponentInChildren<HPSystem>().Damage(bonusLight);
                    }

                }
                Destroy(gameObject);
            }

        }
        else
        {
            List<GameObject> hit = detector.IsTagWithinRangeList(Etag, 4);
            if (hit.Count != 0)
            {
                for (int i = 0; i < hit.Count; i++)
                {
                    if(i == 0)
                    {
                        hit[i].GetComponentInChildren<HPSystem>().Damage(damage*1.75f);
                    }
                    hit[i].GetComponentInChildren<HPSystem>().Damage(damage);

                    if (bonusArmored > 0 && hit[i].GetComponentInChildren<HPSystem>().isArmoured)
                    {
                        hit[i].GetComponentInChildren<HPSystem>().Damage(bonusArmored);
                    }
                    Destroy(gameObject);
                }
            }


            List<GameObject> hitFlying = detector.IsTagWithinRangeList(Etag + "Flying", 4);
            if (hitFlying.Count != 0)
            {
                for (int i = 0; i < hitFlying.Count; i++)
                {
                    if (i == 0)
                    {
                        hitFlying[i].GetComponentInChildren<HPSystem>().Damage(damage * 1.75f);
                    }
                    hitFlying[i].GetComponentInChildren<HPSystem>().Damage(damage);
                    hitFlying[i].GetComponentInChildren<HPSystem>().Daze();

                    if (bonusArmored > 0 && hitFlying[i].GetComponentInChildren<HPSystem>().isArmoured)
                    {
                        hitFlying[i].GetComponentInChildren<HPSystem>().Damage(bonusArmored);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}
