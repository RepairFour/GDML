using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kenobo : Weapon
{
    GameObject targetedEnemy;
    List<GameObject> highlightedEnemies = new List<GameObject>();
    List<GameObject> targetedEnemies = new List<GameObject>();
    List<GameObject> pushBackTargets = new List<GameObject>();
    float range;

    int animationCounter = 0;

    [Header("Sword Basic Attack Hitbox")]
    [SerializeField] protected Vector3 cleaveAttackHitBox;
    [Space]
    [Header("Sword Charge Attack Hitbox")]
    [SerializeField] protected Vector3 chargeAttackHitBox;



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
    public float pushbackForce;
    public float knockbackDuration;
    public float knockbackDistance;
    public float knockbackHeight;
    public int knockbackWallSmashDamage;

    [Header("Charge Attack Particle")]
    public float maxElectricityAmount;
    float currentElectricityAmount;

    protected override void Update()
    {
        base.Update();
        HandleChargingAttack();
        HandleAttacks();
        HandleAttackCleanUp();

        DrawBoxCast.DrawBoxCastBox(feelerPoint.position, chargeAttackHitBox, feelerPoint.rotation, feelerPoint.forward, chargeAttackRange, Color.red);
        DrawBoxCast.DrawBoxCastBox(feelerPoint.position, cleaveAttackHitBox, feelerPoint.rotation, feelerPoint.forward, attackRange, Color.green);
    }

    #region ChargeAttack
    protected override void ChargeAttackFeelerRay()
    {
        if (attackCharged && canAttack)
        {
            List<RaycastHit> hits = new List<RaycastHit>();
            //List<RaycastHit> mid = new List<RaycastHit>();
            //List<RaycastHit> outer = new List<RaycastHit>();

            hits.AddRange(Physics.BoxCastAll(feelerPoint.position, chargeAttackHitBox, feelerPoint.forward, feelerPoint.rotation, chargeAttackRange, attackMask));
            //hits.AddRange(Physics.BoxCastAll(feelerPoint.position, feelerMidTierExtents, feelerPoint.forward, Quaternion.identity, attackRange, attackMask));
            //hits.AddRange(Physics.BoxCastAll(feelerPoint.position, feelerOuterTierExtents, feelerPoint.forward, Quaternion.identity, attackRange, attackMask));

            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    if (!highlightedEnemies.Contains(hits[i].collider.gameObject))
                    {
                        highlightedEnemies.Add(hits[i].collider.gameObject);
                    }
                }
            }

            if (highlightedEnemies.Count > 0)
            {
                foreach (GameObject c in highlightedEnemies)
                {
                    bool t = false;
                    for (int i = 0; i < hits.Count; i++)
                    {
                        if (c == hits[i].collider.gameObject)
                        {
                            t = true;
                        }
                    }
                    if (!t)
                    {
                        c.GetComponent<Outline>().OutlineWidth = 0f;
                        highlightedEnemies.Remove(c);
                    }
                }


                foreach (GameObject e in highlightedEnemies)
                {
                    RaycastHit occulusionTest;
                    if (Physics.Raycast(transform.position, e.transform.position - transform.position,
                        out occulusionTest, Vector3.Distance(e.transform.position, transform.position)))
                    {
                        if (!occulusionTest.transform.gameObject.CompareTag("Enemy"))
                        {
                            highlightedEnemies.Remove(e);
                        }
                    }
                }

                foreach(GameObject h in highlightedEnemies)
                {
                    h.GetComponent<Outline>().OutlineWidth = 5f;
                }

            }
        }
    }

    protected override void HandleChargeAttackEffect()
    {
        if (attackCharged && canAttack)
        {
            foreach (GameObject g in pushBackTargets)
            {
                var direction = g.transform.localPosition - GameManager.instance.playerController.transform.localPosition;
                

                var ec = g.GetComponent<EnemyChase>();
                ec.setKnockbackDirection = direction;
                ec.isGettingKnockbacked = true;
                ec.setKnockbackDuration = knockbackDuration;
                ec.setKnockbackDistance = knockbackDistance;
                ec.setKnockbackHeight = knockbackHeight;
                ec.setWallSmashDamage = knockbackWallSmashDamage;
                g.GetComponent<Outline>().OutlineWidth = 0f;
                Debug.Log("Knocking back");
            }
        }
    }
    #endregion

    #region BasicAttack
    protected override void HandleTarget()
    {
        if (targetedEnemies.Count == 0)
        {
            RaycastHit[] inner;
            RaycastHit[] mid;
            RaycastHit[] outer;

            if (attackCharged)
            {
                range = chargeAttackRange;
                inner = Physics.BoxCastAll(feelerPoint.position, chargeAttackHitBox, feelerPoint.forward, feelerPoint.rotation, range, attackMask);
                //mid = Physics.BoxCastAll(feelerPoint.position, feelerMidTierExtents, feelerPoint.forward, Quaternion.identity, range, attackMask);
                //outer = Physics.BoxCastAll(feelerPoint.position, feelerOuterTierExtents, feelerPoint.forward, Quaternion.identity, range, attackMask);
                
            }
            else
            {
                range = attackRange;
                inner = Physics.BoxCastAll(feelerPoint.position, cleaveAttackHitBox, feelerPoint.forward, feelerPoint.rotation, range, attackMask);
                //mid = Physics.BoxCastAll(feelerPoint.position, attackFeelerMidTierExtents, feelerPoint.forward, Quaternion.identity, range, attackMask);
                //outer = Physics.BoxCastAll(feelerPoint.position, attackFeelerOuterTierExtents, feelerPoint.forward, Quaternion.identity, range, attackMask);
            }
            if (inner.Length > 0)
            {
                for(int i = 0; i < inner.Length; i++)
                {
                    if (!targetedEnemies.Contains(inner[i].collider.gameObject))
                    {
                        targetedEnemies.Add(inner[i].collider.gameObject);
                    }
                }

            }
            //if (mid.Length > 0)
            //{
            //    for (int i = 0; i < mid.Length; i++)
            //    {
            //        if (!targetedEnemies.Contains(mid[i].collider.gameObject))
            //        {
            //            targetedEnemies.Add(mid[i].collider.gameObject);
            //        }
            //    }
            //}
            //if (outer.Length > 0)
            //{
            //    for (int i = 0; i < outer.Length; i++)
            //    {
            //        if (!targetedEnemies.Contains(outer[i].collider.gameObject))
            //        {
            //            targetedEnemies.Add(outer[i].collider.gameObject);
            //        }
            //    }
            //}
        }

        if (targetedEnemies.Count > 0)
        {
            foreach (GameObject e in targetedEnemies)
            {
                RaycastHit occulusionTest;
                if (Physics.Raycast(transform.position, e.transform.position - transform.position,
                    out occulusionTest, Vector3.Distance(e.transform.position, transform.position)))
                {
                    if (!occulusionTest.transform.gameObject.CompareTag("Enemy"))
                    {
                        targetedEnemies.Remove(e);
                    }
                }
            }
            pushBackTargets = targetedEnemies;
            HandleChargeAttackEffect();

            if (targetedEnemies.Count > 0)
            {
                foreach (GameObject e in targetedEnemies)
                {
                    var enemyStats = e.GetComponent<EnemyStats>();
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
                        Instantiate(blood, e.GetComponent<Collider>().ClosestPoint(transform.position), Quaternion.identity);
                        Instantiate(attackHitEffect, e.GetComponent<Collider>().ClosestPoint(transform.position), Quaternion.identity);
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
                targetedEnemies.Clear();
            }
        }
    }
    #endregion

    #region Animation
    protected override void BasicAttackAnimation()
    {
        animator.SetBool("Attacking", true);
        animator.SetTrigger(attacks[animationCounter]);

        animationCounter++;
        if (animationCounter > attacks.Count - 1)
        {
            animationCounter = 0;
        }
        weaponTrail.Play();
    }
    
    protected override void ResetChargeAnimation()
    {

    }
    protected override void ChargeAttackAnimation()
    {
       
    }
    protected override void ChargingAttackAnimation()
    {
       
    }
    protected override void ChargeStartAnimation()
    {
        
    }
    #endregion

    #region Sound
    protected override void BasicAttackSound()
    {
        AudioHandler.instance.PlaySound("SwordSlash2", 1, true, 3);
    }
    protected override void ChargeAttackSound()
    {
       
    }
    #endregion

    #region Particles
    protected override void TurnOnChargeAttackParticles()
    {
        
    }
    protected override void TurnOffChargeAttackParticles()
    {
        
    }
    #endregion
}
