using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		var playerInventory = other.GetComponent<PlayerInventory>();
		if (playerInventory != null)
		{
			playerInventory.AddKey(this);
			GetComponent<CapsuleCollider>().enabled = false;
			FindObjectOfType<HUDCon>().FillSlot(gameObject);
			//Destroy(gameObject);				
		}
	}
}
