using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int health;
	public static Player instance;
	[HideInInspector] public bool respawning = false;
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
	public void TakeDamage(int amount)
	{
		if (!respawning)
		{
			health -= amount;
			CheckDeath();
		}
	}

    void CheckDeath()
	{
        if(health < 1)
		{
			UIHandler.instance.GameOverScreen(true);			
		}
	}
}
