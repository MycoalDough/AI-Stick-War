using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPool : MonoBehaviour
{
    public EnemyDetector detector;
    public string Etag;
    public float damage;

    public float time;
    public float ticks = .5f;
    void Update()
    {
        time += Time.deltaTime;

        if(time < ticks)
        {
            return;
        }

        time = 0;
        GameObject hit = detector.IsTagWithinRange(Etag);
        if (hit != null)
        {
            hit.GetComponentInChildren<HPSystem>().Damage(damage);
            hit.GetComponentInChildren<HPSystem>().poisonStacks = (Etag == "Team2") ? (5) : 999;
        }
    }
}
