using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
	public int dmgPerSecond = 10;
	Timer timer;

	private void Start()
	{
		timer = new Timer(1);
	}
	private void Update()
	{
		timer.UpdateTimer();
	}
	private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			if (timer.IsFinished())
			{
				PlayerStats player = other.GetComponent<PlayerStats>();
				player.Hurt(dmgPerSecond);
				timer.ResetTimer();
			}
		}
	}
}
