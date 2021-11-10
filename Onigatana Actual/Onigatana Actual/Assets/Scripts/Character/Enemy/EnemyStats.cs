﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int health;
    public enum MeleeAnimation
    {
        ANIMATION1,
        ANIMATION2
    }
    [SerializeField] ParticleSystem blood;    
    [Header("Animations")]
    public Animator enemyTestAnims;

    private bool InAnimation1;
    private bool InAnimation2;

    private GameObject Player;
    private Rigidbody enemyRigidbody;
    private float knockbackStrength;

    public Vector3 knockBackDirection;

    public Material defaultMaterial;
    public Material hurtMaterial;

	private void Start()
	{
        Player = GameObject.FindGameObjectWithTag("Player");
        enemyRigidbody = gameObject.GetComponent<Rigidbody>();
    }
	public int Health
	{
        get { return health; }
        private set { health = value; }
	}
    
    public void Hurt(int dmg, MeleeAnimation ani)
	{
        Health -= dmg;
        Debug.Log($"OUCHHHHHH! I took {dmg} damage");
        IsHit();
        EnemyAnimateHit(ani);
        if (isDead())
		{
            HUDCon.instance.UpdateKillCount();
            AudioHandler.instance.PlaySound("EnemyDeath",1f,true,1);
            GameManager.instance.bloodFuryState.FillBloodMeter(3.5f);
            if(GameManager.instance.bloodFuryState.active)
			{
                GameManager.instance.bloodFuryState.Revive();
            }
            Destroy(gameObject);
		}
	}
    public void IsHit()
    {
        //hitAni.Play("Death");
        if (blood != null)
        {
            blood.Play();
        }
        AudioHandler.instance.PlaySound("SwordSlashHit");
        GameManager.instance.bloodFuryState.FillBloodMeter(1.5f);
    }

    private bool isDead()
	{
        
        if (Health <= 0)
		{
            return true;
		}
        return false;
	}

    public void EnemyAnimateHit(MeleeAnimation ani)
    {
        gameObject.GetComponent<Animator>().enabled = true;
        if (ani == MeleeAnimation.ANIMATION1)
        {
            InAnimation1 = true;
            InAnimation2 = false;
            enemyTestAnims.SetTrigger("Attacked1");
        }
        else if (ani == MeleeAnimation.ANIMATION2)
        {
            InAnimation1 = false;
            InAnimation2 = true;
            enemyTestAnims.SetTrigger("Attacked2");
        }
        if (GetComponent<GalahadLvl1>() == null)
        {
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = hurtMaterial;
        }
		else
		{
            gameObject.GetComponent<MeshRenderer>().material = hurtMaterial;
        }


        //Here we calculate from what position the player hits the enemy, in order to calculate what direction the knockback will trigger in later. We also increase the knockback strength, but it is capped at a certain amount.
        //enemyRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        knockBackDirection = enemyRigidbody.transform.position - Player.transform.position;
        knockbackStrength = knockbackStrength + 150;
        if (knockbackStrength >= 650)
        {
            knockbackStrength = 650;
        }

        //We activate their animation, and we cancel any reset invokes. We do this so that we can continually keep the enemy in hitstun/animations until we stop attacking, and then we send them back to normal.

        CancelInvoke("resetToNormal");

        //After all this, we reset their entire state back to normal in a few frames (only slightly longer than the max attack speed of the sword). This gets cancelled if another attack is launched, in order to allow players to continually attack the same enemy.
        //We reset materials after only 1/10th of a second, to give a satisfying hit effect.
        Invoke("resetToNormal", 0.25f);
        Invoke("resetMaterial", 0.1f);


    }

    private void resetToNormal()
    {

        //When we reset back to normal, we clear the states, change the material back and reset the animation to default. We also calculate, perform and reset knockback, finally disabling the animator.

        InAnimation1 = false;
        InAnimation2 = false;
        if (GetComponent<GalahadLvl1>() == null)
        {
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = defaultMaterial;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
        enemyTestAnims.SetTrigger("Reset");

        Debug.Log(knockbackStrength);
        //enemyRigidbody.constraints = ~RigidbodyConstraints.FreezePositionX | ~RigidbodyConstraints.FreezePositionZ;
        enemyRigidbody.AddForce(knockBackDirection.normalized * knockbackStrength);
        knockbackStrength = 0;
        gameObject.GetComponent<Animator>().enabled = false;

    }


    //Simple invoked function to reset the material after a short timer.
    private void resetMaterial()
    {
        if (GetComponent<GalahadLvl1>() == null)
        {
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = defaultMaterial;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
    }
}
