using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public void Attack(Enemy enemy)
    {
        enemy.health--;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Attack(collision.gameObject.GetComponent<Enemy>());
        }
    }
}
