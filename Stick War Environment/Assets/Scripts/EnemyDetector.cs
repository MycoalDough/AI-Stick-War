using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyDetector : MonoBehaviour
{
    public float detectionWidth;
    public float detectionHeight;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 center = transform.position;
        Vector3 size = new Vector3(detectionWidth, detectionHeight, 0f);

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawWireCube(Vector3.zero, size);
    }
    public bool IsTargetWithinRange(GameObject enemy)
    {
        if (enemy == null)
            return false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionWidth, detectionHeight), transform.rotation.eulerAngles.z);
        foreach (var collider in colliders)
        {
            if(colliders.Length == 0) { return false; }
            if (collider && collider.gameObject && collider.gameObject.GetComponentInChildren<HPSystem>().gameObject == enemy)
                return true;
        }

        return false;

    }

    public int getNameNameInRange(string enemy)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionWidth, detectionHeight), transform.rotation.eulerAngles.z);
        int num = 0;
        foreach (var collider in colliders)
        {
            if (colliders.Length == 0) { return 0; }
            if (collider && collider.gameObject && collider.gameObject.name.Contains(enemy))
            {
                num++;
            }
        }

        return num;

    }

    public GameObject IsTagWithinRange(String tag)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionWidth, detectionHeight), transform.rotation.eulerAngles.z);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.tag == tag && collider.gameObject.GetComponentInChildren<HPSystem>())
                return collider.gameObject.GetComponentInChildren<HPSystem>().gameObject;
        }

        return null;

    }


    public List<GameObject> IsTagWithinRangeList(String tag, int count)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionWidth, detectionHeight), transform.rotation.eulerAngles.z);
        int currentCount = 0;
        List<GameObject> list = new List<GameObject>();
        HashSet<GameObject> uniqueSet = new HashSet<GameObject>();

        foreach (var collider in colliders)
        {
            GameObject hpSystemObject = collider.gameObject.GetComponentInChildren<HPSystem>()?.gameObject;

            // Check if the collider has the specified tag, has an HPSystem component, and is not already in the list
            if (collider.gameObject.tag == tag && hpSystemObject != null && !uniqueSet.Contains(hpSystemObject))
            {
                list.Add(hpSystemObject);
                uniqueSet.Add(hpSystemObject); // Add the GameObject to the HashSet
                currentCount++;
            }

            if (currentCount >= count)
            {
                break;
            }
        }

        return list;
    }
}