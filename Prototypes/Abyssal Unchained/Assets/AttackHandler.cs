using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public GameObject meleeAttackCollider;

    public bool attacking = false;

    public float meleeAttackActiveTime = 1f;
    public float meleeAttackCooldown = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!attacking)
            {
                StartCoroutine(MeleeAttackToggle());
                attacking = true;
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
