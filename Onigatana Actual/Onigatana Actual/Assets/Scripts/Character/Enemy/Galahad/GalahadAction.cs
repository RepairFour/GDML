using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GalahadAction : MonoBehaviour
{
	[HideInInspector]
	public bool finished = false;
	public abstract void Perform();

}
