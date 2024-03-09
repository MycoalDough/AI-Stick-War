using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagikillSpell : MonoBehaviour
{
    public EnemyDetector detector;
    public string Etag;
    public float damage;


    public void Awake()
    {
        StartCoroutine(deleteAfterTime());
    }

    void Update()
    {
        if (!detector) return;

        GameObject hit = detector.IsTagWithinRange(Etag);
        if (hit != null)
        {
            hit.GetComponentInChildren<HPSystem>().Damage(damage);
            hit.GetComponentInChildren<HPSystem>().Daze();
        }

        Destroy(detector);
    }

    IEnumerator deleteAfterTime()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
