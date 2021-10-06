using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int health;

    //[SerializeField] Animation hitAni;
    [SerializeField] ParticleSystem blood;
    //[SerializeField] AudioSource weaponHit;

    public int Health
	{
        get { return health; }
        private set { health = value; }
	}
    
    public void Hurt(int dmg)
	{
        Health -= dmg;
        Debug.Log($"OUCHHHHHH! I took {dmg} damage");
        IsHit();
        if(isDead())
		{
            HUDCon.instance.UpdateKillCount();
            Destroy(gameObject);
		}
	}
    public void IsHit()
    {
        //hitAni.Play("Death");
        if (blood)
        {
            blood.Play();
        }
        AudioHandler.instance.PlaySound("SwordSlashHit");
    }

    private bool isDead()
	{
        if(Health <= 0)
		{
            return true;
		}
        return false;
	}

   
}
