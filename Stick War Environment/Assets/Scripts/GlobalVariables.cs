using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Mathematics;
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

    public int _team1;
    public int _team2;

    public float _statue1;
    public float _statue2;

    public float _gold1;
    public float _gold2;
    public float _crystal1;
    public float _crystal2;


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

    //TOWER SPAWN
    public int buyTowerSpawn1()
    {
        if (gold1 >= 600 && crystal1 >= 200 && towerSpawn1 == 0)
        {
            towerSpawn1 = 1;
            gold1 -= 600;
            crystal1 -= 200;
            return 0;
        }
        else if (gold1 >= 2000 && crystal1 >= 1000 && towerSpawn1 == 1)
        {
            towerSpawn1 = 2;
            gold1 -= 2000;
            crystal1 -= 1000;
            return 0;
        }
        return -10;

    }

    public int buyTowerSpawn2()
    {
        if (gold2 >= 600 && crystal2 >= 200 && towerSpawn2 == 0)
        {
            towerSpawn2 = 1;
            gold2 -= 600;
            crystal2 -= 200;
            return 0;
        }
        else if (gold2 >= 2000 && crystal2 >= 1000 && towerSpawn2 == 1)
        {
            towerSpawn2 = 2;
            gold2 -= 2000;
            crystal2 -= 1000;
            return 0;
        }
        return -10;

    }


    // RAGE

    public RLConnection RLConnection;
    public IEnumerator RAGE()
    {
        if (rageBUY && canRage)
        {
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
        }

        yield return null;
    }
    public int buyRage()
    {
        if(gold1 >= 50 && crystal1 >= 50 && !rageBUY) {
            rageBUY = true;
            canRage = true;
            gold1 -= 50;
            crystal1 -= 50;
            return 0;
        }
        return -10;

    }


    // FIRE ARROWS
    public IEnumerator FIRE_ARROWS()
    {
        if (canFireArrows) {
            fireArrows = !fireArrows;
        }
        yield return null;
    }
    public int buyFire_Arrows()
    {
        if (gold1 >= 50 && crystal1 >= 100 && !canFireArrows)
        {
            canFireArrows = true;
            gold1 -= 50;
            crystal1 -= 100;
            return 0;
        }
        return -10;
    }

    //SHIELD WALL
    public IEnumerator SHIELD_WALL()
    {
        shieldWall = !shieldWall;
        yield return null;
    }

    //BLAZING BOLTS
    public int buyBlazing_Bolts()
    {
        if (gold1 >= 400 && !blazingBolts)
        {
            blazingBolts = true;
            gold1 -= 400;
            return 0;
        }
        return -10;
    }

    //GIANT UPGRADE
    public int buyGiant_Upgrade1()
    {
        if (gold1 >= 400 && crystal1 >= 400 && giantUpgrade1 == 1)
        {
            giantUpgrade1 = 1.3f;
            gold1 -= 400;
            crystal1 -= 400;
            return 0;
        }else if(gold1 >= 600 && crystal1 >= 600 && giantUpgrade1 == 1.3f)
        {
            giantUpgrade1 = 1.7f;
            gold1 -= 600;
            crystal1 -= 600;return 0;
        }
        return -10;
    }

    public int buyGiant_Upgrade2()
    {
        if (gold2 >= 400 && crystal2 >= 400 && giantUpgrade2 == 1)
        {
            giantUpgrade2 = 1.3f;
            gold2 -= 400;
            crystal2 -= 400;
            return 0;
        }
        else if (gold2 >= 600 && crystal2 >= 600 && giantUpgrade2 == 1.3f)
        {
            giantUpgrade2 = 1.7f;
            gold2 -= 600;
            crystal2 -= 600;
            return 0;   
        }
        return -10;
    }


    //PASSIVE INCOME
    public int buyPassive_Income1()
    {
        if(gold1 >= 150 && crystal1 >= 100 && passive1 != 15)
        {
            if(passive1 == 5)
            {
                passive1 = 10;
                gold1 -= 150;
                crystal1 -= 100;
                return 0;
            }
            else
            {
                passive1 = 15;
                gold1 -= 150;
                crystal1 -= 100;
                return 0;
            }
        }

        return -10;


    }
    public int buyPassive_Income2()
    {
        if (gold2 >= 150 && crystal2 >= 100 && passive2 != 15)
        {
            if (passive2 == 5)
            {
                passive2 = 10;
                gold2 -= 150;
                crystal2 -= 100;
                return 0;
            }
            else
            {
                passive2 = 15;
                gold2 -= 150;
                crystal2 -= 100;
                return 0;
            }
        }

        return -10;


    }

    //CASTLE ARCHERS
    public int buyCastle_Archer1()
    {
        if (castle1ability == 3) { return -10; }
        if (gold1 >= 300 && castle1ability == 0)
        {
            gold1 -= 300;
            castle1ability++;
            return 0;
        }
        else if (gold1 >= 600 && castle1ability == 1)
        {
            gold1 -= 600;
            castle1ability++;
            return 0;
        }
        else if (gold1 >= 1000 && castle1ability == 2)
        {
            gold1 -= 1000;
            castle1ability++;
            return 0;
        }

        return -10;
    }
    public int buyCastle_Archer2()
    {
        if (castle2ability == 3) { return -10; }
        if (gold2 >= 300 && castle2ability == 0)
        {
            gold2 -= 300;
            castle2ability++;
            return 0;
        }
        else if (gold2 >= 600 && castle2ability == 1)
        {
            gold2 -= 600;
            castle2ability++;
            return 0;
        }
        else if (gold2 >= 2000 && castle2ability == 2)
        {
            gold2 -= 2000;
            castle2ability++;
            return 0;
        }
        return -10;
    }





    private void Update()
    {
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

        int numberOfCastles = castle1ability + (int)Mathf.Floor(s / 10);
        if (s > 1) { numberOfCastles++; }

        if(RLConnection.time > 1800)
        {
            numberOfCastles = 0;
        }

        while (numberOfCastles != castle1.Count) {
            if(numberOfCastles > castle1.Count)
            {
                CastleArchidon ca = Instantiate(castleArchidon.gameObject, new Vector2(castleSpawn1.position.x, castleSpawn1.position.y + UnityEngine.Random.Range(-1f, 1f)), quaternion.identity).GetComponent<CastleArchidon>();
                ca.tag = "Team1";
                castle1.Add(ca);
            }

            if(numberOfCastles < castle1.Count)
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

        int numberOfCastles = castle2ability + (int)Mathf.Floor(s / 10);

        if(s > 1) { numberOfCastles++; }

        if (RLConnection.time > 1800)
        {
            numberOfCastles = 0;
        }

        while (numberOfCastles != castle2.Count)
        {
            if (numberOfCastles > castle2.Count)
            {
                CastleArchidon ca = Instantiate(castleArchidon.gameObject, new Vector2(castleSpawn2.position.x, castleSpawn2.position.y + UnityEngine.Random.Range(-1f, 1f)), quaternion.identity).GetComponent<CastleArchidon>();
                ca.tag = "Team2";
                castle2.Add(ca);
            }

            if (numberOfCastles < castle2.Count)
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

            foreach (Resource resouce in mines)
            {
                resouce.durability = 200;
                resouce.queue.Clear();
            }

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
        team1miners = to;
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

    public void testAction(int action)
    {
        int reward = 0;
        if (action == 0)
        {
            setTeam2(1);
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
        if (team2units.Count < 50)
        {
            if (action == 5)
            {
                if (gold2 >= 150)
                {
                    gold2 -= 150;
                    summonTroop2("MINER");
                }
                else
                {
                    reward -= 10;
                }

                int numMiners = 0;
                foreach (GameObject unit in team2units)
                {
                    if (unit.GetComponent<Miner>() != null)
                    {
                        numMiners++;
                    }
                }

                if (numMiners > 8)
                {
                    reward -= 50;
                }
            }
            if (action == 6)
            {
                if (gold2 >= 100)
                {
                    gold2 -= 100;
                    summonTroop2("CRAWLER");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 7)
            {
                if (gold2 >= 200)
                {
                    gold2 -= 200;
                    summonTroop2("BOMBER");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 8)
            {
                if (gold2 >= 450 && crystal2 >= 50)
                {
                    gold2 -= 450;
                    crystal2 -= 50;
                    summonTroop2("JUGGERNAUT");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 9)
            {
                if (gold2 >= 300 && crystal2 >= 100)
                {
                    gold2 -= 300;
                    crystal2 -= 100;
                    summonTroop2("DEAD");
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
                    summonTroop2("GIANT");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 11)
            {
                if (gold2 >= 400 && crystal2 >= 100)
                {
                    gold2 -= 400;
                    crystal2 -= 100;
                    summonTroop2("ECLIPSOR");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 12)
            {
                if (gold2 >= 400 && crystal2 >= 400)
                {
                    gold2 -= 400;
                    crystal2 -= 400;
                    summonTroop2("MARROWKAI");
                }
                else
                {
                    reward -= 10;
                }
            }
            if (action == 13)
            {
                if (gold2 >= 500 && crystal2 >= 400)
                {
                    gold2 -= 500;
                    crystal2 -= 400;
                    summonTroop2("MEDUSA");
                }
                else
                {
                    reward -= 10;
                }
            }
        }
        if (action == 13)
        {
            reward += buyCastle_Archer2();
        }
        else if (action == 14)
        {
            reward += buyPassive_Income2();
        }
        else if (action == 15)
        {
            reward += buyGiant_Upgrade2();
        }
        else if(action == 16)
        {
            setTeam2(4);
        }
        else if (action == 17)
        {
            reward += buyTowerSpawn2();
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
            if(team1units.Count < 50)
            {
                if (action == 5)
                {
                    if (gold1 >= 150)
                    {
                        gold1 -= 150;
                        summonTroop1("MINER");
                        reward += 20;
                    }
                    else
                    {
                        reward -= 10;
                    }

                    int numMiners = 0;
                    foreach(GameObject unit in team1units)
                    {
                        if(unit.GetComponent<Miner>() != null)
                        {
                            numMiners++;
                        }
                    }

                    if(numMiners > 12)
                    {
                        reward -= 80;
                    }
                }
                else if (action == 6)
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
                else if (action == 7)
                {
                    if (gold1 >= 300)
                    {
                        gold1 -= 300;
                        summonTroop1("ARCHIDON");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                else if(action == 8)
                {
                    if (gold1 >= 450 && crystal1 >= 100)
                    {
                        gold1 -= 450;
                        crystal1 -= 100;
                        summonTroop1("SPEARTON");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                else if (action == 9)
                {
                    if (gold1 >= 400 && crystal1 >= 400)
                    {
                        gold1 -= 400;
                        crystal1 -= 400;
                        summonTroop1("MAGIKILL");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                else if (action == 10)
                {
                    if (gold1 >= 1500)
                    {
                        gold1 -= 1500;
                        summonTroop1("GIANT");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                else if (action == 11)
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
                else if (action == 12)
                {
                    if (gold1 >= 450 && crystal1 >= 200)
                    {
                        gold1 -= 450;
                        crystal1 -= 200;
                        summonTroop1("ALBOWTROSS");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                else if (action == 13)
                {
                    if (gold1 >= 300 && crystal1 >= 200)
                    {
                        gold1 -= 300;
                        crystal1 -= 200;
                        summonTroop1("MERIC");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
            }
            if (action == 14)
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
                StartCoroutine(FIRE_ARROWS());
            }
            else if (action == 18)
            {
                StartCoroutine(SHIELD_WALL());
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
                setTeam2(1);
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
            if(team2units.Count < 50)
            {
                if (action == 5)
                {
                    if (gold2 >= 150)
                    {
                        gold2 -= 150;
                        summonTroop2("MINER");
                        reward += 10;
                    }
                    else
                    {
                        reward -= 10;
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
                        reward -= 80;
                    }
                }
                if (action == 6)
                {
                    if (gold2 >= 100)
                    {
                        gold2 -= 100;
                        summonTroop2("CRAWLER");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                if (action == 7)
                {
                    if (gold2 >= 200)
                    {
                        gold2 -= 200;
                        summonTroop2("BOMBER");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                if (action == 8)
                {
                    if (gold2 >= 450 && crystal2 >= 50)
                    {
                        gold2 -= 450;
                        crystal2 -= 50;
                        summonTroop2("JUGGERNAUT");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                if (action == 9)
                {
                    if (gold2 >= 300 && crystal2 >= 100)
                    {
                        gold2 -= 300;
                        crystal2 -= 100;
                        summonTroop2("DEAD");
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
                        summonTroop2("GIANT");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                if (action == 11)
                {
                    if (gold2 >= 400 && crystal2 >= 100)
                    {
                        gold2 -= 400;
                        crystal2 -= 100;
                        summonTroop2("ECLIPSOR");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                if (action == 12)
                {
                    if (gold2 >= 400 && crystal2 >= 400)
                    {
                        gold2 -= 400;
                        crystal2 -= 400;
                        summonTroop2("MARROWKAI");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
                if (action == 12)
                {
                    if (gold2 >= 500 && crystal2 >= 400)
                    {
                        gold2 -= 500;
                        crystal2 -= 400;
                        summonTroop2("MEDUSA");
                    }
                    else
                    {
                        reward -= 10;
                    }
                }
            }
            if(action == 13)
            {
                reward += buyCastle_Archer2();
            }else if(action == 14)
            {
                reward += buyPassive_Income2();
            }else if(action == 15)
            {
                reward += buyGiant_Upgrade2();
            }
            else if (action == 16)
            {
                setTeam2(4);
            }
            else if (action == 17)
            {
                reward += buyTowerSpawn2();
            }
            else
            {
                reward -= 10;
            }
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
                reward -= 15;
            }
            if(team1units.Count < _team1)
            {
                reward -= 10;
            }
            if(team2units.Count < _team2)
            {
                reward += 20;
            }
            if (statue2.currentHP < _statue2)
            {
                reward += 30;
            }
            if(gold1 > 3000)
            {
                reward -= 10;
            }
            if(tower.control == 100)
            {
                reward += 10;
            }
            if((team1 == 1 && team2 != 3) || (team1miners == 1 && team2 != 3))
            {
                reward -= 30;
            }

            if(_gold1 < gold1)
            {
                reward += 10;
                _gold1 = gold1;
            }
            if(_crystal1 < crystal1)
            {
                reward += 10;
                _crystal1 = crystal1;
            }

            if(gold1 < 150) { reward -= 10; }


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

            if (gold2 < 150) { reward -= 10; }
            if (statue1.currentHP < _statue1)
            {
                reward += 30;
            }
            if (team1units.Count < _team1)
            {
                reward += 20;
            }
            if (team2units.Count < _team2)
            {
                reward -= 10;
            }
            if (statue2.currentHP < _statue2)
            {
                reward -= 15;
            }
            if (gold2 > 3000)
            {
                reward -= 10;
            }
            if (tower.control == -100)
            {
                reward += 10;
            }
            if ((team2 == 1 && team1 != 3) || (team2miners == 1 && team1 != 3))
            {
                reward -= 30;
            }
            if (_gold2 < gold2)
            {
                reward += 10;
                _gold2 = gold2;
            }
            if(_crystal2 < crystal2)
            {
                reward += 10;
                _crystal2 = crystal2;
            }
        }

        _statue1 = statue1.currentHP;
        _statue2 = statue2.currentHP;
        _team1 = team1units.Count;
        _team2 = team2units.Count;
        return reward;
    }

    public void summonTroop1(string team1)
    {
        population1 = team1units.Count;
        if (population1 > 50) return;
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
            inst.GetComponentInChildren<HPSystem>().currentHP = inst.GetComponentInChildren<HPSystem>().currentHP * giantUpgrade1;
            inst.GetComponentInChildren<HPSystem>().maxHP = inst.GetComponentInChildren<HPSystem>().maxHP * giantUpgrade1;
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
        population2 = team2units.Count;
        if (population2 > 50) return;
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
            inst.GetComponentInChildren<HPSystem>().currentHP = inst.GetComponentInChildren<HPSystem>().currentHP * giantUpgrade2;
            inst.GetComponentInChildren<HPSystem>().maxHP = inst.GetComponentInChildren<HPSystem>().maxHP * giantUpgrade2;
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

}
