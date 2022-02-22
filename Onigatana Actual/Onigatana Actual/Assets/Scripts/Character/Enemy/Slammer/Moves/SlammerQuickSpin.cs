using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammerQuickSpin : SlammerMove
{
	[SerializeField] float qSpinSpeed = 300;
	[SerializeField] float qSpinArc = 360;
	[SerializeField] float windUpTime = 2;
	float windUpTimer = 0;
	float arcCounter = 0;

	public override bool Activate()
	{
		windUpTimer += Time.deltaTime;
		if (windUpTimer >= windUpTime)
		{
			transform.RotateAround(transform.position, transform.up, Time.deltaTime * qSpinSpeed);
			arcCounter += Time.deltaTime * qSpinSpeed;
			if (arcCounter >= qSpinArc)
			{
				return false;
				windUpTimer = 0;
			}
		}
		return true;
	}
}
