using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchidonArrowFix : MonoBehaviour
{
    void Update()
    {
        if (!GetComponent<Transform>())
        {
            Destroy(gameObject);
        }
    }
}
