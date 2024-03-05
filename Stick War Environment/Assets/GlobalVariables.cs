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

    public float rowSpacing = 1f; // Spacing between rows
    public float columnSpacing = 1f; // Spacing between columns
    public int maxStickmenPerRow = 5; // Maximum stickmen per row

    private void Update()
    {
        if(team1 == 2)
        {
            ArrangeStickmen();
        }
    }
    void ArrangeStickmen()
    {
        int currentRow = 0;
        int currentColumn = 0;
        int stickmenCount = 0;

        foreach (GameObject stickman in team1units)
        {
            if (stickman.GetComponent<Swordswrath>())
            {
                stickman.GetComponent<Swordswrath>().toMove = centerPoint1.position + new Vector3(currentColumn * columnSpacing, currentRow * rowSpacing, 0);
                stickmenCount++;
            }
            //else if (stickman.GetComponent<Archidon>())
            //{
                // Place in the back row
            //    stickman.transform.position = new Vector3(currentColumn * columnSpacing, 0, 0);
            //}
            

            if (stickmenCount >= maxStickmenPerRow)
            {
                currentColumn++;
                currentRow = 0;
                stickmenCount = 0;
            }
            else
            {
                currentRow++;
            }
        }
    }

}
