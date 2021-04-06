using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] int health;
	[SerializeField] AudioSource hurt;
	[SerializeField] Slider slider;
	bool isBoss = false;
	Animator aniCon;

	float cooldown = 1;
	float timer = 0;

	private void Start()
	{
		if (hurt == null)
		{
			hurt = GameObject.FindGameObjectWithTag("HurtAudio").GetComponent<AudioSource>();
		}
		if(GetComponent<TestBoss>() != null)
		{
			isBoss = true;
			aniCon = GetComponent<Animator>();
		}
	}
	public void TakeDamage(int amount)
	{
		health -= amount;
		if (slider != null)
		{
			slider.value = health;
		}
		if(isBoss)
		{
			aniCon.SetBool("Hit", true);
		}
		hurt.Play();
		
		CheckDeath();
	}
	private void CheckDeath()
	{
		if(health < 1)
		{
			if (!isBoss)
			{
				ParticleSystem temp = GameObject.FindGameObjectWithTag("EnemyDeathParticles").GetComponent<ParticleSystem>();
				temp.transform.position = transform.position;
				temp.Play();
			}
			else
			{
				ParticleSystem temp = GameObject.FindGameObjectWithTag("EnemyDeathParticles").GetComponent<ParticleSystem>();
				temp.transform.position = transform.position;
				temp.Play();
				UIHandler.instance.ShowWinScreen(true);
			}
			Destroy(gameObject);
            DeathAnalytics();
		}
	}

	public int CurrentHealth()
	{
		return health;
	}

	private void Update()
	{
		if (aniCon)
		{
			if (aniCon.GetBool("Hit") == true)
			{
				timer += Time.deltaTime;
				if (timer > cooldown)
				{
					timer = 0;
					aniCon.SetBool("Hit", false);
				}
			}
		}
	}

    void DeathAnalytics()
    {
        LevelEventManager.instance.RaiseBossKillEvent();
        LevelEventManager.instance.RaiseRunFinishedEvent();
    }
}
