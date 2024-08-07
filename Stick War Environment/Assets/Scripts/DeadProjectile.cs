using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadProjectile : MonoBehaviour
{
    public EnemyDetector detector;
    public string Etag;
    public float damage;
    public Vector2 moveTo;

    public float speed;
    public Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Vector2.Distance(moveTo, transform.position) < 0.05f)
        {
            gameObject.AddComponent<Remove>();
            Destroy(this);
        }

        List<GameObject> hit = detector.IsTagWithinRangeList(Etag, 1);
        if (hit.Count != 0)
        {
            foreach (GameObject go in hit)
            {
                go.GetComponentInChildren<HPSystem>().Damage(damage);
                go.GetComponentInChildren<HPSystem>().poisonStacks = (go.GetComponentInChildren<HPSystem>().isChaos) ? 5 : 999;
                if (go.GetComponentInChildren<HPSystem>().isArmoured)
                {
                    go.GetComponentInChildren<HPSystem>().Damage(5);
                }

            }
            Destroy(gameObject);
        }

        List<GameObject> hitFlying = detector.IsTagWithinRangeList(Etag + "Flying", 1);
        if (hitFlying.Count != 0)
        {
            foreach (GameObject go in hitFlying)
            {
                go.GetComponentInChildren<HPSystem>().Damage(damage);
                go.GetComponentInChildren<HPSystem>().poisonStacks = (go.GetComponentInChildren<HPSystem>().isChaos) ? 5 : 999;
                if (go.GetComponentInChildren<HPSystem>().isArmoured)
                {
                    go.GetComponentInChildren<HPSystem>().Damage(5);
                }

            }
            Destroy(gameObject);
        }

        if (Mathf.Abs(transform.position.x) > 1000)
        {
            Destroy(gameObject);
        }

        Vector2 moveDirection = (moveTo - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);
    }
}
