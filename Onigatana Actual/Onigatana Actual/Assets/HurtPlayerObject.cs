using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayerObject : MonoBehaviour
{
    public int dmg;
	public bool timed;
	public float duration;
	float timer;
	bool hit;
	private void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<PlayerStats>() != null)
		{
			other.GetComponent<PlayerStats>().Hurt(dmg);
		}
		Debug.Log(other.name);
	}

	private void Update()
	{
		if (timed)
		{
			timer += Time.deltaTime;
			if (timer > duration)
			{
				timer = 0;
				gameObject.SetActive(false);
			}
		}
	}
}
