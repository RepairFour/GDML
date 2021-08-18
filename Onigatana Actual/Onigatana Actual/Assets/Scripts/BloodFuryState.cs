using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFuryState : MonoBehaviour
{
	[HideInInspector]
	public bool active = false;
    public void EnterState()
	{
		Debug.Log("Entering BloodFury");
		active = true;
	}
	public void ExitState()
	{
		Debug.Log("Exiting BloodFury");
		active = false;
	}
}
