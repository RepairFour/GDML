using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : MonoBehaviour
{
    public int health;

	//returns true if the object has been destroyed
	public bool Hurt(int dmg)
	{
		health -= dmg;
		if(health <= 0 )
		{
			Destroy(gameObject);
			return true;
		}
		return false;
	}
}
