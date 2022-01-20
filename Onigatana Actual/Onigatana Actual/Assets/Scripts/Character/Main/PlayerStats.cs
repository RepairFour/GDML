using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BloodFuryState))]
public class PlayerStats : MonoBehaviour
{
	[SerializeField] int maxHealth;
	[SerializeField] Animator playerHurtAni;
	public int health { get; private set; }
	public int armour { get; private set; }

	BloodFuryState bloodFuryState;
	CameraShake cameraShake;
	public ImageFader bloodFader;

	
	private void Start()
	{
		health = maxHealth;
		bloodFuryState = GetComponent<BloodFuryState>();
		cameraShake = GetComponent<CameraShake>();
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
		HUDCon.instance.UpdateHpBar();
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
			IsDeadCheck();
		}
		HUDCon.instance.UpdateHpBar();
		
		//playerHurtAni.SetTrigger("Hurt");
		
		AudioHandler.instance.PlaySound("PlayerHurt");
		StartCoroutine(cameraShake.ShakeCamera());
		StartCoroutine(bloodFader.fadeImage());
	}
	public void IsDeadCheck()
	{
		if (health <= 0)
		{
			if (!bloodFuryState.EnterStateCheck()) //if it cannot enter blood fury
			{
				Death();
				AudioHandler.instance.PlaySound("PlayerDeath");
			}
		}
	}
	private void Death()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
		//GameManager.instance.gameOverCanvas.SetActive(true);
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
