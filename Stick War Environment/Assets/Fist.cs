using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{
    public EnemyDetector detector;
    public string Etag;
    public float damage;
    public Vector2 moveTo;

    void Update()
    {

        List<GameObject> hitFlying = detector.IsTagWithinRangeList(Etag, 5);
        if (hitFlying.Count != 0)
        {
            foreach (GameObject go in hitFlying)
            {
                go.GetComponentInChildren<HPSystem>().Damage(damage);
                go.GetComponentInChildren<HPSystem>().poisonStacks = (go.GetComponentInChildren<HPSystem>().isChaos) ? 5 : 999;
                go.GetComponentInChildren<HPSystem>().Daze();

            }
            Destroy(gameObject);
        }
    }
}
