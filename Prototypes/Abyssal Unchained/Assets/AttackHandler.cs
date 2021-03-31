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

    public void HandleAttack(float attack)
    {
        if(attack == 0)
        {
            if (!attacking)
            {
                StartCoroutine(MeleeAttackToggle());
                attacking = true;
            }
        }
        if (attack == 1)
        {
            if (!rangeAttack)
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
        //if (Input.GetButtonDown("MeleeAttack"))
        //{
        //    if (!attacking)
        //    {
        //        StartCoroutine(MeleeAttackToggle());
        //        attacking = true;
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    if (!rangeAttack)
        //    {
        //        var temp = Instantiate(rangedAttackPrefab, rangedAttackSpawnLocation.transform.position, Quaternion.identity);
        //        temp.GetComponent<Rigidbody2D>().AddForce(forceForRangedAttack);
        //        rangeAttack = true;
        //        rangedAttackInternalTimer = 0f;
        //    }
        //}

        if (rangeAttack)
        {
            rangedAttackInternalTimer += Time.deltaTime;
            if(rangedAttackInternalTimer >= rangedAttackCooldown)
            {
                rangeAttack = false;
                rangedAttackInternalTimer = 0f;
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
