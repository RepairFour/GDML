using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompStats : MonoBehaviour
{
	[HideInInspector]
    public int dmg;
	[HideInInspector]
	public bool hitPlayer = false;
	private void OnTriggerEnter(Collider other)
	{
		PlayerStats player = other.GetComponent<PlayerStats>();
		if (player != null && !hitPlayer)
		{
			player.Hurt(dmg);
			hitPlayer = true;
		}
	}
}
