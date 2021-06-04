using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] int damageTicks;
    private bool on = false;
    float timer = 0;
    float tickTimer = 0;
    [SerializeField] LaserHit hitbox;
    [SerializeField] GameObject laser;
    [SerializeField] float energyRequired;
    [SerializeField] int dmgPerTick;
    Player player;
    
	private void Start()
	{
        player = FindObjectOfType<Player>();
    }

	private void FixedUpdate()
	{
        //if weapon energy is at requirment 
        //and button attack pressed
        if (player.WeaponEnergy() >= energyRequired && Input.GetMouseButtonDown(1) && on == false)
		{
            player.UseWeaponEnergy(energyRequired);
            on = true;
		}
        Fire();
	}


	public void Fire()
    {
        if (on)
		{
            float timeBetweenDmg = duration / damageTicks;
            laser.SetActive(true);

            timer += Time.deltaTime;
            if (timer > duration)
			{
                timer = 0;
                on = false;
                return;
			}
            tickTimer += Time.deltaTime;
            if (tickTimer > timeBetweenDmg)
            {
                tickTimer = 0;
                //deal dmg
                if(hitbox.hitInfo.hitting)
				{
                    hitbox.hitInfo.enemyHit.TakeDamage(dmgPerTick);
                }
               
            }
		}
		else
		{
            laser.SetActive(false);
        }
	}


}
