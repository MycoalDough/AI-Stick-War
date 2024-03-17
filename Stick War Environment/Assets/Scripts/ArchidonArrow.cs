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

    public bool canPoison;

    public GlobalVariables gv;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }
    void Update()
    {
        if(Vector2.Distance(moveTo, transform.position) < 0.05f)
        {
            gameObject.AddComponent<Remove>();
            Destroy(this);
        }

        GameObject hit = detector.IsTagWithinRange(Etag);
        if (hit != null)
        {
            hit.GetComponentInChildren<HPSystem>().Damage(damage);
            if(canFire) {
                hit.GetComponentInChildren<HPSystem>().fireStacks = 8;
            }
            Destroy(gameObject);
        }
        
        List<GameObject> hitFlying = detector.IsTagWithinRangeList(Etag + "Flying", 1);
        if (hitFlying.Count != 0)
        {
            foreach (GameObject go in hitFlying)
            {
                go.GetComponentInChildren<HPSystem>().Damage(damage);

                if(canPoison)
                    go.GetComponentInChildren<HPSystem>().poisonStacks = (go.GetComponentInChildren<HPSystem>().isChaos) ? 5 : 999;

                if (canFire)
                {
                    go.GetComponentInChildren<HPSystem>().fireStacks = 8;
                }

            }
            Destroy(gameObject);
        }

        if(Mathf.Abs(transform.position.x) > 1000) {
            Destroy(gameObject);
        }

        Vector2 moveDirection = (moveTo - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);
    }
}
