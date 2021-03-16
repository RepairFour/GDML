using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
	[SerializeField] int damage;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.GetComponent<EnemyStats>() != null)
		{
			collision.gameObject.GetComponent<EnemyStats>().TakeDamage(damage);
		}
		if(collision.gameObject.GetComponent<PlayerMovement>())
		{
			return;
		}
		Destroy(gameObject);
	}
}
