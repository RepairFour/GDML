using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayerObject : MonoBehaviour
{
    public int dmg;
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.GetComponent<PlayerStats>().Hurt(dmg);
		}
	}
}
