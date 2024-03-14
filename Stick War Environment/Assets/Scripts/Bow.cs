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

        ArchidonArrow a = _arrow.GetComponent<ArchidonArrow>();

        if (facing == "right")
        {
            a.moveTo = new Vector2(transform.position.x + launchForce, transform.position.y);
        }
        else if (facing == "left")
        {
            a.moveTo = new Vector2(transform.position.x - launchForce, transform.position.y);
        }

        _arrow.GetComponent<ArchidonArrow>().Etag = Etag;
    }
}
