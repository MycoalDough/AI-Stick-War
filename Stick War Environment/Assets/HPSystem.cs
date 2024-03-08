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

    public bool dazed;

    public List<Sprite> dazeEffects = new List<Sprite>();

    public void Awake()
    {
        gv = GameObject.FindObjectOfType<GlobalVariables>().GetComponent<GlobalVariables>();
    }

    public void Damage(float dmg)
    {
        currentHP -= dmg;

        HPBar.localScale = new Vector3 (currentHP/maxHP, HPBar.localScale.y, HPBar.localScale.z);

        if(currentHP <= 0)
        {
            spriteRenderer.sprite = deathSprite;

            gv.team1units.Remove(spriteRenderer.gameObject);
            spriteRenderer.gameObject.tag = "Untagged";

            foreach (Component sp in spriteRenderer.gameObject.GetComponents(typeof(Component)))
            {
                if(sp.GetType() != typeof(SpriteRenderer) && sp.GetType() != typeof(Transform))
                {
                    Destroy(sp);
                }
            }
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
