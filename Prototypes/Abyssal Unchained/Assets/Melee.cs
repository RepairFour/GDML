using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int energyCharge;
    public void Attack(EnemyStats enemy)
    {
        int dmgEnhancement = 0;
        if(PlayerAbilities.instance.spellswordKeyStone.buffOn)
		{
            dmgEnhancement = PlayerAbilities.instance.spellswordKeyStone.dmgEnhancement;
            PlayerAbilities.instance.spellswordKeyStone.buffOn = false;
        }
        enemy.TakeDamage(damage + dmgEnhancement);
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
