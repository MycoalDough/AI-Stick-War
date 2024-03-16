using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Reaper : MonoBehaviour
{
    public EnemyDetector detector;
    public float yValue;
    public string Etag;
    public float damage;
    public Vector2 moveTo;

    public GameObject h;
    public GlobalVariables gv;

    public float speed;
    public Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }
    void Update()
    {
        if (h)
        {
            if (Etag == "Team1")
            {
                gv.setPos(h, gv.garrison2.transform.position);
            }
            else
            {
                gv.setPos(h, gv.garrison1.transform.position);
            }
        }
        if (!GetComponent<EnemyDetector>())
        {
            return;
        }
        if (Vector2.Distance(moveTo, transform.position) < 0.05f)
        {
            gameObject.AddComponent<Remove>();
            Destroy(this);
        }

        GameObject hit = detector.IsTagWithinRange(Etag);
        if (hit != null)
        {
            h = hit;
            if(Etag == "Team1")
            {
                gv.setPos(hit.transform.parent.gameObject, gv.statue2.transform.position);
            }
            else
            {
                gv.setPos(hit.transform.parent.gameObject, gv.statue1.transform.position);
            }
            
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<EnemyDetector>());
            Remove g = gameObject.AddComponent<Remove>();
        }

        if (Mathf.Abs(transform.position.x) > 1000)
        {
            Destroy(gameObject);
        }

        Vector2 moveDirection = (moveTo - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);
    }
}
