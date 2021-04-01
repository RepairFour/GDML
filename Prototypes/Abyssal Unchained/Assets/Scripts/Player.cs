using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int health;
	public static Player instance;
	[HideInInspector] public bool respawning = false;
	[SerializeField] float invincibilityTime;
	float invincibilityTimer = 0;
	bool takenDmg = false;
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

	public int CurrentHealth()
	{
		return health;
	}
}
