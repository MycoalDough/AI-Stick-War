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
        if(a == null && _arrow.GetComponent<Hellfist>())
        {
            Hellfist w = _arrow.GetComponent<Hellfist>();
            w.moveTo = pos;
            _arrow.GetComponent<Hellfist>().Etag = Etag;
            return;
        }else if (_arrow.GetComponent<Reaper>())
        {
            Reaper t = _arrow.GetComponent<Reaper>();
            t.moveTo = pos;
            _arrow.GetComponent<Reaper>().Etag = Etag;
            return;
        }
        a.moveTo = pos;
        _arrow.GetComponent<ArchidonArrow>().Etag = Etag;
    }
}
