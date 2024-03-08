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
            if (collider.gameObject.GetComponentInChildren<HPSystem>().gameObject == enemy)
                return true;
        }

        return false;

    }

    public GameObject IsTagWithinRange(String tag)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionWidth, detectionHeight), transform.rotation.eulerAngles.z);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.tag == tag)
                return collider.gameObject.GetComponentInChildren<HPSystem>().gameObject;
        }

        return null;

    }


}
