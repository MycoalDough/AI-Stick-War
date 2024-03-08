using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public int gold1;
    public int crystal1;
    public int gold2;
    public int crystal2;

    public Resource[] mines;

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

    public float rowSpacing = 1f; // Spacing between rows
    public float columnSpacing = 1f; // Spacing between columns
    public int maxStickmenPerRow = 5; // Maximum stickmen per row

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
            else if (stickman.GetComponent<Archidon>())
            {
                stickmanType = "Archidon";
                xOffset = -5f; // Example offset for Archidon
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

            stickmenCounts[stickmanType]++;
            if (stickmenCounts[stickmanType] >= maxStickmenPerRow)
            {
                stickmenColumns[stickmanType]++;
                stickmenCounts[stickmanType] = 0;
            }
        }
    }


}
