using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Player : MonoBehaviour
{
    [SerializeField] int health;
	public static Player instance;
	[HideInInspector] public bool respawning = false;
	[SerializeField] float invincibilityTime;
	float invincibilityTimer = 0;
	bool takenDmg = false;
	[SerializeField] Animator screenFlash;
	private float weaponEnergy = 60; //half full

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else 
		{
			Destroy(this);
		}
	}

	private void Update()
	{
		if (takenDmg)
		{
			invincibilityTimer += Time.deltaTime;
			if (invincibilityTimer > invincibilityTime)
			{
				invincibilityTimer = 0;
				takenDmg = false;
			}
		}
	}
	public void TakeDamage(int amount)
	{
		
		if (!respawning && !takenDmg)
		{
			GetComponent<Animator>().SetTrigger("PlayerHit");
			AudioHandler.instance.PlaySound("PlayerHurt",1,true,1);
			takenDmg = true;
 			health -= amount;
			UIHandler.instance.ReducePlayerHealthText();
			screenFlash.SetTrigger("Trigger");
			CheckDeath();
		}
		
	}

    void CheckDeath()
	{
        if(health < 1)
		{
			UIHandler.instance.GameOverScreen(true);
            DeathAnalytics();
		}
	}

	public int CurrentHealth()
	{
		return health;
	}

    void DeathAnalytics()
    {
        LevelEventManager.instance.RaiseDeathEvent();
        LevelEventManager.instance.RaiseRunFinishedEvent();
    }

	public void ChargeWeaponEnergy(float amount)
	{
		weaponEnergy += amount;
		if(weaponEnergy > 120)
		{
			weaponEnergy = 120;
		}
		UIHandler.instance.UpdateWeaponEnergySlider((int)weaponEnergy);
	}
	public bool UseWeaponEnergy(float amount)
	{
		if (amount <= weaponEnergy)
		{
			weaponEnergy -= amount;
			UIHandler.instance.UpdateWeaponEnergySlider((int)weaponEnergy);
			return true;
		}
		else
		{
			return false;
		}
	}
	public float WeaponEnergy()
	{
		return weaponEnergy;
	}
}
