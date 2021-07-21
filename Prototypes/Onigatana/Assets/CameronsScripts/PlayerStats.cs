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

	public void ModHealth(int amount)
	{
		health += amount;
	}
	public void ModArmour(int amount)
	{
		armour += amount;
	}
}
