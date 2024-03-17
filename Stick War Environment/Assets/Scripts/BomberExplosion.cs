using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


                // Use a HashSet to track unique enemies
                HashSet<GameObject> uniqueEnemies = new HashSet<GameObject>();
                Debug.Log(enemies.Count);
                for(int i = 0; i < enemies.Count; i++)
                {
                    // Check if the enemy is already in hitenemeies
                    if (!uniqueEnemies.Contains(enemies[i]))
                    {
                        Debug.Log(enemies[i].transform.position + " " + enemies[i].name);
                        enemies[i].GetComponent<HPSystem>().Daze();
                        enemies[i].GetComponent<HPSystem>().Damage(damage);

                        // Add the enemy to the uniqueEnemies HashSet
                        uniqueEnemies.Add(enemies[i]);
                    }
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
