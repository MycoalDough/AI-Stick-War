using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public List<Miner> queue = new List<Miner>();
    public string type; //GOLD, CRYSTAL
    public int durability; //50 mines max; scale changes with the #of mines * 2.3
    public int team; //1 or 2

    public Transform minerSpot1;
    public Transform minerSpot2;

    public Vector3 minerSpot() //return miner spot
    {
        if(team == 1)
        {
            if (queue.Count == 2)
            {
                return minerSpot2.position;
            }
            else
            {
                return minerSpot1.position;
            }
        }
        else
        {
            if (queue.Count == 2)
            {
                return minerSpot1.position;
            }
            else
            {
                return minerSpot2.position;
            }
        }
    }

    public bool flip(string _team)
    {
        if (team == 1)
        {
            if (queue.Count == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (queue.Count == 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
