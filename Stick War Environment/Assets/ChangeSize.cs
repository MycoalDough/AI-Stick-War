using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSize : MonoBehaviour
{
    public float middle = -8.37f;
    public float change = 50;
    float max;
    private void Awake()
    {
        max = transform.localScale.x;
    }
    void Update()
    {
        float distance = (transform.position.y - middle);

        transform.localScale = new Vector3(max - distance / change, max - distance / change, transform.localScale.z);
    }
}
