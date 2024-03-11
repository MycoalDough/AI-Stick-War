﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RLConnection : MonoBehaviour
{
    [Header("Socket")]
    private const string host = "127.0.0.1"; // localhost
    private const int port = 12345;
    TcpClient client;
    NetworkStream stream;
    private Thread receiveThread;
    private bool isRunning = true;
    static RLConnection instance;
    public bool play; //determines whether the ais can play; adds pauses
    public UnityMainThreadDispatcher umtd;
    public int team = 1;

    public Text gold1;
    public Text gold2;
    public Text crystal1;
    public Text crystal2;
    public Text pop1;
    public Text pop2;
    int dones = 0;

    public Slider speed;
    public Text speedT;

    [Header("Gameplay")]
    public GlobalVariables gv;
    //AI PLANNING:
    //INPUTS:  1) num bullets | 2) num real | 3) num fake | 4) red lives | 5) blue lives | 6) red items (list) | 7) blue items (list) | 8) gun damage | 9) next bullet (-1 if not aviable, 0 for fake, 1 for real)
    //OUTPUTS: 1) shoot self | 2) shoot other | 3) drink | 4) mag. glass | 5) cig | 6) knife | 7) cuffs
    private void Awake()
    {
        Application.targetFrameRate = 50;
        Time.timeScale = 3;
        gv.ResetLevel();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ConnectToServer();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Application.targetFrameRate = 50;
    }

    public string playStep(string toPlay)
    {
        float reward = gv.playAction(int.Parse(toPlay), team);
        string toSend = "";
        string done = "False";

        if(team % 2 == 0)
        {
            if(gv.statue1.currentHP <= 3000)
            {
                reward += 500;
                done = "True";
            }else if(gv.statue2.currentHP <= 3000)
            {
                reward -= 500;
                done = "True";

            }
        }
        else
        {
            if (gv.statue1.currentHP < 3000)
            {
                reward -= 500;
                done = "True";

            }
            else if (gv.statue2.currentHP < 3000)
            {
                reward += 500;
                done = "True";

            }
        }
        toSend += reward.ToString();
        toSend += ":";
        toSend += done.ToString();

        return toSend;
    }

    IEnumerator playAnimation(Animator anim, string animName)
    {
        anim.Play(animName);
        yield return null;
    }
    private void Update()
    {
        Time.timeScale = speed.value;
        speedT.text = speed.value.ToString();
        gold1.text = "Gold:" + gv.gold1.ToString();
        gold2.text = "Gold: " + gv.gold2.ToString();

        crystal1.text = "Crystal:" + gv.crystal1.ToString();
        crystal2.text = "Crystal: " + gv.crystal2.ToString();

        pop1.text = "Population:" + gv.population1.ToString();
        pop2.text = "Population: " + gv.population2.ToString();

    }
    public void ConnectToServer()
    {
        try
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();

            // Start the receive thread
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
        }
    }
    void ReceiveData()
    {
        Debug.Log("Thread started!");
        byte[] data = new byte[1024];
        while (isRunning)
        {
            try
            {
                int bytesRead = stream.Read(data, 0, data.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);
                    if (message == "get_state")
                    {
                        // Enqueue the getItems call to be executed on the main thread
                        umtd.Enqueue(() => {
                            string toSend = "";

                            if (team % 2  == 0) { toSend = sendInput(2); } else { toSend = sendInput(1); }
                            byte[] dataToSend = Encoding.UTF8.GetBytes(toSend);
                            stream.Write(dataToSend, 0, dataToSend.Length); 
                        });
                    }
                    else if (message.Contains("play_step"))
                    {
                        string[] step = message.Split(':');
                        umtd.Enqueue(() => {
                            string s = "";

                            //int playstep = (int.Parse(step[1]) + 1);
                            s += playStep(step[1].ToString());
                            s += ":";
                            
                            if (team % 2 == 0) { s += sendInput(2); } else { s += sendInput(1); }
                            s += ":";
                            s += speed.value.ToString();
                            team++;
                            byte[] dataToSend = Encoding.UTF8.GetBytes(s);
                            stream.Write(dataToSend, 0, dataToSend.Length);
                            
                        });
                    }
                    if (message == "reset")
                    {
                        umtd.Enqueue(() =>
                        {
                            gv.ResetLevel();
                        });
                        //gv.ResetLevel();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
            }
        }
    }

    public int boolToInt(bool b)
    {
        return b ? 1 : 0;
    }
    public string sendInput(int team) //429
    {

        string connected = ""; 
        List<string> saved = new List<string>();

        if(team == 1)
        {
            saved.Add(gv.gold1.ToString());
            saved.Add(gv.crystal1.ToString());
            saved.Add(gv.population1.ToString());
            saved.Add(gv.statue1.currentHP.ToString());
            saved.Add(gv.statue2.currentHP.ToString());
        }
        else
        {
            saved.Add(gv.gold2.ToString());
            saved.Add(gv.crystal2.ToString());
            saved.Add(gv.population2.ToString());
            saved.Add(gv.statue1.currentHP.ToString());
            saved.Add(gv.statue2.currentHP.ToString());
        }

        saved.Add(gv.team1miners.ToString());
        saved.Add(gv.team2miners.ToString());
        saved.Add(gv.team1.ToString());
        saved.Add(gv.team2.ToString());


        for(int i =  0; i < 30; i++)
        {
            //1) TYPE (1 MINER, 2 SWORD, 3 ARCHIDON, 4 SPEARTON, 5 MAGIKILL, 6 GIANT, 7 SHADOWRATH)
            //2) HEALTH
            //3) X
            //4) Y
            //5) Z
            //6) MINER: STORAGE
            //7) TEAM

            //7) IF NULL, SET ALL TO -2
            string unit = "";
            if (gv.team1units.Count > i) { unit = returnUnitType(gv.team1units[i]); }
            if (unit == "null" || gv.team1units.Count <= i)
            {
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("1");
            }
            else
            {
                saved.Add(unit);
                saved.Add(gv.team1units[i].GetComponentInChildren<HPSystem>().currentHP.ToString());
                saved.Add(gv.team1units[i].transform.position.x.ToString());
                saved.Add(gv.team1units[i].transform.position.y.ToString());
                saved.Add(gv.team1units[i].transform.position.z.ToString());
                if (unit == "1")
                {
                    saved.Add(gv.team1units[i].GetComponent<Miner>().maxStorage.ToString());
                }
                else
                {
                    saved.Add("-2");
                }
                saved.Add("1");
            }
        }
        for (int i = 0; i < 30; i++)
        {
            //1) TYPE (1 MINER, 2 SWORD, 3 ARCHIDON, 4 SPEARTON, 5 MAGIKILL, 6 GIANT, 7 SHADOWRATH)
            //2) HEALTH
            //3) X
            //4) Y
            //5) Z
            //6) MINER: STORAGE
            //7) TEAM

            //7) IF NULL, SET ALL TO -2
            string unit = "";
            if (gv.team2units.Count > i) { unit = returnUnitType(gv.team2units[i]); }
            if (unit == "null" || gv.team2units.Count <= i)
            {
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("-2");
                saved.Add("2");
            }
            else
            {
                saved.Add(unit);
                saved.Add(gv.team2units[i].GetComponentInChildren<HPSystem>().currentHP.ToString());
                saved.Add(gv.team2units[i].transform.position.x.ToString());
                saved.Add(gv.team2units[i].transform.position.y.ToString());
                saved.Add(gv.team2units[i].transform.position.z.ToString());
                if (unit == "1")
                {
                    saved.Add(gv.team2units[i].GetComponent<Miner>().maxStorage.ToString());
                } 
                else
                {
                    saved.Add("-2");
                }
                saved.Add("2");
            }
        }


        for (int i = 0; i < saved.Count - 1; i++)
        {
            connected += saved[i] + ",";
        }
        connected += saved[saved.Count - 1];
        return connected;
    } 


     
    public string returnUnitType(GameObject unit)
    {
        if(unit == null) { return "null"; }
        if (unit.GetComponent<Miner>())
        {
            return "1";
        }
        if (unit.GetComponent<Swordswrath>())
        {
            return "2";
        }
        if (unit.GetComponent<Archidon>())
        {
            return "3";
        }
        if (unit.GetComponent<Spearton>())
        {
            return "4";
        }
        if (unit.GetComponent<Magikill>())
        {
            return "5";
        }
        if (unit.GetComponent<Giant>())
        {
            return "6";
        }
        if (unit.GetComponent<Shadowrath>())
        {
            return "7";
        }
        return "null";
    }


}