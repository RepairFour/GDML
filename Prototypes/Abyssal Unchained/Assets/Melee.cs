﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int energyCharge;
    public void Attack(EnemyStats enemy)
    {
        enemy.TakeDamage(damage);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Attack(collision.gameObject.GetComponent<EnemyStats>());
            FindObjectOfType<Player>().ChargeWeaponEnergy(energyCharge);
        }
    }
}
