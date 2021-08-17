using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	[HideInInspector]
	public int dmg;
	[SerializeField] float lifeSpan;
	float timeAlive = 0;
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<EnemyStats>() == null)
		{
			if (other.GetComponent<PlayerStats>() != null)
			{
				other.GetComponent<PlayerStats>().Hurt(dmg);
			}
			Destroy(gameObject);
		}
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
