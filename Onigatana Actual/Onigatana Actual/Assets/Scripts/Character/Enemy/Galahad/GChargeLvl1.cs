using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// short wind up
/// Dashes towards player until she hits something
/// Deals dmg
/// </summary>
public class GChargeLvl1 : GalahadAction
{
	[SerializeField] float chargeUpTimerMax;
	float chargeUpTimer = 0;
	[SerializeField] float chargeSpeed;
	[SerializeField] int dmg;

	Vector3 chargePoint = Vector3.zero;
	PlayerStats player;
	bool performAction = false;

	Rigidbody rb;
	float lerpTimer = 0;
	private void Start()
	{
		player = FindObjectOfType<PlayerStats>();
		rb = GetComponent<Rigidbody>();
	}
	public override void Perform()
	{
		performAction = true;
	}
	private void Update()
	{
		if (performAction)
		{
			chargeUpTimer += Time.deltaTime;

			if (chargeUpTimer >= chargeUpTimerMax)
			{
				var hits = Physics.RaycastAll(transform.position, player.transform.position - transform.position);
				foreach(var hit in hits)
				{
					if(hit.collider.gameObject.tag != "Enemy" &&
					   hit.collider.gameObject.tag != "Player")
					{
						chargePoint = hit.point;
						break;
					}
				}
				chargeUpTimer = 0;
				rb.AddForce((chargePoint - transform.position)*chargeSpeed);
			}
			
		}
	}

	
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Enemy" &&
			other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") &&
			other.gameObject.tag != "Player")
		{
			finished = true;
			performAction = false;
			rb.velocity = Vector3.zero;
		}
		if(other.tag == "Player")
		{
			other.GetComponent<PlayerStats>().Hurt(dmg);
		}
	}
}


