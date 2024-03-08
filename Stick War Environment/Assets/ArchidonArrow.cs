using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchidonArrow : MonoBehaviour
{
    public EnemyDetector detector;
    public float yValue;
    public Rigidbody2D rb;
    public string Etag;
    public float damage;
    private void Awake()
    {
        yValue = transform.position.y - 0.11f;
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if(Mathf.Abs(transform.position.y - yValue) < 0.1f)
        {
            Destroy(rb);
            Destroy(this);
        }

        GameObject hit = detector.IsTagWithinRange(Etag);
        if (hit != null)
        {
            hit.GetComponentInChildren<HPSystem>().Damage(damage);
            Destroy(gameObject);
        }
    }
}
