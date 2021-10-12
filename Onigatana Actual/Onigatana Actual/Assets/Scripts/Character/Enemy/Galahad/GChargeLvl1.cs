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

	Color objectColor;
	Material mat;
	//Vector3 orginalScale;
	private void Start()
	{
		player = FindObjectOfType<PlayerStats>();
		rb = GetComponent<Rigidbody>();
		mat = GetComponent<MeshRenderer>().material;
		objectColor = mat.color;
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
			mat.color = Color.blue;
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
				mat.color = objectColor;
				rb.AddForce((chargePoint - transform.position)*chargeSpeed);
				
				finished = true;
				performAction = false;
			}
			
		}
	}

	
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Enemy" &&
			other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") &&
			other.gameObject.tag != "Player")
		{
			
			rb.velocity = Vector3.zero;
		}
		if(other.tag == "Player")
		{
			other.GetComponent<PlayerStats>().Hurt(dmg);
		}
	}
}


