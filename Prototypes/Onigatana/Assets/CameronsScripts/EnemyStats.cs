using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int health;
    public int Health
	{
        get { return health; }
        private set { health = value; }
	}
    
    public void Hurt(int dmg)
	{
        Health -= dmg;
        Debug.Log($"OUCHHHHHH! I took {dmg} damage");
	}

   
}
