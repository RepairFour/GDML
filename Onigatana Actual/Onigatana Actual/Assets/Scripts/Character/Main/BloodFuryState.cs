using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFuryState : MonoBehaviour
{
	[HideInInspector]
	public bool active = false;

	[SerializeField] float maxBlood;
	[SerializeField] float bloodDecayTimerMax;
	[SerializeField] float bloodLostPerInterval;

	float bloodDecayTimer = 0;
	[HideInInspector]
	public float currentBlood;
	bool lateStart = true;

	private void Update()
	{
		if(lateStart)
		{
			lateStart = false;
			currentBlood = 50;
			HUDCon.instance.Initialise(maxBlood);
		}
		if(active)
		{
			bloodDecayTimer += Time.deltaTime;
			if(bloodDecayTimer >= bloodDecayTimerMax)
			{
				bloodDecayTimer = 0;
				DrainBloodMeter(bloodLostPerInterval);
			}
		}
	}
	public bool EnterStateCheck()
	{
		if (currentBlood > 0)
		{
			Debug.Log("Entering blood fury");
			active = true;
		}
		else 
		{
			Debug.Log("Cannot enter blood fury, current blood == 0");
		}
		return active;
	}
	public void ExitState()
	{
		Debug.Log("Exiting BloodFury");
		active = false;
	}
	public void FillBloodMeter(float amount)
	{
		currentBlood += amount;
		if (currentBlood > maxBlood)
		{
			currentBlood = maxBlood;
		}
		HUDCon.instance.UpdateBloodBar();
	}
	public void DrainBloodMeter(float amount)
	{
		currentBlood -= amount;
		if (currentBlood < 0)
		{
			currentBlood = 0;
		}
		HUDCon.instance.UpdateBloodBar();
		//come out of state if you have no blood
		if (currentBlood < 0)
		{
			currentBlood = 0;
		}
		if (currentBlood == 0)
		{
			ExitState();
		}
	}

	public void Revive()
	{
		GameManager.instance.playerStats.Heal(Mathf.RoundToInt(currentBlood));
	}
}
