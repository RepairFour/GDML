using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack4 : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<Player>() != null)
		{
			Player.instance.TakeDamage(1);
		}
	}
}
