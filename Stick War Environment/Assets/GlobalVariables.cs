using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public int gold1;
    public int crystal1;
    public int gold2;
    public int crystal2;

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

    private void Update()
    {
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

    public void summonTroop1(string team1)
    {
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
        
    }

    public void summonTroop2(string team1)
    {
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

    }

}
