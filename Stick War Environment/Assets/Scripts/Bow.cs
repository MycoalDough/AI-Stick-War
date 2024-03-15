using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public float launchForce;

    public void Shoot(string Etag, Vector3 pos)
    {
        GameObject _arrow = Instantiate(arrow, transform.position, Quaternion.identity);

        ArchidonArrow a = _arrow.GetComponent<ArchidonArrow>();
        a.moveTo = pos;
        _arrow.GetComponent<ArchidonArrow>().Etag = Etag;
    }
}
