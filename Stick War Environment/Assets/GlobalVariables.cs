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

}
