using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BomberExplosion : MonoBehaviour
{
    public EnemyDetector ed;
    public string enemyTag;
    public int damage;
    public ParticleSystem ps;

    private void Awake()
    {
        StartCoroutine(boom());
    }

    IEnumerator boom()
    {
        while (true)
        {
            if (enemyTag == "Team1" || enemyTag == "Team2")
            {
                ps.Play();
                List<GameObject> enemies = new List<GameObject>();
                enemies = ed.IsTagWithinRangeList(enemyTag, 5);

                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<HPSystem>().Daze();
                    enemy.GetComponent<HPSystem>().Damage(damage);
                }
                break;
            }
            StartCoroutine(destroy());
            yield return null; // Yielding null lets Unity continue to the next frame
        }
    }

    IEnumerator destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
