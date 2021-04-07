using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public GameObject rangedAttackPrefab;
    public GameObject meleeAttackCollider;
    public GameObject rangedAttackSpawnLocation;

    public Vector2 forceForRangedAttack;

    public bool attacking = false;
    public bool rangeAttack = false;

    public float meleeAttackActiveTime = 1f;
    public float meleeAttackCooldown = 1f;

    public float rangedAttackCooldown = 1f;
    float rangedAttackInternalTimer = 0f;

    Animator playerAni;

	private void Start()
	{
        playerAni = GetComponent<Animator>();
    }
	public void HandleAttack(float attack)
    {
        if(attack == 0)
        {
            if (!attacking && !rangeAttack)
            {
                StartCoroutine(MeleeAttackToggle());
                attacking = true;
                AudioHandler.instance.PlaySound("PlayerMelee",1);
                playerAni.SetTrigger("Melee");
            }
        }
        if (attack == 1)
        {
            if (!rangeAttack && !attacking)
            {
                var temp = Instantiate(rangedAttackPrefab, rangedAttackSpawnLocation.transform.position, rangedAttackPrefab.transform.rotation);
                temp.GetComponent<Rigidbody2D>().AddForce(forceForRangedAttack);
                temp.GetComponent<RangedBullet>().lifeTimer = 0;
                rangeAttack = true;
                rangedAttackInternalTimer = 0f;
            } 
        }

    }


    private void Update()
    {
   
        if (rangeAttack)
        {
            rangedAttackInternalTimer += Time.deltaTime;
            if(rangedAttackInternalTimer >= rangedAttackCooldown)
            {
                rangeAttack = false;
                //rangedAttackInternalTimer = 0f;
            }
        }
    }

    public IEnumerator MeleeAttackToggle()
    {
        meleeAttackCollider.SetActive(true);
        yield return new WaitForSeconds(meleeAttackActiveTime);
        meleeAttackCollider.SetActive(false);
        yield return new WaitForSeconds(meleeAttackCooldown);
        
        attacking = false;
    }



}
