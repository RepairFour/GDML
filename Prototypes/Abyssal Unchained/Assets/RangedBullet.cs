using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBullet : MonoBehaviour
{
    public int damage;

    public float lifeTimer;
    public float internalLifeTimer = 0f;

    void Attack(EnemyStats enemy)
    {
        enemy.TakeDamage(damage);
    }
    private void Start()
    {
        lifeTimer = 10;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Attack(collision.gameObject.GetComponent<EnemyStats>());
        }
        if (collision.tag != "Checkpoint")
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        internalLifeTimer += Time.deltaTime;
        if(internalLifeTimer >= lifeTimer)
        {
            Destroy(gameObject);
        }
    }

}
