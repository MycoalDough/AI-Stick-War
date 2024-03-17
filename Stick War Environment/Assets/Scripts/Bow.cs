using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public float launchForce;
    public GlobalVariables gv;

    public void Awake()
    {
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    public void Shoot(string Etag, Vector3 pos, string shooter)
    {
        GameObject _arrow = Instantiate(arrow, transform.position, Quaternion.identity);

        if(shooter == "Albowtross")
        {
            ArchidonArrow albowtross = _arrow.GetComponent<ArchidonArrow>();
            albowtross.moveTo = pos;
            _arrow.GetComponent<ArchidonArrow>().Etag = Etag;
            if (gv.blazingBolts)
            {
                albowtross.canFire = true;
                albowtross.fireParticle.SetActive(true);
            }
            return;
        }else if(shooter == "Archidon")
        {
            ArchidonArrow archidon = _arrow.GetComponent<ArchidonArrow>();
            archidon.moveTo = pos;
            _arrow.GetComponent<ArchidonArrow>().Etag = Etag;
            if (gv.fireArrows)
            {
                archidon.canFire = true;
                archidon.fireParticle.SetActive(true);
                gv.crystal1 -= 5;
            }
            return;
        }
        else if (shooter == "Eclipsor" || shooter == "Castle")
        {
            ArchidonArrow archidon = _arrow.GetComponent<ArchidonArrow>();
            archidon.moveTo = pos;
            _arrow.GetComponent<ArchidonArrow>().Etag = Etag;
            return;
        }
        else
        {
            ArchidonArrow a = _arrow.GetComponent<ArchidonArrow>();
            if (a == null && _arrow.GetComponent<Hellfist>())
            {
                Hellfist w = _arrow.GetComponent<Hellfist>();
                w.moveTo = pos;
                _arrow.GetComponent<Hellfist>().Etag = Etag;
                return;
            }
            else if (_arrow.GetComponent<Reaper>())
            {
                Reaper t = _arrow.GetComponent<Reaper>();
                t.moveTo = pos;
                _arrow.GetComponent<Reaper>().Etag = Etag;
                return;
            }
        }


        
    }
}
