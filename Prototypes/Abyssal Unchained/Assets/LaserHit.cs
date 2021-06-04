using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHit : MonoBehaviour
{
    public struct HitInfo
    {
        public bool hitting;
        public EnemyStats enemyHit;
    }
    public HitInfo hitInfo;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyStats>() != null)
        {
            hitInfo.hitting = true;
            hitInfo.enemyHit = collision.GetComponent<EnemyStats>();
        }
        else
        {
            hitInfo.hitting = false;
            hitInfo.enemyHit = null;
        }
    }
}
