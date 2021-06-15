using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float lifetime; //how long this will exist

	private void Update()
	{
		lifetime -= Time.deltaTime;
		if(lifetime < 0)
		{
			Destroy(gameObject);
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.GetComponent<EnemyStats>() != null)
		{
			collision.gameObject.GetComponent<EnemyStats>().TakeDamage(damage);
			Destroy(gameObject);
		}
	}
}
