using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
	public enum PickupType
	{
		health,
		armour,
		bomb
	}
	[SerializeField] PickupType pickupType;
	[SerializeField] int value;
	

	private void OnTriggerEnter(Collider other)
	{
		PlayerStats playerStats = other.gameObject.GetComponent<PlayerStats>();
		if (playerStats != null)
		{
			switch (pickupType)
			{
				case PickupType.health:
					playerStats.Heal(value);
					break;
				case PickupType.armour:
					playerStats.ModArmour(value);
					break;
				case PickupType.bomb:
					playerStats.Hurt(value);
					break;
			}
			Destroy(gameObject);
		}
	}

}
