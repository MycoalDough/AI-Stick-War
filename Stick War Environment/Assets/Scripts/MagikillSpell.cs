using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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


            List<GameObject> enemies = new List<GameObject>();
            enemies = detector.IsTagWithinRangeList(Etag, 5);

            foreach (GameObject e in enemies)
            {
                e.GetComponent<HPSystem>().Damage(damage);
                e.GetComponent<HPSystem>().Daze();
            }
        Destroy(detector);
    }

    IEnumerator deleteAfterTime()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
