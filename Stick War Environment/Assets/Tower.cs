using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TextMesh controllingText;

    public int control;

    public EnemyDetector detector;
    public GlobalVariables gv;
    public EnemyDetector detectorAttack;

    public float ticks;
    public float maxTicks;

    public float ticksResources;
    public float maxTicksResources;

    public GameObject spearton;
    public GameObject juggernaut;

    public GameObject giant;
    public GameObject enslaved_giant;

    public GameObject spawn;

    public string contesting = "null";
    // Update is called once per frame
    void Update()
    {
        controllingText.text = control.ToString();
        ticks += Time.deltaTime;
        ticksResources += Time.deltaTime;
        if (ticks > maxTicks)
        {
            if (detector.IsTagWithinRange("Team1") && !detector.IsTagWithinRange("Team2"))
            {
                control += 1;
            }
            else if (detector.IsTagWithinRange("Team2") && !detector.IsTagWithinRange("Team1"))
            {
                control -= 1;
            }


            if(control >= 100) {
                control = 100;

                if(ticksResources > maxTicksResources)
                {
                    gv.gold1 += 20;
                    ticksResources = 0;
                }
                

                if(contesting != "team1")
                {
                    contesting = "team1";
                    if (spawn)
                    {
                        spawn.GetComponentInChildren<HPSystem>().Damage(5000);
                        Destroy(spawn);
                    }
                }

                if (!spawn && gv.team1units.Count < 50)
                {
                    if(gv.towerSpawn1 == 1)
                    {
                        spawn = Instantiate(spearton, transform.position, Quaternion.identity);
                    }else if(gv.towerSpawn1 == 2) {
                        spawn = Instantiate(enslaved_giant, transform.position, Quaternion.identity);
                    }
                }

            }
            else if (control <= -100)
            {
                control = -100;

                if (ticksResources > maxTicksResources)
                {
                    gv.gold2 += 20;
                    ticksResources = 0;
                }
                
                if (contesting != "team2")
                {
                    contesting = "team2";
                    if (spawn)
                    {
                        spawn.GetComponentInChildren<HPSystem>().Damage(5000);
                        Destroy(spawn);
                    }
                }

                if (!spawn && gv.team2units.Count < 50)
                {
                    if (gv.towerSpawn2 == 1)
                    {
                        spawn = Instantiate(juggernaut, transform.position, Quaternion.identity);
                    }
                    else if (gv.towerSpawn2 == 2)
                    {
                        spawn = Instantiate(giant, transform.position, Quaternion.identity);
                    }
                }

            }
            else
            {
                contesting = "null";
            }
            ticks = 0;
        }

        if(gv.team1 == 4 && detectorAttack.IsTagWithinRange("Team2"))
        {
            gv.team1 = 3;
        }
        if (gv.team2 == 4 && detectorAttack.IsTagWithinRange("Team1"))
        {
            gv.team2 = 3;
        }
    }
}
