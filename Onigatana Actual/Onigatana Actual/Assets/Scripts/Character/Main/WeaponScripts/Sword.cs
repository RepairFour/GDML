using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    GameObject targetedEnemy;
    GameObject blinkTarget;
    bool strikeLocked;
    int animationCounter = 0;

    [Header("Animation and Particles")]
    public Animator animator;
    public ParticleSystem weaponTrail;
    public ParticleSystem attackHitEffect;
    public ParticleSystem blood;
    public GameObject chargeTrail;
    public GameObject chargeParticles;
    public List<string> attacks = new List<string>();
    public MeshRenderer swordRenderer;
    
    [Header("Charge Attack Stats")]
    public float dashSpeed;
    public float chargeAttackStoppingDistance;

    [Header ("Charge Attack Particle")]
    public float maxElectricityAmount;
    float currentElectricityAmount;




    #region ChargeAttack

    protected override void Update()
    {
        base.Update();
        HandleChargingAttack();
        HandleChargeAttackEffect();
        HandleAttacks();
        HandleAttackCleanUp();
    }

    protected override void ChargeAttackFeelerRay()
    {
        if (attackCharged && canAttack)
        {

            RaycastHit hit;
            if (Physics.BoxCast(feelerPoint.position, feelerLowTierHalfExtents, feelerPoint.forward, out hit, Quaternion.identity, chargeAttackRange, chargeAttackMask))
            {
                if (blinkTarget != hit.collider.gameObject)
                {
                    if (blinkTarget != null)
                    {
                        blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    }
                    blinkTarget = hit.collider.gameObject;
                    targetedEnemy = blinkTarget;
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 5f;
                    strikeLocked = true;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, feelerMidTierExtents, feelerPoint.forward, out hit, Quaternion.identity, chargeAttackRange, chargeAttackMask))
            {
                if (blinkTarget != hit.collider.gameObject)
                {
                    if (blinkTarget != null)
                    {
                        blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    }
                    blinkTarget = hit.collider.gameObject;
                    targetedEnemy = blinkTarget;
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 5f;
                    strikeLocked = true;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, feelerOuterTierExtents, feelerPoint.forward, out hit, Quaternion.identity, chargeAttackRange, chargeAttackMask))
            {
                if (blinkTarget != hit.collider.gameObject)
                {
                    if (blinkTarget != null)
                    {
                        blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    }
                    blinkTarget = hit.collider.gameObject;
                    targetedEnemy = blinkTarget;
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 5f;
                    strikeLocked = true;
                }
            }
            else
            {
                if (blinkTarget != null)
                {
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    blinkTarget = null;
                    targetedEnemy = null;
                    strikeLocked = false;
                }
            }

        }
    }

    protected override void HandleChargeAttackEffect()
    {
        HandleDashToTarget();
        CheckIfDashing();
    }
    void HandleDashToTarget()
    {
        if (attackCharged && attackQueued && strikeLocked)
        {
            controller.BlinkToPosition(blinkTarget.transform.position, dashSpeed, chargeAttackStoppingDistance);
            strikeLocked = false;
        }
    }
    void CheckIfDashing()
    {
        if (!controller.isBlinkStrikeActivated && !strikeLocked)
        {
            if (blinkTarget != null)
            {
                blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
            }
            blinkTarget = null;
        }
    }

    #endregion

    #region BasicAttack
    protected override void HandleTarget()
    {
        RaycastHit hit;
        //targetedEnemy = null;
        if (targetedEnemy == null)
        {
            if (Physics.BoxCast(feelerPoint.position, attackFeelerLowTierHalfExtents, feelerPoint.forward, out hit, Quaternion.identity, attackRange, attackMask))
            {

                if (targetedEnemy != hit.collider.gameObject)
                {
                    targetedEnemy = hit.collider.gameObject;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, attackFeelerMidTierExtents, feelerPoint.forward, out hit, Quaternion.identity, attackRange, attackMask))
            {
                if (targetedEnemy != hit.collider.gameObject)
                {
                    targetedEnemy = hit.collider.gameObject;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, attackFeelerOuterTierExtents, feelerPoint.forward, out hit, Quaternion.identity, attackRange, attackMask))
            {
                if (targetedEnemy != hit.collider.gameObject)
                {
                    targetedEnemy = hit.collider.gameObject;
                }
            }
        }

        if (targetedEnemy != null)
        {
            RaycastHit occulusionTest;
            if (Physics.Raycast(transform.position, targetedEnemy.transform.position - transform.position,
                out occulusionTest, Vector3.Distance(targetedEnemy.transform.position, transform.position)))
            {
                if (!occulusionTest.transform.gameObject.CompareTag("Enemy"))
                {
                    targetedEnemy = null;
                    return;
                }
            }



            if (targetedEnemy.CompareTag("Enemy"))
            {
                var enemyStats = targetedEnemy.GetComponent<EnemyStats>();
                if (enemyStats.hasShield == false)
                {
                    enemyStats.Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION1);
                    for (int i = 0; i < GameManager.instance.playerController.gameObject.GetComponent<CrimsonFlourish>().getLinkedEnemies().Count; i++)
                    {
                        if (GameManager.instance.playerController.gameObject.GetComponent<CrimsonFlourish>().getLinkedEnemies()[i] == enemyStats)
                        {
                            continue;
                        }
                        else
                        {
                            GameManager.instance.playerController.gameObject.GetComponent<CrimsonFlourish>().getLinkedEnemies()[i].Hurt(Mathf.RoundToInt(damage * GameManager.instance.playerController.gameObject.GetComponent<CrimsonFlourish>().damageModifer), EnemyStats.MeleeAnimation.ANIMATION1);
                        }
                    }
                    Instantiate(blood, targetedEnemy.GetComponent<Collider>().ClosestPoint(transform.position), Quaternion.identity);
                    Instantiate(attackHitEffect, targetedEnemy.GetComponent<Collider>().ClosestPoint(transform.position), Quaternion.identity);
                }



                else
                {
                    //Is player infront of enemy
                    if (Vector3.Dot((GameManager.instance.playerStats.transform.position - targetedEnemy.gameObject.transform.position).normalized,
                                   targetedEnemy.gameObject.transform.forward) > 0)
                    {
                        var shield = targetedEnemy.GetComponentInChildren<ShieldHealth>();
                        if (shield.Hurt(damage))//if the shield dies then
                        {
                            enemyStats.hasShield = false;
                        }

                        else
                        {
                            enemyStats.Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION1);
                            Instantiate(blood, targetedEnemy.GetComponent<Collider>().ClosestPoint(transform.position), Quaternion.identity);
                            Instantiate(attackHitEffect, targetedEnemy.GetComponent<Collider>().ClosestPoint(transform.position), Quaternion.identity);
                        }

                    }


                    else if (targetedEnemy.CompareTag("Shield"))
                    {
                        var shield = gameObject.GetComponentInParent<ShieldHealth>();
                        if (shield.Hurt(damage))//if the shield dies then
                        {
                            shield.GetComponentInParent<EnemyStats>().hasShield = false;
                        }

                    }

                    else if (targetedEnemy.CompareTag("Well"))
                    {
                        targetedEnemy.GetComponent<Well>().Hurt(damage);
                    }

                    targetedEnemy = null;
                }
            }
        }
    }
    #endregion

    #region Animation
    protected override void BasicAttackAnimation() {
        animator.SetBool("Attacking", true);
        animator.SetTrigger(attacks[animationCounter]);
        
        animationCounter++;
        if (animationCounter > attacks.Count - 1)
        {
            animationCounter = 0;
        }
        weaponTrail.Play();
    }
    protected override void ResetChargeAnimation() {
        animator.SetBool("ChargeStart", false);
        animator.SetBool("Charging", false);
    }
    protected override void ChargeAttackAnimation() 
    {
        animator.SetTrigger("ChargeAttack");
        animator.SetBool("Charging", false);
        animator.SetBool("ChargeStart", false);
    }
    protected override void ChargingAttackAnimation() 
    {
        animator.SetBool("Charging", true);
    }
    protected override void ChargeStartAnimation() 
    { 
        animator.SetBool("ChargeStart", true); 
    }
    #endregion

    #region Sound
    protected override void BasicAttackSound() {
        AudioHandler.instance.PlaySound("SwordSlash2", 1, true, 3);
    }
    protected override void ChargeAttackSound() 
    {
        AudioHandler.instance.PlaySound("SwordSlash2", 1, true, 3);
    }
    #endregion

    #region Particles
    protected override void TurnOnChargeAttackParticles() 
    {
        swordRenderer.materials[1].SetFloat("electric_amount", 5);
        chargeParticles.SetActive(true);
    }
    protected override void TurnOffChargeAttackParticles() {
        chargeParticles.SetActive(false);
        swordRenderer.materials[1].SetFloat("electric_amount", 0);
    }
    #endregion
}
