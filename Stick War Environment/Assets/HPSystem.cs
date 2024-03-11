using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSystem : MonoBehaviour
{
    public Transform HPBar;
    public float maxHP;
    public float currentHP;

    public GlobalVariables gv;

    public List<Component> toRemove = new List<Component>();

    public SpriteRenderer spriteRenderer;
    public Sprite deathSprite;

    public SpriteRenderer daze;

    public bool heal;
    public bool dazed;

    public bool isInList = false;

    public List<Sprite> dazeEffects = new List<Sprite>();

    public void Awake()
    {
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    private void Update()
    {
        HPBar.localScale = new Vector3(currentHP / maxHP, HPBar.localScale.y, HPBar.localScale.z);
        if(!isInList) { StartCoroutine(addToList()); }

        if(spriteRenderer.gameObject.tag == "Team1")
        {
            if (gv.garrisonDetector1.IsTargetWithinRange(gameObject))
            {
                if (!heal)
                {
                    StartCoroutine(healSelf());
                }
            }
        }

        if (spriteRenderer.gameObject.tag == "Team2")
        {
            if (gv.garrisonDetector2.IsTargetWithinRange(gameObject))
            {
                if (!heal)
                {
                    StartCoroutine(healSelf());
                }
            }
        }
    }

    public IEnumerator healSelf()
    {
        heal = true;
        yield return new WaitForSeconds(3);
        currentHP += maxHP / 15;

        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        heal = false;
    }

    public IEnumerator addToList()
    {
        yield return new WaitForSeconds(0.1f);
        if (!isInList)
        {
            if (spriteRenderer.tag == "Team1")
            {
                if (gv.team1units.Contains(spriteRenderer.gameObject))
                {
                    isInList = true;
                }
                else
                {
                    gv.team1units.Add(spriteRenderer.gameObject);
                    isInList = true;
                }
            }

            if (spriteRenderer.tag == "Team2")
            {
                if (gv.team2units.Contains(spriteRenderer.gameObject))
                {
                    isInList = true;
                }
                else
                {
                    gv.team2units.Add(spriteRenderer.gameObject);
                    isInList = true;
                }
            }
        }
    }

    public void Damage(float dmg)
    {
        currentHP -= dmg;

        HPBar.localScale = new Vector3 (currentHP/maxHP, HPBar.localScale.y, HPBar.localScale.z);

        if(currentHP <= 0)
        {
            spriteRenderer.sprite = deathSprite;
            spriteRenderer.sortingOrder = -19;

            foreach (Component sp in spriteRenderer.gameObject.GetComponents(typeof(Component)))
            {
                if (sp.GetType() != typeof(SpriteRenderer) && sp.GetType() != typeof(Transform))
                {
                    Destroy(sp);
                }
            }

            if (spriteRenderer.gameObject.tag == "Team1")
            {
                gv.team1units.Remove(spriteRenderer.gameObject);
            }
            else
            {
                gv.team2units.Remove(spriteRenderer.gameObject);
            }

            spriteRenderer.gameObject.tag = "Untagged";

            spriteRenderer.gameObject.AddComponent<Remove>();
            Destroy(gameObject);

        }
        
    }

    public void Daze()
    {
        StartCoroutine(DazeWait());
    }

    IEnumerator DazeWait()
    {
        daze.gameObject.SetActive(true);
        dazed = true;

        for(int i = 0; i < 8; i++)
        {
            if(i % 2 == 0)
            {
                daze.sprite = dazeEffects[0];
            }
            else
            {
                daze.sprite = dazeEffects[1];
            }
            yield return new WaitForSeconds(0.15f);
        }
        daze.gameObject.SetActive(false);
        dazed = false;
    }
}
