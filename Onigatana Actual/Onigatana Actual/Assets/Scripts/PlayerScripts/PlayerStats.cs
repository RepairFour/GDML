using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BloodFuryState))]
public class PlayerStats : MonoBehaviour
{
	[SerializeField] int maxHealth;
	[SerializeField] int bloodCap;
	public int health { get; private set; }
	public int bloodMeter { get; private set; }
	public int armour { get; private set; }

	//[SerializeField] int bloodHardCap;
	BloodFuryState bloodFuryState;

	float attackTimer = 0;
	float attackTimerMax = 10;

	float drainTimer = 0;
	float drainTimerMax = 0.25f;
	private void Start()
	{
		health = maxHealth;
		bloodFuryState = GetComponent<BloodFuryState>();
		//HUDCon.instance.Initialise(bloodCap);
	}
	///////health stuff/////////
	public bool IsDead()
	{
        if(health <= 0)
		{
			return true;
		}
		return false;
	}
	public void Heal(int amount)
	{
		health += amount;		
	}
	public void Hurt(int dmg)
	{
		if (armour >= dmg / 2)
		{
			ModArmour(dmg / 2 * -1);
			health -= dmg - (dmg / 2);
		}
		else
		{
			int dmgRemaining = dmg - armour;
			armour = 0;
			health -= dmgRemaining;
		}
		//HUDCon.instance.UpdateHpBar();
		if(health <= 0)
		{
			Death();
		}
	}
	private void Death()
	{

	}
	public void ModArmour(int amount)
	{
		armour += amount;
		if(armour < 0)
		{
			armour = 0;
		}
	}

	//////blood stuff//////////
	
	public void FillBloodMeter(int amount)
	{
		bloodMeter += amount;
		if(bloodMeter > bloodCap)
		{
			bloodMeter = bloodCap;
		}	
		HUDCon.instance.UpdateBloodBar();
		////if > than 100 enter blood fury state
		//if(amount >= bloodSoftCap && !bloodFuryState.active)
		//{
		//	bloodFuryState.EnterState();
		//}
	}
	public void DrainBloodMeter(int amount)
	{
		bloodMeter -= amount;
		if (bloodMeter < 0)
		{
			bloodMeter = 0;
		}
		HUDCon.instance.UpdateBloodBar();
		//if(bloodMeter < 0)
		//{
		//	bloodMeter = 0;
		//}
		//if(bloodMeter < bloodSoftCap && bloodFuryState.active)
		//{
		//	bloodFuryState.ExitState();
		//}
	}
	public void Update()
	{
		attackTimer += Time.deltaTime;
		//if (attackTimer > attackTimerMax)
		//{
		//	//TODO: reset timer upon attacking an enemy
		//	drainTimer += Time.deltaTime;
		//	if (drainTimer > drainTimerMax)
		//	{
		//		drainTimer = 0;
		//		DrainBloodMeter(5);
		//	}
		//}
	}
}
