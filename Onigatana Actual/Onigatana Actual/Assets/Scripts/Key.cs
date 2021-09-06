using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
	int id = 0;
	public void SetId(int _id)
	{
		id = _id;
	}
	public int GetId()
	{
		return id;
	}
	private void OnTriggerEnter(Collider other)
	{
		var playerInventory = other.GetComponent<PlayerInventory>();
		if (playerInventory != null)
		{
			playerInventory.AddKey(this);
			Destroy(gameObject);
				
		}
	}
}
