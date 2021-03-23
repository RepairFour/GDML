using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public void Attack(EnemyStats enemy)
    {
        enemy.TakeDamage(1);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Attack(collision.gameObject.GetComponent<EnemyStats>());
        }
    }
}
