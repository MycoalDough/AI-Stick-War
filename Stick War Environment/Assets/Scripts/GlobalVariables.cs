using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariables : MonoBehaviour
{
    public int gold1;
    public int crystal1;
    public int gold2;
    public int crystal2;
    public int population1;
    public int population2;

    public float maxCooldown = 7f;
    public float cdteam1;
    public float cdteam2;

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

    public Transform towerPoint1;
    public Transform towerPoint2;

    public float rowSpacing = 1.5f; // Spacing between rows
    public float columnSpacing = 1f; // Spacing between columns
    public int maxStickmenPerRow = 4; // Maximum stickmen per row

    public EnemyDetector team1detection;
    public EnemyDetector team2detection;

    public GameObject miner;
    [Header("ORDER UNITS")]
    public GameObject swordswrath;
    public GameObject archidon;
    public GameObject spearton;
    public GameObject magikill;
    public GameObject enslaved_giant;
    public GameObject shadowrath;
    public GameObject meric;
    public GameObject albowtross;
    [Header("CHAOS UNITS")]
    public GameObject crawler;
    public GameObject bomber;
    public GameObject juggernaut;
    public GameObject marrowkai;
    public GameObject giant;
    public GameObject dead;
    public GameObject eclipsor;
    public GameObject medusa;

    public UnityMainThreadDispatcher umtd;

    public HPSystem statue1;
    public HPSystem statue2;

    public EnemyDetector garrisonDetector1;
    public EnemyDetector garrisonDetector2;

    public int garrisonedUnits1;
    public int garrisonedUnits2;

    public List<CastleArchidon> castle1;
    public List<CastleArchidon> castle2;



    public Transform castleSpawn1;
    public Transform castleSpawn2;

    public GameObject castleArchidon;
    public GameObject castleDead;

    public int _team1;
    public int _team2;

    public float _statue1;
    public float _statue2;

    public float _gold1;
    public float _gold2;
    public float _crystal1;
    public float _crystal2;

    public TextMeshProUGUI TEAM1;
    public TextMeshProUGUI TEAM2;

    public int numberOfCastles1;
    public int numberOfCastles2;


    [Header("Rage")]
    public bool rageBUY;
    public bool canRage;
    public bool rage;

    [Header("Fire Arrows")]
    public bool canFireArrows;
    public bool fireArrows;

    [Header("Shield Wall")]
    public bool shieldWall;


    [Header("BLAZING BOLTS")]
    public bool blazingBolts;

    [Header("GIANT UPGRADE")]
    public float giantUpgrade1 = 1;
    public float giantUpgrade2 = 1;

    [Header("PASSIVE INCOME")]
    public float passive;
    public float passiveMax = 20;
    public int passive1;
    public int passive2;

    [Header("CASTLE ARCHERS")]
    public int castle1ability;
    public int castle2ability;

    [Header("TOWER SPAWN")]
    public int towerSpawn1;
    public int towerSpawn2;

    public Tower tower;


    [Header("GAMEPLAY TEAM 1")]
    public Text r_team1;
    public Text fa_team1;
    public Text bb_team1;
    public Text sw_team1;
    public Text g_text1;
    public Text pi_text1;
    public Text ca_text1;
    public Text ts_text1;
    public Text ac_r_team1;
    public Text ac_fa_team1;

    [Header("WALLS")]
    public Transform wallSpawn1;
    public Transform wallSpawn2;
    public GameObject wall1;
    public GameObject wall2;
    public EnemyDetector insideteam1;
    public EnemyDetector insideteam2;
    //TOWER SPAWN
    public int buyTowerSpawn1()
    {
        ts_text1.text = "G:600 C:200 TOWER SPAWN 1";
        if (gold1 >= 600 && crystal1 >= 200 && towerSpawn1 == 0)
        {
            TEAM1.text = "TOWER SPAWN";
            towerSpawn1 = 1;
            gold1 -= 600;
            ts_text1.text = "G:2000 C:1000 TOWER SPAWN 2";
            crystal1 -= 200;
            return 0;
        }
        else if (gold1 >= 2000 && crystal1 >= 1000 && towerSpawn1 == 1)
        {
            TEAM1.text = "TOWER SPAWN 2";
            towerSpawn1 = 2;
            ts_text1.text = "TOWER SPAWN FINISHED";
            gold1 -= 2000;
            crystal1 -= 1000;
            return 0;
        }
        TEAM1.text = "CANNOT AFFORD TOWER SPAWN";
        return -100;

    }

    public int buyTowerSpawn2()
    {
        if (gold2 >= 600 && crystal2 >= 200 && towerSpawn2 == 0)
        {
            TEAM2.text = "TOWER SPAWN";
            towerSpawn2 = 1;
            gold2 -= 600;
            crystal2 -= 200;
            return 0;
        }
        else if (gold2 >= 2000 && crystal2 >= 1000 && towerSpawn2 == 1)
        {
            TEAM2.text = "TOWER SPAWN 2";
            towerSpawn2 = 2;
            gold2 -= 2000;
            crystal2 -= 1000;
            return 0;
        }
        TEAM2.text = "CANNOT AFFORD TOWER SPAWN";
        return -100;

    }


    // RAGE

    public RLConnection RLConnection;
    public IEnumerator RAGE()
    {
        TEAM1.text = "ATTEMPTED RAGE";
        if (rageBUY && canRage)
        {
            TEAM1.text = "RAGE";
            ac_r_team1.text = "RAGE CD";
            for (int i = 0; i < team2units.Count; i++)
            {
                if (team2units[i].GetComponent<Swordswrath>())
                {
                    if (team2units[i].GetComponentInChildren<HPSystem>().currentHP > 20)
                    {
                        team2units[i].GetComponentInChildren<HPSystem>().Damage(20);
                    }
                }
            }
            canRage = false;
            rage = true;

            yield return new WaitForSeconds(8);
            rage = false;
            yield return new WaitForSeconds(12);
            canRage = true;
            ac_r_team1.text = "RAGE";
        }

        yield return null;
    }

    public int rageCheck()
    {
        if ((rageBUY && canRage))
        {
            return 100;  
        }
        return -100;
    }
    public int buyRage()
    {
        if(gold1 >= 50 && crystal1 >= 50 && !rageBUY) {
            TEAM1.text = "RAGE BUY";
            rageBUY = true;
            canRage = true;
            gold1 -= 50;
            crystal1 -= 50;
            r_team1.text = "SOLD";
            ac_r_team1.text = "RAGE";
            return 0;
        }
        TEAM1.text = "CANNOT AFFORD RAGE";
        return -100;

    }


    // FIRE ARROWS
    public float FIRE_ARROWS()
    {
        TEAM1.text = "ATTEMPTED FIRE ARROWS";
        if (canFireArrows) {
            TEAM1.text = "FIRE ARROWS";
            fireArrows = !fireArrows;

            if(fireArrows)
            {
                ac_fa_team1.text = "FIRE ARROW ACTIVE";
            }
            else
            {
                ac_fa_team1.text = "FIRE ARROW NOT ACTIVE";
            }
            return 0;
        }
        return -100;
    }
    public int buyFire_Arrows()
    {
        if (gold1 >= 50 && crystal1 >= 100 && !canFireArrows)
        {
            TEAM1.text = "FIRE ARROW BUY";
            fa_team1.text = "FIRE ARROW SOLD";
            canFireArrows = true;
            gold1 -= 50;
            crystal1 -= 100;
            return 0;
        }
        TEAM1.text = "FIRE ARROW TOO EXPENSIVE";
        return -100;
    }

    //SHIELD WALL
    public int SHIELD_WALL()
    {
        TEAM1.text = "SHIELD WALL";
        shieldWall = !shieldWall;

        if(shieldWall)
        {
            sw_team1.text = "SHIELD WALL ACTIVE";
        }
        else
        {
            sw_team1.text = "SHIELD WALL NOT ACTIVE";
        }
        
        if(team2 == 3)
        {
            return 0;
        }
        else
        {
            return -100;
        }
    }

    //BLAZING BOLTS
    public int buyBlazing_Bolts()
    {
        if (gold1 >= 400 && !blazingBolts)
        {
            TEAM1.text = "BLAZING BOLTS BUY";
            blazingBolts = true;
            gold1 -= 400;
            bb_team1.text = "BLAZING BOLTS BOUGHT";
            return 0;
        }

        TEAM1.text = "ATTEMPTED BLAZING BOLTS BUY";
        return -100;
    }

    //GIANT UPGRADE
    public int buyGiant_Upgrade1()
    {
        g_text1.text = "G: 400 C:400 GIANT UP 1";
        if (gold1 >= 400 && crystal1 >= 400 && giantUpgrade1 == 1)
        {
            TEAM1.text = "GIANT 1";
            giantUpgrade1 = 1.3f;
            gold1 -= 400;
            g_text1.text = "G: 600 C:600 GIANT UP 2";
            crystal1 -= 400;
            return 0;
        }else if(gold1 >= 600 && crystal1 >= 600 && giantUpgrade1 == 1.3f)
        {
            TEAM1.text = "GIANT 2";
            g_text1.text = "GIANT UPGRADES FINISHED";
            giantUpgrade1 = 1.7f;
            gold1 -= 600;
            crystal1 -= 600;return 0;
        }

        TEAM1.text = "GIANT UPGRADE TOO EXPENSIVE";
        return -100;
    }

    public int buyGiant_Upgrade2()
    {
        if (gold2 >= 400 && crystal2 >= 400 && giantUpgrade2 == 1)
        {
            TEAM2.text = "GIANT 1";
            giantUpgrade2 = 1.3f;
            gold2 -= 400;
            crystal2 -= 400;
            return 0;
        }
        else if (gold2 >= 600 && crystal2 >= 600 && giantUpgrade2 == 1.3f)
        {
            TEAM2.text = "GIANT 2";
            giantUpgrade2 = 1.7f;
            gold2 -= 600;
            crystal2 -= 600;
            return 0;   
        }

        TEAM2.text = "GIANT UPGRADE TOO EXPESNIVE";
        return -100;
    }


    //PASSIVE INCOME
    public int buyPassive_Income1()
    {
        if(gold1 >= 150 && crystal1 >= 100 && passive1 != 15)
        {
            pi_text1.text = "G: 150 C:100 PASSIVE 1";
            if (passive1 == 5)
            {
                TEAM1.text = "PASSIVE 1";
                passive1 = 10;
                gold1 -= 150;
                pi_text1.text = "G: 150 C:100 PASSIVE 2";

                crystal1 -= 100;
                return 0;
            }
            else
            {
                TEAM1.text = "PASSIVE 2";
                pi_text1.text = "PASSIVE INCOME FINISHED";

                passive1 = 15;
                gold1 -= 150;
                crystal1 -= 100;
                return 0;
            }
        }

        TEAM1.text = "PASSIVE TOO EXPENSIVE";

        return -100;


    }
    public int buyPassive_Income2()
    {
        if (gold2 >= 150 && crystal2 >= 100 && passive2 != 15)
        {
            if (passive2 == 5)
            {
                TEAM2.text = "PASSIVE 1";
                passive2 = 10;
                gold2 -= 150;
                crystal2 -= 100;
                return 0;
            }
            else
            {
                TEAM2.text = "PASSIVE 2";
                passive2 = 15;
                gold2 -= 150;
                crystal2 -= 100;
                return 0;
            }
        }
        TEAM2.text = "PASSIVE TOO EXPENSIVE";
        return -100;


    }

    //CASTLE ARCHERS
    public int buyCastle_Archer1()
    {
        if (castle1ability == 3) { TEAM1.text = "CASTLE ARCHER FULL"; return -100; }
        ca_text1.text = "G:300 CASTLE ARCHER 1";
        if (gold1 >= 300 && castle1ability == 0)
        {
            gold1 -= 300;
            castle1ability++;
            ca_text1.text = "G:600 CASTLE ARCHER 2";
            TEAM1.text = "CASTLE ARCHER 1";
            return 0;
        }
        else if (gold1 >= 600 && castle1ability == 1)
        {
            gold1 -= 600;
            castle1ability++;
            ca_text1.text = "G:1000 CASTLE ARCHER 3";
            TEAM1.text = "CASTLE ARCHER 2";
            return 0;
        }
        else if (gold1 >= 1000 && castle1ability == 2)
        {
            gold1 -= 1000;
            castle1ability++;
            ca_text1.text = "CASTLE ARCHER FINISHED";
            TEAM1.text = "CASTLE ARCHER 3";
            return 0;
        }
        TEAM1.text = "CASTLE ARCHER COST";
        return -100;
    }
    public int buyCastle_Archer2()
    {
        if (castle2ability == 3) { TEAM2.text = "CASTLE ARCHER FULL"; return -100; }
        if (gold2 >= 300 && castle2ability == 0)
        {
            gold2 -= 300;
            TEAM2.text = "CASTLE ARCHER 1";
            castle2ability++;
            return 0;
        }
        else if (gold2 >= 600 && castle2ability == 1)
        {
            gold2 -= 600;
            castle2ability++;
            TEAM2.text = "CASTLE ARCHER 2";
            return 0;
        }
        else if (gold2 >= 2000 && castle2ability == 2)
        {
            gold2 -= 2000;
            castle2ability++;
            TEAM2.text = "CASTLE ARCHER 3";
            return 0;
        }

        TEAM2.text = "CASTLE ARCHER COST";
        return -100;
    }





    private void Update()
    {
        cdteam1 += Time.deltaTime;
        cdteam2 += Time.deltaTime;

        if(cdteam1 > maxCooldown) cdteam1 = maxCooldown;
        if(cdteam2 > maxCooldown) cdteam2 = maxCooldown;

        if (fireArrows && crystal1 < 5)
        {
            fireArrows = false;
        }
        population2 = team2units.Count;
        population1 = team1units.Count;

        if (team1 == 2)
        {
            ArrangeStickmenTeam1(centerPoint1);
        }
        else if(team1 == 1)
        {
            arrangeGiants1();
        }
        else if (team1 == 4)
        {
            ArrangeStickmenTeam1(towerPoint1);
        }

        if (team2 == 2)
        {
            ArrangeStickmenTeam2(centerPoint2);
        }
        else if (team2 == 1)
        {
            arrangeGiants2();
        }
        else if (team2 == 4)
        {
            ArrangeStickmenTeam2(towerPoint2);
        }
        detection();
        team1units.RemoveAll(item => item == null);
        team2units.RemoveAll(item => item == null);

        findGarrisonedUnits1();
        findGarrisonedUnits2();

        passive += Time.deltaTime;

        if(passive > passiveMax)
        {
            gold1 += passive1;
            gold2 += passive2;
            crystal1 += passive1;
            crystal2 += passive2;
            passive = 0;
        }

        if (team1miners == 1)
        {
            for(int i = 0; i < 6; i++)
            {
                mines[i].queue.Clear();
                mines[i].RemoveNullItems();
            }
        }

        if (team2miners == 1)
        {
            for (int i = 6; i < mines.Count; i++)
            {
                mines[i].queue.Clear();
                mines[i].RemoveNullItems();
            }
        }
    }
    public void findGarrisonedUnits1()
    {
        int s = 0;
        if(team1 == 1 || team1miners == 1)
        {
            for(int i = 0; i < team1units.Count; i++)
            {
                if(team1 == 1)
                {
                    if (!team1units[i].GetComponent<Miner>())
                    {
                        s++;
                    }
                }
                if(team1miners == 1)
                {
                    if (team1units[i].GetComponent<Miner>())
                    {
                        s++;
                    }
                }
            }
        }

        numberOfCastles1 = castle1ability + Math.Min((int)Mathf.Floor(s / 3),2);
        if (s >= 1) { numberOfCastles1++; }
        numberOfCastles1 = Math.Min(numberOfCastles1, 6);

        if (RLConnection.time > 1800)
        {
            numberOfCastles1 = 0;
        }

        while (numberOfCastles1 != castle1.Count) {
            if(numberOfCastles1 > castle1.Count)
            {
                CastleArchidon ca = Instantiate(castleArchidon.gameObject, new Vector2(castleSpawn1.position.x, castleSpawn1.position.y + UnityEngine.Random.Range(-1f, 1f)), quaternion.identity).GetComponent<CastleArchidon>();
                ca.tag = "Team1";
                castle1.Add(ca);
            }

            if(numberOfCastles1 < castle1.Count)
            {
                CastleArchidon ca = castle1[castle1.Count - 1];
                castle1.Remove(castle1[castle1.Count - 1]);
                Destroy(ca.gameObject);
            }
        }
    }
    public void findGarrisonedUnits2()
    {
        int s = 0;
        if (team2 == 1 || team2miners == 1)
        {
            for (int i = 0; i < team2units.Count; i++)
            {
                if (team2 == 1)
                {
                    if (!team2units[i].GetComponent<Miner>())
                    {
                        s++;
                    }
                }
                if (team2miners == 1)
                {
                    if (team2units[i].GetComponent<Miner>())
                    {
                        s++;
                    }
                }
            }
        }

        int numberOfCastles2 = castle2ability + Math.Min((int)Mathf.Floor(s / 3), 2);

        if (s >= 1) { numberOfCastles2++; }

        numberOfCastles2 = Math.Min(numberOfCastles2, 6);

        if (RLConnection.time > 1800)
        {
            numberOfCastles2 = 0;
        }

        while (numberOfCastles2 != castle2.Count)
        {
            if (numberOfCastles2 > castle2.Count)
            {
                CastleArchidon ca = Instantiate(castleDead.gameObject, new Vector2(castleSpawn2.position.x, castleSpawn2.position.y + UnityEngine.Random.Range(-1f, 1f)), quaternion.identity).GetComponent<CastleArchidon>();
                ca.tag = "Team2";
                castle2.Add(ca);
            }

            if (numberOfCastles2 < castle2.Count)
            {
                CastleArchidon ca = castle2[castle2.Count - 1];
                castle2.Remove(castle2[castle2.Count - 1]);
                Destroy(ca.gameObject);
            }
        }
    }
    private void FixedUpdate()
    {
        team1units.RemoveAll(item => item == null);
        team2units.RemoveAll(item => item == null);
    }
    public void LateUpdate()
    {
        team1units.RemoveAll(item => item == null);
        team2units.RemoveAll(item => item == null);
    }
    public void ResetLevel()
    {
        umtd.Enqueue(() =>
        {
            rageBUY = false;
            canRage = false;
            rage = false;

            canFireArrows = false;
            fireArrows = false;

            shieldWall = false;


            blazingBolts = false;

            giantUpgrade1 = 1;
            giantUpgrade2 = 1;

            passive = 0;
            passiveMax = 20;
            passive1 = 5;
            passive2 = 5;

            castle1ability = 0;
            castle2ability = 0;
            Debug.Log("reset");
            if (team1units.Count > 1)
            {
                for (int i = team1units.Count - 1; i > 0; i--)
                {
                    GameObject toRemove = team1units[i];
                    team1units.RemoveAt(i);
                    Destroy(toRemove);
                }
            }

            if (team2units.Count > 1)
            {
                for (int i = team2units.Count - 1; i > 0; i--)
                {
                    GameObject toRemove = team2units[i];
                    team2units.RemoveAt(i);
                    Destroy(toRemove);
                }
            }

            statue1.currentHP = statue1.maxHP;
            statue2.currentHP = statue2.maxHP;
            team1 = 2;
            team1miners = 2;
            team2 = 2;
            team2miners = 2;
            foreach (Resource resouce in mines)
            {
                resouce.durability = 500;
                resouce.queue.Clear();
                resouce.RemoveNullItems();
            }

            GameObject one = Instantiate(miner, garrison1.transform.position, Quaternion.identity);
            one.tag = "Team1";
            GameObject two = Instantiate(miner, garrison1.transform.position, Quaternion.identity);
            two.tag = "Team1";
            GameObject three = Instantiate(miner, garrison2.transform.position, Quaternion.identity);
            three.tag = "Team2";
            GameObject four = Instantiate(miner, garrison2.transform.position, Quaternion.identity);
            four.tag = "Team2";
            GameObject five = Instantiate(miner, garrison1.transform.position, Quaternion.identity);
            five.tag = "Team1";
            GameObject six = Instantiate(miner, garrison2.transform.position, Quaternion.identity);
            six.tag = "Team2";

            GameObject w1 = Instantiate(wall1, wallSpawn1.position, Quaternion.identity);
            GameObject w2 = Instantiate(wall2, wallSpawn2.position, Quaternion.identity);

            //ADD STARTING UNITS IF AN AGENT STARTS TO FAIL MORE THAN THE OTHER CONSISTANTLY!

            //GameObject arc = Instantiate(archidon, garrison1.transform.position, Quaternion.identity);
            //arc.tag = "Team1";
            //GameObject arc3 = Instantiate(spearton, garrison1.transform.position, Quaternion.identity);
            //arc3.tag = "Team1";

            //GameObject arc1 = Instantiate(juggernaut, garrison2.transform.position, Quaternion.identity);
            //arc1.tag = "Team2";
            //GameObject arc4 = Instantiate(dead, garrison2.transform.position, Quaternion.identity);
            //arc4.tag = "Team2";
            gold1 = 300;
            gold2 = 300;
            crystal1 = 0;
            crystal2 = 0;
            _statue1 = statue1.currentHP;
            _statue2 = statue2.currentHP;
            _team1 = team1units.Count;
            _team2 = team2units.Count;
            _gold1 = gold1;
            _gold2 = gold2;
            _crystal1 = crystal1;
            _crystal2 = crystal2;
            towerSpawn1 = 0;
            towerSpawn2 = 0;
            tower.control = 0;
            tower.ticks = 0;
            tower.ticksResources = 0;



            passive = 0;

        });
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
        team2miners = to;
    }
    void ArrangeStickmenTeam1(Transform point)
    {
        Dictionary<string, int> stickmenCounts = new Dictionary<string, int>(); // Dictionary to store counts of each stickman type
        Dictionary<string, int> stickmenColumns = new Dictionary<string, int>(); // Dictionary to store current column index for each stickman type

        foreach (GameObject stickman in team1units)
        {
            if (stickman != null)
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
                //CHAOS.
                else if (stickman.GetComponent<Juggernaut>())
                {
                    stickmanType = "Juggernaut";
                    xOffset = 2f; // Example offset for Archidon
                }
                else if (stickman.GetComponent<Bomber>())
                {
                    stickmanType = "Bomber";
                    xOffset = -4f; // Example offset for Archidon
                }
                else if (stickman.GetComponent<Crawler>())
                {
                    stickmanType = "Crawler";
                    xOffset = 1f; // Example offset for Spearton
                }
                else if (stickman.GetComponent<EnslavedGiant>())
                {
                    stickmanType = "EnslavedGiant";
                    xOffset = -6f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Dead>())
                {
                    stickmanType = "Dead";
                    xOffset = -4f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Meric>())
                {
                    stickmanType = "Meric";
                    xOffset = -2f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Albowtross>())
                {
                    stickmanType = "Albowtross";
                    xOffset = -4f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Eclipsor>())
                {
                    stickmanType = "Eclipsor";
                    xOffset = -4f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Marrowkai>())
                {
                    stickmanType = "Eclipsor";
                    xOffset = -5f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Medusa>())
                {
                    stickmanType = "Eclipsor";
                    xOffset = -8f; // Example offset for Giant
                }

                // If stickman type is not registered, initialize count and column index
                if (!stickmenCounts.ContainsKey(stickmanType))
                {
                    stickmenCounts[stickmanType] = 0;
                    stickmenColumns[stickmanType] = 0;
                }

                // Calculate position based on type, current column index, and offset
                Vector3 position = point.position + new Vector3((stickmenColumns[stickmanType] * columnSpacing) + xOffset, stickmenCounts[stickmanType] * rowSpacing, 0);

                // Update stickman position
                setPos(stickman, position);
                // Increment counts and update column index
                stickmenCounts[stickmanType]++;
                if (stickmenCounts[stickmanType] >= maxStickmenPerRow)
                {
                    stickmenColumns[stickmanType]++;
                    stickmenCounts[stickmanType] = 0;
                }
            }
        }
    }
    public void setPos(GameObject stickman, Vector3 position)
    {
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
        else if (stickman.GetComponent<EnslavedGiant>())
            stickman.GetComponent<EnslavedGiant>().toMove = position;
        else if (stickman.GetComponent<Juggernaut>())
            stickman.GetComponent<Juggernaut>().toMove = position;
        else if (stickman.GetComponent<Bomber>())
            stickman.GetComponent<Bomber>().toMove = position;
        else if (stickman.GetComponent<Crawler>())
            stickman.GetComponent<Crawler>().toMove = position;
        else if (stickman.GetComponent<Dead>())
            stickman.GetComponent<Dead>().toMove = position;
        else if (stickman.GetComponent<Albowtross>())
            stickman.GetComponent<Albowtross>().toMove = position + new Vector3(0, 6);
        else if (stickman.GetComponent<Eclipsor>())
            stickman.GetComponent<Eclipsor>().toMove = position + new Vector3(0, 6);
        else if (stickman.GetComponent<Meric>())
            stickman.GetComponent<Meric>().toMove = position;
        else if (stickman.GetComponent<Marrowkai>())
            stickman.GetComponent<Marrowkai>().toMove = position;
        else if (stickman.GetComponent<Medusa>())
            stickman.GetComponent<Medusa>().toMove = position;
    }
    void arrangeGiants1()
    {
        Dictionary<string, int> stickmenCounts = new Dictionary<string, int>(); // Dictionary to store counts of each stickman type
        Dictionary<string, int> stickmenColumns = new Dictionary<string, int>(); // Dictionary to store current column index for each stickman type

        foreach (GameObject stickman in team1units)
        {
            if (stickman != null)
            {
                string stickmanType = ""; // Determine stickman type
                float xOffset = 0f; // Offset for each stickman type
                if (stickman.GetComponent<Giant>())
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

                if (stickman.GetComponent<Giant>())
                    stickman.GetComponent<Giant>().toMove = position;
                // Increment counts and update column index
                stickmenCounts[stickmanType]++;
                if (stickmenCounts[stickmanType] >= maxStickmenPerRow)
                {
                    stickmenColumns[stickmanType]++;
                    stickmenCounts[stickmanType] = 0;
                }
            }
        }
    }

    void arrangeGiants2()
    {
        Dictionary<string, int> stickmenCounts = new Dictionary<string, int>(); // Dictionary to store counts of each stickman type
        Dictionary<string, int> stickmenColumns = new Dictionary<string, int>(); // Dictionary to store current column index for each stickman type

        foreach (GameObject stickman in team2units)
        {
            if (stickman != null)
            {
                string stickmanType = ""; // Determine stickman type
                float xOffset = 0f; // Offset for each stickman type
                if (stickman.GetComponent<Giant>())
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
                Vector3 position = centerPoint2.position + new Vector3((stickmenColumns[stickmanType] * columnSpacing) + xOffset, stickmenCounts[stickmanType] * rowSpacing, 0);

                if (stickman.GetComponent<Giant>())
                    stickman.GetComponent<Giant>().toMove = position;
                // Increment counts and update column index
                stickmenCounts[stickmanType]++;
                if (stickmenCounts[stickmanType] >= maxStickmenPerRow)
                {
                    stickmenColumns[stickmanType]++;
                    stickmenCounts[stickmanType] = 0;
                }
            }
        }
    }

    void ArrangeStickmenTeam2(Transform point)
    {
        Dictionary<string, int> stickmenCounts = new Dictionary<string, int>(); // Dictionary to store counts of each stickman type
        Dictionary<string, int> stickmenColumns = new Dictionary<string, int>(); // Dictionary to store current column index for each stickman type

        foreach (GameObject stickman in team2units)
        {
            if (stickman != null)
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

                else if (stickman.GetComponent<Juggernaut>())
                {
                    stickmanType = "Juggernaut";
                    xOffset = 1f; // Example offset for Archidon
                }
                else if (stickman.GetComponent<Bomber>())
                {
                    stickmanType = "Bomber";
                    xOffset = 4f; // Example offset for Archidon
                }
                else if (stickman.GetComponent<Crawler>())
                {
                    stickmanType = "Crawler";
                    xOffset = 2f; // Example offset for Spearton
                }
                else if (stickman.GetComponent<EnslavedGiant>())
                {
                    stickmanType = "EnslavedGiant";
                    xOffset = 6f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Dead>())
                {
                    stickmanType = "Dead";
                    xOffset = 4f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Meric>())
                {
                    stickmanType = "Meric";
                    xOffset = 2f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Albowtross>())
                {
                    stickmanType = "Albowtross";
                    xOffset = 4f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Eclipsor>())
                {
                    stickmanType = "Eclipsor";
                    xOffset = 4f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Marrowkai>())
                {
                    stickmanType = "Eclipsor";
                    xOffset = 5f; // Example offset for Giant
                }
                else if (stickman.GetComponent<Medusa>())
                {
                    stickmanType = "Eclipsor";
                    xOffset = 8f; // Example offset for Giant
                }

                if (!stickmenCounts.ContainsKey(stickmanType))
                {
                    stickmenCounts[stickmanType] = 0;
                    stickmenColumns[stickmanType] = 0;
                }

                Vector3 position = point.position + new Vector3((stickmenColumns[stickmanType] * columnSpacing) + xOffset, stickmenCounts[stickmanType] * rowSpacing, 0);

                setPos(stickman, position);
                stickmenCounts[stickmanType]++;
                if (stickmenCounts[stickmanType] >= maxStickmenPerRow)
                {
                    stickmenColumns[stickmanType]++;
                    stickmenCounts[stickmanType] = 0;
                }
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
        TEAM1.text = "";
        TEAM2.text = "";

        if(team % 2 != 0)
        {

            if(action == 0)
            {
                TEAM1.text = "GARRISON UNITS";

                if(team1 == 1)
                {
                    reward -= 50;
                }
                setTeam1(1);

                int numUnits = 0;
                foreach (GameObject unit in team1units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
            }
            else if (action == 1)
            {
                if (team1 == 2)
                {
                    reward -= 50;
                }
                setTeam1(2);
                TEAM1.text = "DEFEND UNITS";
                int numUnits = 0;
                foreach (GameObject unit in team1units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
            }
            else if (action == 2)
            {
                if (team1 == 3)
                {
                    reward -= 50;
                }
                setTeam1(3);
                TEAM1.text = "ATTACK UNITS";
                reward += 30;
                int numUnits = 0;
                foreach (GameObject unit in team1units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }

            }
            else if (action == 3)
            {
                if (team1miners == 1)
                {
                    reward -= 50;
                }
                setTeam1Miners(1);
                TEAM1.text = "GARRISON MINERS";
                int numUnits = 0;
                foreach (GameObject unit in team1units)
                {
                    if (unit.GetComponent<Miner>() != null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
                else
                {
                    if(insideteam1.IsTagWithinRange("Team2") != null || insideteam1.IsTagWithinRange("Team2Flying") != null)
                    {
                        reward += 50;
                    }
                }
            }
            else if (action == 4)
            {
                if (team1miners == 2)
                {
                    reward -= 50;
                }
                setTeam1Miners(2);
                TEAM1.text = "MINE";
                int numUnits = 0;
                foreach (GameObject unit in team1units)
                {
                    if (unit.GetComponent<Miner>() != null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
            }
            else if (action >= 5 && action <= 13)
            {
                if(team1units.Count >= 30)
                {
                    TEAM1.text = "MAX UNITS";
                    reward -= 100;
                }
                else if(cdteam1 < maxCooldown)
                {
                    TEAM1.text = "UNIT COOLDOWN";
                    reward -= 100;
                }
                else
                {
                    if (action == 5)
                    {
                        if (gold1 >= 150)
                        {
                            TEAM1.text = "MINER SUMMON";
                            gold1 -= 150;
                            summonTroop1("MINER");
                            reward += 400;
                        }
                        else
                        {
                            TEAM1.text = "CANNOT AFFORD MINER";
                            reward -= 100;
                        }

                        int numMiners = 0;
                        foreach (GameObject unit in team1units)
                        {
                            if (unit.GetComponent<Miner>() != null)
                            {
                                numMiners++;
                            }
                        }

                        if (numMiners > 12)
                        {
                            reward -= 500;
                        }
                    }
                    else if (action == 6)
                    {
                        if (gold1 >= 150)
                        {
                            reward += 150;
                            gold1 -= 150;
                            summonTroop1("SWORD");
                            TEAM1.text = "SWORD SUMMON";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD SWORD";
                        }
                    }
                    else if (action == 7)
                    {
                        if (gold1 >= 300)
                        {
                            reward += 300;
                            gold1 -= 300;
                            TEAM1.text = "ARCHIDON SUMMON";
                            summonTroop1("ARCHIDON");
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD ARCHIDON";
                        }
                    }
                    else if (action == 8)
                    {
                        if (gold1 >= 450 && crystal1 >= 100)
                        {
                            reward += 450;
                            gold1 -= 450;
                            crystal1 -= 100;
                            summonTroop1("SPEARTON");
                            TEAM1.text = "SUMMON SPEARTON";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD SPEARTON";
                        }
                    }
                    else if (action == 9)
                    {
                        if (gold1 >= 400 && crystal1 >= 400)
                        {
                            reward += 450;
                            gold1 -= 400;
                            crystal1 -= 400;
                            TEAM1.text = "MAGIKILL SUMMON";
                            summonTroop1("MAGIKILL");
                        }
                        else
                        {
                            reward -= 10;
                            TEAM1.text = "CANNOT AFFORD MAGIKILL";
                        }
                    }
                    else if (action == 10)
                    {
                        if (gold1 >= 1500)
                        {
                            reward += 500;
                            gold1 -= 1500;
                            summonTroop1("GIANT");
                            TEAM1.text = "GIANT SPAWN";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD GIANT";
                        }
                    }
                    else if (action == 11)
                    {
                        if (gold1 >= 450 && crystal1 >= 150)
                        {
                            reward += 450;
                            gold1 -= 450;
                            crystal1 -= 150;
                            TEAM1.text = "SHADOWRATH SPAWN";
                            summonTroop1("SHADOW");
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD SHADOW";
                        }
                    }
                    else if (action == 12)
                    {
                        if (gold1 >= 450 && crystal1 >= 200)
                        {
                            reward += 450;
                            gold1 -= 450;
                            crystal1 -= 200;
                            summonTroop1("ALBOWTROSS");
                            TEAM1.text = "ALBOWTROSS SPAWN";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD ALBOWTROSS";
                        }
                    }
                    else if (action == 13)
                    {
                        if (gold1 >= 300 && crystal1 >= 200)
                        {
                            reward += 400;
                            gold1 -= 300;
                            crystal1 -= 200;
                            TEAM1.text = "MERIC SUMMON";
                            summonTroop1("MERIC");
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD MERIC";
                        }
                    }
                }
            }
            else if (action == 14)
            {
                reward += buyRage();
            }
            else if (action == 15)
            {
                reward += rageCheck();
                StartCoroutine(RAGE());
            }
            else if (action == 16)
            {
                reward += buyFire_Arrows();
            }
            else if (action == 17)
            {
                reward += (int)FIRE_ARROWS();
            }
            else if (action == 18)
            {
                reward += (int)SHIELD_WALL();
            }
            else if (action == 19)
            {
                reward += buyBlazing_Bolts();
            }
            else if (action == 20)
            {
                reward += buyGiant_Upgrade1();
            }
            else if (action == 21)
            {
                reward += buyPassive_Income1();
            }
            else if (action == 22)
            {
                reward += buyCastle_Archer1();
            }
            else if (action == 23)
            {
                if (team1 == 4)
                {
                    reward -= 50;
                }
                setTeam1(4);
                TEAM1.text = "UNITS TO TOWER";
                int numUnits = 0;
                foreach (GameObject unit in team1units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
            }
            else if (action == 24)
            {
                reward += buyTowerSpawn1();
            }
            else
            {
                reward -= 10;
            }
        }
        else
        {
            if (action == 0)
            {
                if (team2 == 1)
                {
                    reward -= 50;
                }
                setTeam2(1);
                TEAM2.text = "GARRISON UNITS";
                int numUnits = 0;
                foreach (GameObject unit in team2units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
            }
            else if (action == 1)
            {
                if (team2 == 2)
                {
                    reward -= 50;
                }
                setTeam2(2);
                TEAM2.text = "DEFEND UNITS";
                int numUnits = 0;
                foreach (GameObject unit in team2units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }

            }
            else if (action == 2)
            {
                if (team2 == 3)
                {
                    reward -= 50;
                }
                setTeam2(3);
                TEAM2.text = "ATTACK UNITS";
                reward += 15;
                int numUnits = 0;
                foreach (GameObject unit in team2units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }

            }
            else if (action == 3)
            {
                if (team2miners == 1)
                {
                    reward -= 50;
                }
                setTeam2Miners(1);
                TEAM2.text = "MINERS GARRISON";
                int numUnits = 0;
                foreach (GameObject unit in team2units)
                {
                    if (unit.GetComponent<Miner>() != null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
                else
                {
                    if (insideteam2.IsTagWithinRange("Team1") != null || insideteam2.IsTagWithinRange("Team1Flying") != null)
                    {
                        reward += 50;
                    }
                }

            }
            else if (action == 4)
            {
                if (team2miners == 2)
                {
                    reward -= 50;
                }
                setTeam2Miners(2);
                TEAM2.text = "MINERS MINE";
                int numUnits = 0;
                foreach (GameObject unit in team2units)
                {
                    if (unit.GetComponent<Miner>() != null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }

            }
            else if (action >= 5 && action <= 13)
            {
                if(team2units.Count >= 30)
                {
                    reward -=100;
                    TEAM2.text = "MAX UNITS";
                }
                else if(cdteam2 < maxCooldown)
                {
                    TEAM2.text = "UNIT COOLDOWN";
                    reward -=100;
                }
                else
                {
                    if (action == 5)
                    {
                        if (gold2 >= 150)
                        {
                            gold2 -= 150;
                            summonTroop2("MINER");
                            reward += 400;
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD MINER";
                        }

                        int numMiners = 0;
                        foreach (GameObject unit in team2units)
                        {
                            if (unit.GetComponent<Miner>() != null)
                            {
                                numMiners++;
                            }
                        }

                        if (numMiners > 12)
                        {
                            reward -= 500;
                        }
                    }
                    if (action == 6)
                    {
                        if (gold2 >= 100)
                        {
                            gold2 -= 100;
                            reward += 40;
                            summonTroop2("CRAWLER");
                            TEAM2.text = "CRAWLER SUMMON";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD CRAWLER";
                        }
                    }
                    if (action == 7)
                    {
                        if (gold2 >= 75)
                        {
                            gold2 -= 75;
                            reward += 10;
                            summonTroop2("BOMBER");
                            TEAM2.text = "BOMBER SPAWN";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD BOMBER";
                        }
                    }
                    if (action == 8)
                    {
                        if (gold2 >= 450 && crystal2 >= 50)
                        {
                            reward += 450;
                            gold2 -= 450;
                            crystal2 -= 50;
                            summonTroop2("JUGGERNAUT");
                            TEAM2.text = "JUGGERNAUT SPAWN";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD JUGGER";
                        }
                    }
                    if (action == 9)
                    {
                        if (gold2 >= 300 && crystal2 >= 100)
                        {
                            reward += 300;
                            gold2 -= 300;
                            crystal2 -= 100;
                            summonTroop2("DEAD");
                            TEAM2.text = "DEAD SPAWN";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD DEAD";
                        }
                    }
                    if (action == 10)
                    {
                        if (gold2 >= 1500)
                        {
                            reward += 500;
                            gold2 -= 1500;
                            TEAM2.text = "GIANT SPAWN";
                            summonTroop2("GIANT");
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD GIANT";
                        }
                    }
                    if (action == 11)
                    {
                        if (gold2 >= 400 && crystal2 >= 100)
                        {
                            reward += 450;
                            gold2 -= 400;
                            crystal2 -= 100;
                            summonTroop2("ECLIPSOR");
                            TEAM2.text = "SUMMON ECLIPSOR";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD ECLIPSOR";
                        }
                    }
                    if (action == 12)
                    {
                        if (gold2 >= 400 && crystal2 >= 400)
                        {
                            reward += 450;
                            gold2 -= 400;
                            crystal2 -= 400;
                            summonTroop2("MARROWKAI");
                            TEAM2.text = "MARROWKAI SUMMON";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD MARROW";
                        }
                    }
                    if (action == 13)
                    {
                        if (gold2 >= 500 && crystal2 >= 400)
                        {
                            reward += 500;
                            gold2 -= 500;
                            crystal2 -= 400;
                            summonTroop2("MEDUSA");
                            TEAM2.text = "SPAWN MEDUSA";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM2.text = "CANNOT AFFORD MEDUSA";
                        }
                    }
                }
                
            }
            else if(action == 14)
            {
                reward += buyCastle_Archer2();
            }else if(action == 15)
            {
                reward += buyPassive_Income2();
            }else if(action == 16)
            {
                reward += buyGiant_Upgrade2();
            }
            else if (action == 17)
            {
                if (team2 == 4)
                {
                    reward -= 50;
                }
                setTeam2(4);
                TEAM2.text = "UNITS TO TOWER";
                int numUnits = 0;
                foreach (GameObject unit in team2units)
                {
                    if (unit.GetComponent<Miner>() == null && !unit.name.Contains("Statue") && !unit.name.Contains("Wall"))
                    {
                        numUnits++;
                    }
                }

                if (numUnits == 0)
                {
                    reward -= 300;
                }
            }
            else if (action == 18)
            {
                reward += buyTowerSpawn2();
            }
            else
            {
                reward -= 10;
            }
            TEAM2.text += (team + "," + action);
        }


        //reward 
        if(team % 2 != 0)
        {
            int numMiners = 0;
            foreach (GameObject unit in team1units)
            {
                if (unit.GetComponent<Miner>() != null)
                {
                    numMiners++;
                }
            }

            if (numMiners > 12 || numMiners <= 0)
            {
                reward -= 50;
            }

            if (statue1.currentHP < _statue1)
            {
                reward -= 55;
            }
            if(team1units.Count < _team1)
            {
                reward -= 50;
            }
            if(team2units.Count < _team2)
            {
                reward += 50;
            }
            if (statue2.currentHP < _statue2)
            {
                reward += 50;
            }
            if(gold1 > 3000)
            {
                reward -= 50;
            }
            if(tower.control == 100)
            {
                reward += 50;
            }
            if((team1 == 1 && team2 != 3) || (team1miners == 1 && team2 != 3))
            {
                reward -= 200;
            }

            if(_gold1 < gold1)
            {
                reward += 50;
                _gold1 = gold1;
            }
            if(_crystal1 < crystal1)
            {
                reward += 50;
                _crystal1 = crystal1;
            }

            if(gold1 >= 5000) { 
                reward -= 100; 
                gold1 = 5000; 
            }

            if (crystal1 >= 7000) {
                reward -= 100; 
                crystal1 = 7000; 
            }


        }
        if (team % 2 == 0)
        {
            int numMiners = 0;
            foreach (GameObject unit in team2units)
            {
                if (unit.GetComponent<Miner>() != null)
                {
                    numMiners++;
                }
            }

            if (numMiners > 12 || numMiners <= 0)
            {
                reward -= 50;
            }


            if (gold2 >= 5000)
            {
                reward -= 200;
                gold2 = 5000;
            }

            if (crystal2 >= 7000)
            {
                reward -= 200;
                crystal2 = 7000;

            }
            if (statue1.currentHP < _statue1)
            {
                reward += 50;
            }
            if (team1units.Count < _team1)
            {
                reward += 50;
            }
            if (team2units.Count < _team2)
            {
                reward -= 50;
            }
            if (statue2.currentHP < _statue2)
            {
                reward -= 50;
            }
            if (gold2 > 3000)
            {
                reward -= 50;
            }
            if (tower.control == -100)
            {
                reward += 50;
            }
            if ((team2 == 1 && team1 != 3) || (team2miners == 1 && team1 != 3))
            {
                reward -= 200;
            }
            if (_gold2 < gold2)
            {
                reward += 50;
                _gold2 = gold2;
            }
            if(_crystal2 < crystal2)
            {
                reward += 50;
                _crystal2 = crystal2;
            }
        }

        _statue1 = statue1.currentHP;
        _statue2 = statue2.currentHP;
        _team1 = team1units.Count;
        _team2 = team2units.Count;
        return reward;
    }

    public string whichTeamDidIt(int team)
    {
        if(team % 2 != 0)
        {
            return "TEAM1";
        }
        else
        {
            return "TEAM2";
        }
    }

    public void summonTroop1(string team1)
    {
        cdteam1 = 0;
        population1 = team1units.Count;
        if (population1 > 30) return;
        GameObject inst = null;
        if(team1 == "MINER")
        {
            inst = Instantiate(miner, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "SWORD")
        {
            inst = Instantiate(swordswrath, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "ARCHIDON")
        {
            inst = Instantiate(archidon, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "SPEARTON")
        {
            inst = Instantiate(spearton, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "MAGIKILL")
        {
            inst = Instantiate(magikill, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "GIANT")
        {
            inst = Instantiate(enslaved_giant, garrison1.transform.position, Quaternion.identity);
            if(giantUpgrade1 == 1.3f)
            {
                inst.GetComponentInChildren<HPSystem>().currentHP = 1000;
                inst.GetComponentInChildren<HPSystem>().maxHP = 1000;
            }
            if (giantUpgrade1 == 1.7f)
            {
                inst.GetComponentInChildren<HPSystem>().currentHP = 1300;
                inst.GetComponentInChildren<HPSystem>().maxHP = 1300;
            }
            inst.transform.localScale = new Vector3(inst.transform.localScale.x * giantUpgrade1 / 1.2f, inst.transform.localScale.y * giantUpgrade1 / 1.2f);
        }
        if (team1 == "SHADOW")
        {
            inst = Instantiate(shadowrath, garrison1.transform.position, Quaternion.identity);
        }
        if (team1 == "ALBOWTROSS")
        {
            inst = Instantiate(albowtross, garrison1.transform.position, Quaternion.identity);
            inst.tag = "Team1Flying";
            return;
        }
        if (team1 == "MERIC")
        {
            inst = Instantiate(meric, garrison1.transform.position, Quaternion.identity);
        }


        inst.tag = "Team1";
        //team1units.Add(inst);
        
    }

    public void summonTroop2(string team1)
    {
        cdteam2 = 0;
        population2 = team2units.Count;
        if (population2 > 30) return;
        GameObject inst = null;
        if (team1 == "MINER")
        {
            inst = Instantiate(miner, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "CRAWLER")
        {
            inst = Instantiate(crawler, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "BOMBER")
        {
            inst = Instantiate(bomber, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "DEAD")
        {
            inst = Instantiate(dead, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "JUGGERNAUT")
        {
            inst = Instantiate(juggernaut, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "GIANT")
        {
            inst = Instantiate(giant, garrison2.transform.position, Quaternion.identity);
            if (giantUpgrade1 == 1.3f)
            {
                inst.GetComponentInChildren<HPSystem>().currentHP = 1300;
                inst.GetComponentInChildren<HPSystem>().maxHP = 1300;
            }
            if (giantUpgrade1 == 1.7f)
            {
                inst.GetComponentInChildren<HPSystem>().currentHP = 1800;
                inst.GetComponentInChildren<HPSystem>().maxHP = 1800;
            }
            inst.transform.localScale = new Vector3(inst.transform.localScale.x * giantUpgrade2 / 1.2f, inst.transform.localScale.y * giantUpgrade2 / 1.2f);
        }
        if (team1 == "MARROWKAI")
        {
            inst = Instantiate(marrowkai, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "MEDUSA")
        {
            inst = Instantiate(medusa, garrison2.transform.position, Quaternion.identity);
        }
        if (team1 == "ECLIPSOR")
        {
            inst = Instantiate(eclipsor, garrison2.transform.position, Quaternion.identity);
            inst.tag = "Team2Flying";
            return;
        }
        inst.tag = "Team2";
       // team2units.Add(inst);

    }

    public void playActionTEAM1(int action)
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
        TEAM1.text = "";
        TEAM2.text = "";
            if (action == 0)
            {
                TEAM1.text = "GARRISON UNITS";
                setTeam1(1);
            }
            else if (action == 1)
            {
                setTeam1(2);
                TEAM1.text = "DEFEND UNITS";
            }
            else if (action == 2)
            {
                setTeam1(3);
                TEAM1.text = "ATTACK UNITS";
                reward += 15;

            }
            else if (action == 3)
            {
                setTeam1Miners(1);
                TEAM1.text = "GARRISON MINERS";
            }
            else if (action == 4)
            {
                setTeam1Miners(2);
                TEAM1.text = "MINE";
            }
            else if (action >= 5 && action <= 13)
            {
                if (team1units.Count >= 30)
                {
                    TEAM1.text = "MAX UNITS";
                    reward -= 100;
                }
                else
                {

                    if (action == 5)
                    {
                        if (gold1 >= 150)
                        {
                            TEAM1.text = "MINER SUMMON";
                            gold1 -= 150;
                            summonTroop1("MINER");
                            reward += 20;
                        }
                        else
                        {
                            TEAM1.text = "CANNOT AFFORD MINER";
                            reward -= 100;
                        }

                        int numMiners = 0;
                        foreach (GameObject unit in team1units)
                        {
                            if (unit.GetComponent<Miner>() != null)
                            {
                                numMiners++;
                            }
                        }

                        if (numMiners > 12)
                        {
                            reward -= 300;
                        }
                    }
                    else if (action == 6)
                    {
                        if (gold1 >= 125)
                        {
                            gold1 -= 125;
                            summonTroop1("SWORD");
                            TEAM1.text = "SWORD SUMMON";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD SWORD";
                        }
                    }
                    else if (action == 7)
                    {
                        if (gold1 >= 300)
                        {
                            gold1 -= 300;
                            TEAM1.text = "ARCHIDON SUMMON";
                            summonTroop1("ARCHIDON");
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD ARCHIDON";
                        }
                    }
                    else if (action == 8)
                    {
                        if (gold1 >= 450 && crystal1 >= 100)
                        {
                            gold1 -= 450;
                            crystal1 -= 100;
                            summonTroop1("SPEARTON");
                            TEAM1.text = "SUMMON SPEARTON";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD SPEARTON";
                        }
                    }
                    else if (action == 9)
                    {
                        if (gold1 >= 400 && crystal1 >= 400)
                        {
                            gold1 -= 400;
                            crystal1 -= 400;
                            TEAM1.text = "MAGIKILL SUMMON";
                            summonTroop1("MAGIKILL");
                        }
                        else
                        {
                            reward -= 10;
                            TEAM1.text = "CANNOT AFFORD MAGIKILL";
                        }
                    }
                    else if (action == 10)
                    {
                        if (gold1 >= 1500)
                        {
                            gold1 -= 1500;
                            summonTroop1("GIANT");
                            TEAM1.text = "GIANT SPAWN";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD GIANT";
                        }
                    }
                    else if (action == 11)
                    {
                        if (gold1 >= 450 && crystal1 >= 150)
                        {
                            gold1 -= 450;
                            crystal1 -= 150;
                            TEAM1.text = "SHADOWRATH SPAWN";
                            summonTroop1("SHADOW");
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD SHADOW";
                        }
                    }
                    else if (action == 12)
                    {
                        if (gold1 >= 450 && crystal1 >= 200)
                        {
                            gold1 -= 450;
                            crystal1 -= 200;
                            summonTroop1("ALBOWTROSS");
                            TEAM1.text = "ALBOWTROSS SPAWN";
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD ALBOWTROSS";
                        }
                    }
                    else if (action == 13)
                    {
                        if (gold1 >= 300 && crystal1 >= 200)
                        {
                            gold1 -= 300;
                            crystal1 -= 200;
                            TEAM1.text = "MERIC SUMMON";
                            summonTroop1("MERIC");
                        }
                        else
                        {
                            reward -= 100;
                            TEAM1.text = "CANNOT AFFORD MERIC";
                        }
                    }
                }
            }
            else if (action == 14)
            {
                reward += buyRage();
            }
            else if (action == 15)
            {
                StartCoroutine(RAGE());
            }
            else if (action == 16)
            {
                reward += buyFire_Arrows();
            }
            else if (action == 17)
            {
                reward += (int)FIRE_ARROWS();
            }
            else if (action == 18)
            {
                reward += (int)SHIELD_WALL();
            }
            else if (action == 19)
            {
                reward += buyBlazing_Bolts();
            }
            else if (action == 20)
            {
                reward += buyGiant_Upgrade1();
            }
            else if (action == 21)
            {
                reward += buyPassive_Income1();
            }
            else if (action == 22)
            {
                reward += buyCastle_Archer1();
            }
            else if (action == 23)
            {
                setTeam1(4);
                TEAM1.text = "UNITS TO TOWER";
            }
            else if (action == 24)
            {
                reward += buyTowerSpawn1();
            }
            else
            {
                reward -= 10;
            }
        }
    }