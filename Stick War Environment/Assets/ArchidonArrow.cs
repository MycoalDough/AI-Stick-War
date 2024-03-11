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
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
            Destroy(gameObject);
        }

        if(Mathf.Abs(transform.position.x) > 1000) {
            Destroy(gameObject);
        }

        Vector2 moveDirection = (moveTo - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }
}
