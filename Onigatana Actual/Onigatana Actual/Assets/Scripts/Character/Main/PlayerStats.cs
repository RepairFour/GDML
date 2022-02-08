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
	

	[SerializeField] float normalParryInvincibilityTime;
	[SerializeField] float perfectParryInvincibilityTime;
	float invincibilityTimer;
	float invincibilityTime;
	bool invincibilityFramesActivated = false;
	
	private void Start()
	{
		health = maxHealth;
		bloodFuryState = GetComponent<BloodFuryState>();
		cameraShake = GetComponent<CameraShake>();

       
	}

    private void Update()
    {
        if (invincibilityFramesActivated)
        {
			invincibilityTimer += Time.deltaTime;
			if(invincibilityTimer >= invincibilityTime)
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

			//playerHurtAni.SetTrigger("Hurt");

			AudioHandler.instance.PlaySound("PlayerHurt");
			StartCoroutine(cameraShake.ShakeCamera());
			StartCoroutine(bloodFader.fadeImage());
		}
        else
        {
			//Something else will happen
        }
	}

	public void Hurt(int dmg, ref EnemyStats es)
	{
		Weapon currentWeapon = GameManager.instance.currentPlayerWeapon;
		if (currentWeapon.getParrying == true)
		{
			if (currentWeapon.getParryState == Weapon.ParryState.NORMAL ||
			   currentWeapon.getParryState == Weapon.ParryState.PERFECT)
			{
				float dot = Vector3.Dot(es.transform.forward, transform.forward);
				//If the enemy is in front of player we parry
				if (dot < 0)
				{
					if (currentWeapon.getParryState == Weapon.ParryState.PERFECT)
					{
						//Player takes not damage
						//Does something with attack
						//Recovery state is ignored

						//Invisibility time - Longer then normal
						Debug.Log("Perfect Parry");
						invincibilityFramesActivated = true;
						invincibilityTime = perfectParryInvincibilityTime;
						currentWeapon.ResetParry();
					}
					else if (currentWeapon.getParryState == Weapon.ParryState.NORMAL)
					{
						//Player takes no damage

						//Recovery state is ignored
						//Invisibitility Time - Shorter then Perfect
						Debug.Log("Normal Parry");
						invincibilityFramesActivated = true;
						invincibilityTime = perfectParryInvincibilityTime;
						currentWeapon.ResetParry();
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

	public void Hurt(int dmg, EnemyProjectile projectile)
	{
		Weapon currentWeapon = GameManager.instance.currentPlayerWeapon;
		if (currentWeapon.getParrying == true)
		{
			if (currentWeapon.getParryState == Weapon.ParryState.NORMAL ||
			   currentWeapon.getParryState == Weapon.ParryState.PERFECT)
			{
				var projVelocity = projectile.GetComponent<Rigidbody>().velocity;
				projVelocity.Normalize();
				float dot = Vector3.Dot(projVelocity, transform.forward);
				//If the enemy is in front of player we parry
				if (dot < 0)
				{
					if (currentWeapon.getParryState == Weapon.ParryState.PERFECT)
					{
						//Player takes not damage
						//Does something with attack
						//Recovery state is ignored

						//Invisibility time - Longer then normal
						Debug.Log("Perfect Parry");
						invincibilityFramesActivated = true;
						invincibilityTime = perfectParryInvincibilityTime;
						currentWeapon.ResetParry();
						projectile.GetComponent<Rigidbody>().velocity *= -1;
					}
					else if (currentWeapon.getParryState == Weapon.ParryState.NORMAL)
					{
						//Player takes no damage

						//Recovery state is ignored
						//Invisibitility Time - Shorter then Perfect
						Debug.Log("Normal Parry");
						invincibilityFramesActivated = true;
						invincibilityTime = normalParryInvincibilityTime;
						currentWeapon.ResetParry();
						projectile.GetComponent<Rigidbody>().velocity *= -1;
					}

				}
				else
				{
					Hurt(dmg);
					Destroy(projectile.gameObject);
				}
			}
			else
			{
				Hurt(dmg);
				Destroy(projectile.gameObject);
			}
		}
		else
		{
			Hurt(dmg);
			Destroy(projectile.gameObject);
		}
	}
	public void Hurt(int dmg, TurretEnemy enemy)
	{
		Weapon currentWeapon = GameManager.instance.currentPlayerWeapon;
		if (currentWeapon.getParrying == true)
		{
			if (currentWeapon.getParryState == Weapon.ParryState.NORMAL ||
			   currentWeapon.getParryState == Weapon.ParryState.PERFECT)
			{
				float dot = Vector3.Dot(enemy.transform.forward, transform.forward);
				//If the enemy is in front of player we parry
				if (dot < 0)
				{
					if (currentWeapon.getParryState == Weapon.ParryState.PERFECT)
					{
						//Player takes not damage
						//Does something with attack
						//Recovery state is ignored

						//Invisibility time - Longer then normal
						Debug.Log("Perfect Parry Activated");
						invincibilityFramesActivated = true;
						invincibilityTime = perfectParryInvincibilityTime;
						currentWeapon.ResetParry();
					}
					else if (currentWeapon.getParryState == Weapon.ParryState.NORMAL)
					{
						//Player takes no damage

						//Recovery state is ignored
						//Invisibitility Time - Shorter then Perfect
						Debug.Log("Normal Parry");
						invincibilityFramesActivated = true;
						invincibilityTime = perfectParryInvincibilityTime;
						currentWeapon.ResetParry();
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
