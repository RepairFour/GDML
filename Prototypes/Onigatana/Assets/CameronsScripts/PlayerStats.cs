using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	[SerializeField] int maxHealth;
	public int health { get; private set; }
	public int armour { get; private set; }

	private void Start()
	{
		health = maxHealth;
	}
	public bool isDead()
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
	}
	public void ModArmour(int amount)
	{
		armour += amount;
		if(armour < 0)
		{
			armour = 0;
		}
	}
}
