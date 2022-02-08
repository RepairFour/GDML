using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	[HideInInspector]
	public int dmg;
	[SerializeField] float lifeSpan;
	float timeAlive = 0;
	bool hitSomething = false;
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<EnemyStats>() == null && !hitSomething)
		{
			if (other.GetComponent<PlayerStats>() != null)
			{
				other.GetComponent<PlayerStats>().Hurt(dmg, this);
				//Destroy(gameObject);
				hitSomething = true;
			}
			
		}
	}
	private void Start()
	{
		AudioHandler.instance.PlaySound("EnemyProjectile", 0.5f, true, 3);
	}
	private void Update()
	{
		timeAlive += Time.deltaTime;
		if(timeAlive > lifeSpan)
		{
			Destroy(gameObject);
		}
	}
}
