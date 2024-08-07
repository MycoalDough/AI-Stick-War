using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Security.Cryptography;

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

    [Header("Test")]
    public bool team1AI;
    public bool team2AI;

    public Text gold1;
    public Text gold2;
    public Text crystal1;
    public Text crystal2;
    public Text pop1;
    public Text pop2;

    public Slider speed;
    public Text speedT;

    [Header("Gameplay")]
    public GlobalVariables gv;
    public GameObject team1controls;
    public GameObject team2controls;
    public float time = 0;
    public bool forcedAttack = false;
    //AI PLANNING:
    //INPUTS:  1) num bullets | 2) num real | 3) num fake | 4) red lives | 5) blue lives | 6) red items (list) | 7) blue items (list) | 8) gun damage | 9) next bullet (-1 if not aviable, 0 for fake, 1 for real)
    //OUTPUTS: 1) shoot self | 2) shoot other | 3) drink | 4) mag. glass | 5) cig | 6) knife | 7) cuffs
    private void Awake()
    {
        if (team2AI)
        {
            team1controls.SetActive(true);
        }else if (team1AI)
        {
            team2controls.SetActive(true);
        }
        else
        {
            team1controls.SetActive(false);
            team2controls.SetActive(false);
        }
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
        if (team1AI)
        {
            team = 1;
        }else if (team2AI)
        {
            team = 2;
        }
        float reward = gv.playAction(int.Parse(toPlay), team);

        if(time > 1800)
        {
            gv.team2 = 3;
            gv.team1 = 3;
            forcedAttack = true;
        }
        else
        {
            forcedAttack = false;
        }

        if(time > 2000)
        {
            string toSend1 = "";
            string done1 = "True";
            reward = -700;

            toSend1 += reward.ToString();
            toSend1 += ":";
            toSend1 += done1.ToString();
            return toSend1;
        }
        string toSend = "";
        string done = "False";

        if(team % 2 == 0)
        {
            if(gv.statue1.currentHP <= 1000)
            {
                float timeReward = (time < 1000) ? (1000 - time) : 0;
                reward += 500 + timeReward;
                done = "True";
            }else if(gv.statue2.currentHP <= 1000)
            {
                reward -= 500;
                done = "True";

            }
        }
        else
        {
            if (gv.statue1.currentHP < 1000)
            {
                reward -= 500;
                done = "True";

            }
            else if (gv.statue2.currentHP < 1000)
            {
                float timeReward = (time < 1000) ? (1000 - time) : 0;
                reward += 500 + timeReward;
                done = "True";

            }
        }

        if(time > 1500)
        {
            reward -= time / 100;
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

        time += Time.deltaTime;

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

                            if (team1AI)
                            {
                                toSend = sendInput(1);
                                byte[] dataToSend1 = Encoding.UTF8.GetBytes(toSend);
                                stream.Write(dataToSend1, 0, dataToSend1.Length);
                            }else if (team2AI)
                            {
                                toSend = sendInput(2);
                                byte[] dataToSend1 = Encoding.UTF8.GetBytes(toSend);
                                stream.Write(dataToSend1, 0, dataToSend1.Length);
                            }
                            else
                            {
                                if (team % 2 == 0) { toSend = sendInput(2); } else { toSend = sendInput(1); }
                                byte[] dataToSend = Encoding.UTF8.GetBytes(toSend);
                                stream.Write(dataToSend, 0, dataToSend.Length);
                            }
                        });
                    }
                    else if (message.Contains("play_step"))
                    {
                        string[] step = message.Split(':');
                        umtd.Enqueue(() => {

                            if (team1AI)
                            {
                                team = 1;
                                string a = "";
                                a += playStep(step[1].ToString());
                                a += ":";
                                a += sendInput(1);
                                a += ":";
                                a += speed.value.ToString();
                                a += ":";
                                a += gv.whichTeamDidIt(team);
                                byte[] dataToSend1 = Encoding.UTF8.GetBytes(a);
                                stream.Write(dataToSend1, 0, dataToSend1.Length);
                            }else if(team2AI)
                            {
                                team = 2;
                                string a = "";
                                a += playStep(step[1].ToString());
                                Debug.Log(step[1]);
                                a += ":";
                                a += sendInput(2);
                                a += ":";
                                a += speed.value.ToString();
                                a += ":";
                                a += gv.whichTeamDidIt(team);
                                byte[] dataToSend1 = Encoding.UTF8.GetBytes(a);
                                stream.Write(dataToSend1, 0, dataToSend1.Length);
                            }
                            else
                            {
                                string s = "";

                                //int playstep = (int.Parse(step[1]) + 1);

                                if (forcedAttack)
                                {
                                    s += playStep("2");
                                }
                                else
                                {
                                    s += playStep(step[1].ToString());
                                }
                                s += ":";
                                if (team % 2 == 0) { s += sendInput(2); } else { s += sendInput(1); }
                                s += ":";
                                s += speed.value.ToString();
                                s += ":";
                                s += forcedAttack ? "True" : "False";
                                team++;
                                byte[] dataToSend = Encoding.UTF8.GetBytes(s);
                                stream.Write(dataToSend, 0, dataToSend.Length);
                            }
                            
                        });
                    }
                    if (message == "reset")
                    {
                        umtd.Enqueue(() =>
                        {
                            time = 0;
                            gv.ResetLevel();
                            team = 1;
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

    public float normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
    public string sendInput(int team) //724, 720
    {

        string connected = ""; 
        List<string> saved = new List<string>();

        if(team == 1)
        {
            saved.Add((gv.gold1 / 5000).ToString());
            saved.Add((gv.crystal1 / 7000).ToString());
            saved.Add((gv.population2 / 30).ToString());
            saved.Add((((gv.statue1.currentHP-1000) / 1600)).ToString());
            saved.Add((((gv.statue2.currentHP-1000) / 1600)).ToString());

            saved.Add(Convert.ToInt32(gv.rageBUY).ToString());
            saved.Add(Convert.ToInt32(gv.canRage).ToString());
            saved.Add(Convert.ToInt32(gv.canFireArrows).ToString());
            saved.Add(Convert.ToInt32(gv.passive1).ToString());


            saved.Add(Convert.ToInt32(gv.rage).ToString());
            saved.Add(Convert.ToInt32(gv.shieldWall).ToString());
            saved.Add(Convert.ToInt32(gv.blazingBolts).ToString());
            saved.Add(Convert.ToInt32(gv.fireArrows).ToString());
            saved.Add((gv.giantUpgrade1/1.7).ToString());
            saved.Add((gv.numberOfCastles1 / 6).ToString());
            saved.Add((gv.numberOfCastles2 / 6).ToString());
            saved.Add((gv.giantUpgrade2 / 1.7).ToString());

            saved.Add(normalize(gv.tower.control, -100, 100).ToString());
            saved.Add(Math.Round((gv.tower.ticksResources/20), 2).ToString());
            saved.Add((gv.cdteam1 / gv.maxCooldown).ToString());

            //17

        }
        else
        {
            saved.Add((gv.gold2 / 5000).ToString());
            saved.Add((gv.crystal2 / 7000).ToString());
            saved.Add((gv.population2 / 30).ToString());
            saved.Add(((gv.statue1.currentHP / 600)).ToString());
            saved.Add(((gv.statue2.currentHP / 600)).ToString());

            saved.Add(Convert.ToInt32(gv.rage).ToString());
            saved.Add(Convert.ToInt32(gv.shieldWall).ToString());
            saved.Add(Convert.ToInt32(gv.blazingBolts).ToString());
            saved.Add(Convert.ToInt32(gv.fireArrows).ToString());
            saved.Add((gv.giantUpgrade1 / 1.7).ToString());
            saved.Add((gv.numberOfCastles1 / 6).ToString());
            saved.Add((gv.numberOfCastles2 / 6).ToString());
            saved.Add((gv.giantUpgrade2 / 1.7).ToString());

            saved.Add(normalize(gv.tower.control, -100, 100).ToString());
            saved.Add(Math.Round((gv.tower.ticksResources / 20), 2).ToString());
            saved.Add((gv.cdteam2 / gv.maxCooldown).ToString());


            //13
        }
        saved.Add(Math.Min(time / 1800, 1).ToString());
        saved.Add((gv.team1miners/2).ToString());
        saved.Add((gv.team2miners/2).ToString());
        saved.Add((gv.team1 / 4).ToString());
        saved.Add((gv.team2 / 4).ToString());

        for(int i = 0; i < gv.mines.Count; i++)
        {
            saved.Add((gv.mines[i].durability/500).ToString());
        }

        for(int i =  1; i < 30; i++)
        {
            //1) TYPE (1 MINER, 2 SWORD, 3 ARCHIDON, 4 SPEARTON, 5 MAGIKILL, 6 GIANT, 7 SHADOWRATH)
            //2) HEALTH
            //3) X
            //4) Y
            //5) POISON
            //6) FIRE
            //7) TEAM

            //7) IF NULL, SET ALL TO -2
            string unit = "";
            if (gv.team1units.Count > i) { unit = returnUnitType(gv.team1units[i]); }
            if (unit == "null" || gv.team1units.Count <= i)
            {
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");

                saved.Add("0");
                saved.Add("0");
                saved.Add("0.5");
            }
            else
            {
                saved.Add((float.Parse(unit) / 17).ToString());
                saved.Add((gv.team1units[i].GetComponentInChildren<HPSystem>().currentHP / 1800f).ToString());
                saved.Add((gv.team1units[i].GetComponentInChildren<HPSystem>().damage / 50f).ToString());
                saved.Add((gv.team1units[i].GetComponentInChildren<HPSystem>().extraDamageLight / 5f).ToString());
                saved.Add((gv.team1units[i].GetComponentInChildren<HPSystem>().extraDamageArmor / 25f).ToString());

                saved.Add(normalize(gv.team1units[i].transform.position.x, -44, 55).ToString());
                saved.Add((gv.team1units[i].GetComponentInChildren<HPSystem>().poisonStacks > 0) ? "1" : "0");
                saved.Add((gv.team1units[i].GetComponentInChildren<HPSystem>().fireStacks > 0) ? "1" : "0");
                saved.Add("0.5");
            }
        }
        for (int i = 1; i < 30; i++)
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
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");
                saved.Add("0");

                saved.Add("0");
                saved.Add("1");
            }
            else
            {
                saved.Add((float.Parse(unit) / 17).ToString());
                saved.Add((gv.team2units[i].GetComponentInChildren<HPSystem>().currentHP/ 1800f).ToString());
                saved.Add((gv.team2units[i].GetComponentInChildren<HPSystem>().damage / 50f).ToString());
                saved.Add((gv.team2units[i].GetComponentInChildren<HPSystem>().extraDamageLight / 5f).ToString());
                saved.Add((gv.team2units[i].GetComponentInChildren<HPSystem>().extraDamageArmor / 25f).ToString());

                saved.Add(normalize(gv.team2units[i].transform.position.x, -44, 55).ToString());
                saved.Add((gv.team2units[i].GetComponentInChildren<HPSystem>().poisonStacks > 0) ? "1" : "0");
                saved.Add((gv.team2units[i].GetComponentInChildren<HPSystem>().fireStacks > 0) ? "1" : "0");
                saved.Add("1");
            }
        }

        saved.Add((gv.castle1.Count / 7).ToString());
        saved.Add((gv.castle2.Count / 7).ToString());


        for (int i = 0; i < saved.Count - 1; i++)
        {
            connected += saved[i] + ",";
        }
        connected += saved[saved.Count - 1];
        return connected;
    } 

    public float returnUnitHP(string unit)
    {
        if(unit == "1")
        {
            return 18;
        }
        else if(unit == "2")
        {
            return 18; 
        }else if(unit == "3")
        {
            return 9;
        }
        else if (unit == "4")
        {
            return 54;
        }
        else if (unit == "5")
        {
            return 27;
        }
        else if (unit == "6")
        {
            return 126 * 1.7f;
        }
        else if (unit == "7")
        {
            return 36;
        }
        else if (unit == "8")
        {
            return 27;
        }
        else if (unit == "9")
        {
            return 5;
        }
        else if (unit == "10")
        {
            return 18;
        }
        else if (unit == "11")
        {
            return 9;
        }
        else if (unit == "12")
        {
            return 9;
        }
        else if (unit == "13")
        {
            return 9;
        }
        else if (unit == "14")
        {
            return 9;
        }
        else if (unit == "15")
        {
            return 9;
        }
        else if (unit == "16")
        {
            return 9;
        }
        else if (unit == "17")
        {
            return 9;
        }

        return 0;
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
        if (unit.GetComponent<Meric>())
        {
            return "8";
        }
        if (unit.GetComponentInChildren<Albowtross>())
        {
            return "9";
        }
        if (unit.GetComponent<Crawler>())
        {
            return "10";
        }
        if (unit.GetComponent<Bomber>())
        {
            return "11";
        }
        if (unit.GetComponent<Juggernaut>())
        {
            return "12";
        }
        if (unit.GetComponentInChildren<Eclipsor>())
        {
            return "13";
        }
        if (unit.GetComponent<Medusa>())
        {
            return "14";
        }
        if (unit.GetComponent<Marrowkai>())
        {
            return "15";
        }
        if (unit.GetComponent<Dead>())
        {
            return "16";
        }
        if (unit.GetComponent<EnslavedGiant>())
        {
            return "17";
        }
        if(unit.GetComponent<Minion>()) {
            return "0.5";
        }
        if (unit.name.Contains("Wall"))
        {
            return "1.5";
        }
        return "null";
    }


}