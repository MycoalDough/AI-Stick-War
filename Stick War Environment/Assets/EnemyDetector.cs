using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyDetector : MonoBehaviour
{
    public float detectionRange;

    private void OnDrawGizmos()
    {
        // Draw a visual representation of the detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public bool IsTargetWithinRange(GameObject enemy)
    {
        if (enemy == null)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, detectionRange);
        if (hit.collider != null && hit.collider.gameObject.GetComponentInChildren<HPSystem>().gameObject == enemy)
        {
            return true;

        }
        return false;
    }


}
