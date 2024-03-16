using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hellfist : MonoBehaviour
{
    public Vector2 moveTo;
    public string Etag;

    public float speed;
    public Rigidbody2D rb;

    public GameObject fist;
    public float time;
    public float maxTime;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Vector2.Distance(moveTo, transform.position) < 0.05f)
        {
            gameObject.AddComponent<Remove>();
            Destroy(gameObject);
        }

        Vector2 moveDirection = (moveTo - (Vector2)transform.position).normalized;

        rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);

        time += Time.deltaTime;

        if(time > maxTime)
        {
            time = 0;
            GameObject f = Instantiate(fist, transform.position, fist.transform.rotation);
            f.GetComponent<Fist>().Etag = Etag;
        }
    }
}
