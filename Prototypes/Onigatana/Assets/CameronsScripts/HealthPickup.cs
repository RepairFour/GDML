using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
		if (playerStats != null)
		{
			playerStats.ModHealth(+20);
			Destroy(gameObject);
		}
	}

}
