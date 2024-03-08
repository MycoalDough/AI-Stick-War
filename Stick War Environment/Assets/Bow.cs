using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public float launchForce;

    public void Shoot(string Etag, string facing)
    {
        GameObject _arrow = Instantiate(arrow, transform.position, Quaternion.identity);

        Rigidbody2D rb = _arrow.GetComponent<Rigidbody2D>();

        if (facing == "right")
        {
            rb.velocity = transform.right * launchForce;
        }
        else if (facing == "left")
        {
            _arrow.transform.localScale = new Vector2(-_arrow.transform.localScale.x, _arrow.transform.localScale.y);
            rb.velocity = -transform.right * launchForce;
        }

        rb.AddForce(transform.up * launchForce * 7);
        _arrow.GetComponent<ArchidonArrow>().Etag = Etag;
    }
}
