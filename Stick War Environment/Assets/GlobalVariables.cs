﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public int gold1;
    public int crystal1;
    public int gold2;
    public int crystal2;
    public int population1;
    public int population2;

    public List<Resource> mines = new List<Resource>();

    public int team1; //garrison all units = 1, defend = 2, defend tower = 3, attack = 4
    public int team1miners; //garrison all miners = 1, mine = 2, attack = 3

    public int team2; //garrison all units = 1, defend = 2, defend tower = 3, attack = 4
    public int team2miners; //garrison all miners = 1, mine = 2, attack = 3

    public Transform garrison1;
    public Transform garrison2;

    public Transform miner1pos;
    public Transform miner2pos;

    public List<GameObject> team1units;
    public List<GameObject> team2units;

    public Transform centerPoint1;
    public Transform centerPoint2;

    public float rowSpacing = 1.5f; // Spacing between rows
    public float columnSpacing = 1f; // Spacing between columns
    public int maxStickmenPerRow = 4; // Maximum stickmen per row

    public EnemyDetector team1detection;
    public EnemyDetector team2detection;

    public GameObject miner;
    public GameObject swordswrath;
    public GameObject archidon;
    public GameObject spearton;
    public GameObject magikill;
    public GameObject giant;
    public GameObject shadowrath;

    public HPSystem statue1;
    public HPSystem statue2;

    private void Update()
    {
        population2 = team2units.Count;
        population1 = team1units.Count;

        if (team1 == 2)
        {
            ArrangeStickmenTeam1();
        }

        if (team2 == 2)
        {
            ArrangeStickmenTeam2();
        }
        detection();
    }
    public void ResetLevel()
    {
        Debug.Log("reset");
        if (team1units.Count > 1)
        {
            for (int i = team1units.Count - 1; i > 0; i--)
            {
                Destroy(team1units[i]);
                team1units.RemoveAt(i);
            }
        }

        if (team2units.Count > 1)
        {
            for (int i = team2units.Count - 1; i > 0; i--)
            {
                Destroy(team2units[i]);
                team2units.RemoveAt(i);
            }
        }

        statue1.currentHP = statue1.maxHP;
        statue2.currentHP = statue2.maxHP;
        team1 = 2;
        team1miners = 2;
        team2 = 2;
        team2miners = 2;
        GameObject one = Instantiate(miner, garrison1.transform.position, Quaternion.identity);
        one.tag = "Team1";
        GameObject two = Instantiate(miner, garrison1.transform.position, Quaternion.identity);
        two.tag = "Team1";
        GameObject three = Instantiate(miner, garrison2.transform.position, Quaternion.identity);
        three.tag = "Team2";
        GameObject four = Instantiate(miner, garrison2.transform.position, Quaternion.identity);
        four.tag = "Team2";
    }

    void detection()
    {
        if (team1detection.IsTagWithinRange("Team2") && team1 == 2)
        {
            team1 = 3;
        }

        if (team2detection.IsTagWithinRange("Team1") && team2 == 2)
        {
            team2 = 3;
        }
    }
    public void setTeam1(int to)
    {
        team1 = to;
    }
    public void setTeam2(int to)
    {
        team2 = to;
    }

    public void setTeam1Miners(int to)
    {
        team1miners  = to;
    }
    public void setTeam2Miners(int to)
    {
        team1miners = to;
    }

    void ArrangeStickmenTeam1()
    {
        Dictionary<string, int> stickmenCounts = new Dictionary<string, int>(); // Dictionary to store counts of each stickman type
        Dictionary<string, int> stickmenColumns = new Dictionary<string, int>(); // Dictionary to store current column index for each stickman type

        foreach (GameObject stickman in team1units)
        {
            string stickmanType = ""; // Determine stickman type
            float xOffset = 0f; // Offset for each stickman type
            if (stickman.GetComponent<Swordswrath>())
            {
                stickmanType = "Swordswrath";
                xOffset = -1f; // Example offset for Swordswrath
            }
            if (stickman.GetComponent<Minion>())
            {
                stickmanType = "Swordswrath";
                xOffset = -1f; // Example offset for Swordswrath
            }
            else if (stickman.GetComponent<Archidon>())
            {
                stickmanType = "Archidon";
                xOffset = -5f; // Example offset for Archidon
            }
            else if (stickman.GetComponent<Shadowrath>())
            {
                stickmanType = "Shadowrath";
                xOffset = -3f; // Example offset for Archidon
            }
            else if (stickman.GetComponent<Magikill>())
            {
                stickmanType = "Magikill";
                xOffset = -4f; // Example offset for Archidon
            }
            else if (stickman.GetComponent<Spearton>())
            {
                stickmanType = "Spearton";
                xOffset = 0f; // Example offset for Spearton
            }
            else if (stickman.GetComponent<Giant>())
            {
                stickmanType = "Giant";
                xOffset = 3f; // Example offset for Giant
            }

            // If stickman type is not registered, initialize count and column index
            if (!stickmenCounts.ContainsKey(stickmanType))
            {
                stickmenCounts[stickmanType] = 0;
                stickmenColumns[stickmanType] = 0;
            }

            // Calculate position based on type, current column index, and offset
            Vector3 position = centerPoint1.position + new Vector3((stickmenColumns[stickmanType] * columnSpacing) + xOffset, stickmenCounts[stickmanType] * rowSpacing, 0);

            // Update stickman position
            if (stickman.GetComponent<Swordswrath>())
                stickman.GetComponent<Swordswrath>().toMove = position;
            else if (stickman.GetComponent<Archidon>())
                stickman.GetComponent<Archidon>().toMove = position;
            else if (stickman.GetComponent<Spearton>())
                stickman.GetComponent<Spearton>().toMove = position;
            else if (stickman.GetComponent<Giant>())
                stickman.GetComponent<Giant>().toMove = position;
            else if (stickman.GetComponent<Magikill>())
                stickman.GetComponent<Magikill>().toMove = position;
            else if (stickman.GetComponent<Shadowrath>())
                stickman.GetComponent<Shadowrath>().toMove = position;
            else if (stickman.GetComponent<Minion>())
                stickman.GetComponent<Minion>().toMove = position;
            // Increment counts and update column index
            stickmenCounts[stickmanType]++;
            if (stickmenCounts[stickmanType] >= maxStickmenPerRow)
            {
                stickmenColumns[stickmanType]++;
                stickmenCounts[stickmanType] = 0;
            }
        }
    }

    void ArrangeStickmenTeam2()
    {
        Dictionary<string, int> stickmenCounts = new Dictionary<string, int>(); // Dictionary to store counts of each stickman type
        Dictionary<string, int> stickmenColumns = new Dictionary<string, int>(); // Dictionary to store current column index for each stickman type

        foreach (GameObject stickman in team2units)
        {
            string stickmanType = ""; // Determine stickman type
            float xOffset = 0f; // Offset for each stickman type
            if (stickman.GetComponent<Swordswrath>())
            {
                stickmanType = "Swordswrath";
                xOffset = 1f; // Example offset for Swordswrath
            }
            else if (stickman.GetComponent<Archidon>())
            {
                stickmanType = "Archidon";
                xOffset = 5f; // Example offset for Archidon
            }
            else if (stickman.GetComponent<Magikill>())
            {
                stickmanType = "Magikill";
                xOffset = 4f; // Example offset for Archidon
            }
            else if (stickman.GetComponent<Shadowrath>())
            {
                stickmanType = "Shadowrath";
                xOffset = 3f; // Example offset for Archidon
            }
            else if (stickman.GetComponent<Spearton>())
            {
                stickmanType = "Spearton";
                xOffset = 0f;
            }
            else if (stickman.GetComponent<Giant>())
            {
                stickmanType = "Giant";
                xOffset = -3f; 
            }
            else if (stickman.GetComponent<Minion>())
            {
                stickmanType = "Swordswrath";
                xOffset = 1f; // Example offset for Swordswrath
            }

            if (!stickmenCounts.ContainsKey(stickmanType))
            {
                stickmenCounts[stickmanType] = 0;
                stickmenColumns[stickmanType] = 0;
            }

            Vector3 position = centerPoint2.position + new Vector3((stickmenColumns[stickmanType] * columnSpacing) + xOffset, stickmenCounts[stickmanType] * rowSpacing, 0);

            if (stickman.GetComponent<Swordswrath>())
                stickman.GetComponent<Swordswrath>().toMove = position;
            else if (stickman.GetComponent<Archidon>())
                stickman.GetComponent<Archidon>().toMove = position;
            else if (stickman.GetComponent<Spearton>())
                stickman.GetComponent<Spearton>().toMove = position;
            else if (stickman.GetComponent<Giant>())
                stickman.GetComponent<Giant>().toMove = position;
            else if (stickman.GetComponent<Magikill>())
                stickman.GetComponent<Magikill>().toMove = position;
            else if (stickman.GetComponent<Shadowrath>())
                stickman.GetComponent<Shadowrath>().toMove = position;
            else if (stickman.GetComponent<Minion>())
                stickman.GetComponent<Minion>().toMove = position;

            stickmenCounts[stickmanType]++;
            if (stickmenCounts[stickmanType] >= maxStickmenPerRow)
            {
                stickmenColumns[stickmanType]++;
                stickmenCounts[stickmanType] = 0;
            }
        }
    }

    public float playAction(int action, int team) 
    {
        int reward = 0;
        //0) ⬇️
        //1) garrison troops
        //2) defend
        //3) attack
        //4) miners garrison
        //5) miners mine

        //6) sword
        //7) archidon
        //8) spearton
        //9) magikill
        //10) giant
        //11) shadow

        if(team % 2 != 0)
        {
            if(action == 0)
            {
                setTeam1(1);
            }
            if (action == 1)
            {
                setTeam1(2);
            }
            if (action == 2)
            {
                setTeam1(3);
            }
            if (action == 3)
            {
                setTeam1Miners(1);
            }
            if (action == 4)
            {
                setTeam1Miners(2);
            }
            if (action == 5)
            {
                if(gold1 >= 150)
                {
                    gold1 -= 150;
                    summonTroop1("M");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 6)
            {
                if (gold1 >= 125)
                {
                    gold1 -= 125;
                    summonTroop1("SWORD");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 7)
            {
                if (gold1 >= 300)
                {
                    gold1 -= 300;
                    summonTroop1("A");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 8)
            {
                if (gold1 >= 300 && crystal1 >= 100)
                {
                    gold1 -= 300;
                    crystal1 -= 100;
                    summonTroop1("S");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 9)
            {
                if (gold1 >= 300 && crystal1 >= 300)
                {
                    gold1 -= 300;
                    crystal1 -= 300;
                    summonTroop1("MK");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 10)
            {
                if (gold1 >= 1500)
                {
                    gold1 -= 1500;
                    summonTroop1("G");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 11)
            {
                if (gold1 >= 450 && crystal1 >= 150)
                {
                    gold1 -= 450;
                    crystal1 -= 150;
                    summonTroop1("SHADOW");
                }
                else
                {
                    reward -= 10;
                }
            }
        }
        else
        {
            if (action == 0)
            {
                setTeam1(1);
            }
            if (action == 1)
            {
                setTeam2(2);
            }
            if (action == 2)
            {
                setTeam2(3);
            }
            if (action == 3)
            {
                setTeam2Miners(1);
            }
            if (action == 4)
            {
                setTeam2Miners(2);
            }
            if (action == 5)
            {
                if (gold2 >= 150)
                {
                    gold2 -= 150;
                    summonTroop2("M");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 6)
            {
                if (gold2 >= 125)
                {
                    gold2 -= 125;
                    summonTroop2("SWORD");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 7)
            {
                if (gold2 >= 300)
                {
                    gold2 -= 300;
                    summonTroop2("A");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 8)
            {
                if (gold2 >= 300 && crystal2 >= 100)
                {
                    gold2 -= 300;
                    crystal2 -= 100;
                    summonTroop2("S");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 9)
            {
                if (gold2 >= 300 && crystal2 >= 300)
                {
                    gold2 -= 300;
                    crystal2 -= 300;
                    summonTroop2("MK");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 10)
            {
                if (gold2 >= 1500)
                {
                    gold2 -= 1500;
                    summonTroop2("G");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 11)
            {
                if (gold2 >= 450 && crystal2 >= 150)
                {
                    gold2 -= 450;
                    crystal2 -= 150;
                    summonTroop2("SHADOW");
                }
                else
                {
                    reward -= 10;
                }
            }
        }


        return reward;
    }

    public void summonTroop1(string team1)
    {
        population1 = team1units.Count;
        if (population1 > 30) return;
        GameObject inst = null;
        if(team1 == "M")
        {
            inst = Instantiate(miner, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "SWORD")
        {
            inst = Instantiate(swordswrath, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "A")
        {
            inst = Instantiate(archidon, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "S")
        {
            inst = Instantiate(spearton, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "MK")
        {
            inst = Instantiate(magikill, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "G")
        {
            inst = Instantiate(giant, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "SHADOW")
        {
            inst = Instantiate(shadowrath, garrison1.transform.position, Quaternion.identity);
        }

        inst.tag = "Team1";
        //team1units.Add(inst);
        
    }

    public void summonTroop2(string team1)
    {
        population2 = team2units.Count;
        if (population2 > 30) return;
        GameObject inst = null;
        if (team1 == "M")
        {
            inst = Instantiate(miner, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "SWORD")
        {
            inst = Instantiate(swordswrath, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "A")
        {
            inst = Instantiate(archidon, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "S")
        {
            inst = Instantiate(spearton, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "MK")
        {
            inst = Instantiate(magikill, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "G")
        {
            inst = Instantiate(giant, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "SHADOW")
        {
            inst = Instantiate(shadowrath, garrison2.transform.position, Quaternion.identity);
        }

        inst.tag = "Team2";
       // team2units.Add(inst);

    }

}
