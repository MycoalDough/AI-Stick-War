using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : MonoBehaviour
{
    public int removeAfter = 10;

    private void Awake()
    {
        StartCoroutine(remove());
    }

    private void Update()
    {
        StartCoroutine (remove());
    }

    IEnumerator remove()
    {
        yield return new WaitForSeconds(removeAfter);
        Destroy(gameObject);
    }
}
