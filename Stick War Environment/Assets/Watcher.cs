using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class Watcher : MonoBehaviour
{

    public RLConnection rl;
    public Image image;
    public TextMeshProUGUI name_;
    public TextMeshProUGUI hp;
    public GlobalVariables gv;

    public float time;
    public float maxTime = 10f;

    public GameObject mainCam;
    public GameObject singleCam;
    public GameObject frame;

    public List<GameObject> assets;

    public GameObject target;
    void Start()
    {
        if(rl.team1AI || rl.team2AI)
        {
            mainCam.SetActive(false);
            gameObject.SetActive(false);
        }
        
    }

    public void changeCamera()
    {
        if(Random.Range(0,6)  == 0)
        {
            frame.SetActive(false);
            singleCam.SetActive(false);
            mainCam.SetActive(true);
        }
        else
        {
            frame.SetActive(true);
            singleCam.SetActive(true);
            mainCam.SetActive(false);

            if(Random.Range(0,2) == 0)
            {
                showPerson(1);
            }
            else
            {
                showPerson(2);
            }
        }
    }

    public void showPerson(int team)
    {
        if(team == 1)
        {
            if(gv.team1units.Count == 1)
            {
                time = maxTime;
                return;
            }

            List<GameObject> list = new List<GameObject>();

            for(int i = 0; i < gv.team1units.Count; i++)
            {
                if (!gv.team1units[i].GetComponent<Miner>())
                {
                    list.Add(gv.team1units[i]);
                }
            }
            int index = Random.Range(1, list.Count);
            target = list[index];
            int icon = int.Parse(rl.returnUnitType(target)) - 1;
            if (icon == -0.5)
            {
                icon = 17;
            }
            Texture2D texture = AssetPreview.GetAssetPreview(assets[icon]);
            Sprite sprite = Sprite.Create(texture,
                                          new Rect(0, 0, texture.width, texture.height),
                                          new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
            name_.text = assets[icon].name;

        }
        else
        {
            if (gv.team2units.Count == 1)
            {
                time = maxTime;
                return;
            }

            List<GameObject> list = new List<GameObject>();

            for (int i = 0; i < gv.team2units.Count; i++)
            {
                if (!gv.team2units[i].GetComponent<Miner>())
                {
                    list.Add(gv.team2units[i]);
                }
            }
            int index = Random.Range(1, list.Count); 
            target = list[index];
            int icon = int.Parse(rl.returnUnitType(target)) - 1;
            Debug.Log(assets[icon]);
            Texture2D texture = AssetPreview.GetAssetPreview(assets[icon]);
            Sprite sprite = Sprite.Create(texture,
                                          new Rect(0, 0, texture.width, texture.height),
                                          new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
            name_.text = assets[icon].name;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (singleCam.activeInHierarchy && (target == null || target.transform.Find("HPSystem") == null))
        {
            changeCamera();
            time = 0;
            return;
        }

        if (singleCam.activeInHierarchy)
        {
            if (name_.text != null && target != null)
            {
                HPSystem hpsystem = target.transform.Find("HPSystem").gameObject.GetComponent<HPSystem>();
                hp.text = hpsystem.currentHP + "/" + hpsystem.maxHP;
            }
        }

        if(target && singleCam.activeInHierarchy)
        {
            singleCam.transform.position = new Vector3(target.transform.position.x, singleCam.transform.position.y, singleCam.transform.position.z);
        }

        time += Time.deltaTime;

        if(time > maxTime)
        {
            changeCamera();
            time = 0;
        }


    }
}
