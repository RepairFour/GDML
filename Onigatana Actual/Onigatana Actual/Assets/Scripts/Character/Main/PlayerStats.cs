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
	SwordAttack swordAttack;

	[SerializeField] float normalParryInvincibilityTime;
	[SerializeField] float perfectParryInvincibilityTime;
	float invincibilityTimer;
	float invincivilityTime;
	bool invincibilityFramesActivated = false;
	




	private void Start()
	{
		health = maxHealth;
		bloodFuryState = GetComponent<BloodFuryState>();
		swordAttack = GetComponentInChildren<SwordAttack>();
	}

    private void Update()
    {
        if (invincibilityFramesActivated)
        {
			invincibilityTimer += Time.deltaTime;
			if(invincibilityTimer >= invincivilityTime)
            {
				invincibilityTimer = 0;
				invincibilityFramesActivated = false;
			}
        }
		
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
		if (!invincibilityFramesActivated)
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

			playerHurtAni.SetTrigger("Hurt");
			AudioHandler.instance.PlaySound("PlayerHurt");
		}
        else
        {
			Debug.Log("HAHAHAHHA DIDNT GET HURT");
			//Play a sound or somthing
        }
	}
	public void Hurt(int dmg, ref EnemyStats es)
    {
		if(swordAttack.getParrying == true)
        {
			if(swordAttack.getParryState == SwordAttack.parryState.NORMAL ||
			   swordAttack.getParryState == SwordAttack.parryState.PERFECT)
            {
				float dot = Vector3.Dot(es.transform.forward, transform.position - es.transform.position);
				//If the enemy is in front of player we parry
				if(dot > 0)
                {
					if(swordAttack.getParryState == SwordAttack.parryState.PERFECT)
                    {
						//Player takes not damage
						//Does something with attack
						//Recovery state is ignored

						//Invisibility time - Longer then normal
						Debug.Log("Perfect Parry");
						invincibilityFramesActivated = true;
						invincivilityTime = perfectParryInvincibilityTime;
						swordAttack.ResetParry();
                    }
					else if (swordAttack.getParryState == SwordAttack.parryState.NORMAL)
                    {
						//Player takes no damage

						//Recovery state is ignored
						//Invisibitility Time - Shorter then Perfect
						Debug.Log("Normal Parry");
						invincibilityFramesActivated = true;
						invincivilityTime = perfectParryInvincibilityTime;
						swordAttack.ResetParry();
					}

				}
                else
                {
					Hurt(dmg);
                }
            }
            else
            {
				Hurt(dmg);
            }
        }
        else
        {
			Hurt(dmg);
        }
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
