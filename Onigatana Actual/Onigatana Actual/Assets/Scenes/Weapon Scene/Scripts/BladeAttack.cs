using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;


public class BladeAttack : MonoBehaviour
{
    public float slowdownOnCharge;
    public ParticleSystem chargeParticles;
    public ParticleSystem chargedParticles;
    [SerializeField] int damage;
    PlayerMap input;
    private ButtonControl meleeButton;
    bool buttonPressed;

    public Animator animator;
    public ParticleSystem weaponTrail;

    public List<MeshRenderer> rangedWeapon = new List<MeshRenderer>();
    public GunBase rangedWeaponScript;
    public MeshRenderer meleeWeapon;
    public SkinnedMeshRenderer meleeHand;

    public BoxCollider attackCollider;
    public Transform feelerPoint;
    GameObject blinkTarget;
    public Vector3 feelerLowTierHalfExtents;
    public Vector3 feelerMidTierExtents;
    public Vector3 feelerOuterTierExtents;
    public float feelerRange;
    public LayerMask feelerMask;
    private Vector3 strikeHit;
    private bool strikeLocked;
    public float blinkSpeed;

    private bool attackAnimation1;
    private bool attack1Queued;
    private bool attack2Queued;
    private bool chargeAttackQueued;
    private bool attackAnimation2;
    private bool chargeAttackAnimation;
    private float cleanUpTimer;
    public float cleanUpTime;
    public float attackSpeed;
    public float chargeAttackStoppingDistance;
    private float attackTimer;
    bool canAttack = true;

    public float chargeAttackTime;
    float chargeAttackTimer;
    bool attackCharged;

    public ParticleSystem attackHitEffect;
    public ParticleSystem blood;

    public Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        input = new PlayerMap();
        input.Enable();

        meleeButton = (ButtonControl)input.Player.Melee.controls[0];

        attackCollider.enabled = false;

        meleeWeapon.enabled = true;
        meleeHand.enabled = true;
        foreach (MeshRenderer r in rangedWeapon)
        {
            r.enabled = false;
        }
        rangedWeaponScript.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (meleeButton.isPressed)
        {
            meleeWeapon.enabled = true;
            meleeHand.enabled = true;
            foreach (MeshRenderer r in rangedWeapon)
            {
                r.enabled = false;
            }

        }
        AttackButtonHeld();
        QueueAttack();
        CheckStrikeLocked();
        FeelerRay();
        HandleStrikeLocked();
        RunAnimation1();
        RunAnimation2();
        RunChargeAnimation();
        AttackTimer();
        CleanUpAttack();
    }
    void AttackTimer()
    {
        if(canAttack == false)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackSpeed)
            {
                canAttack = true;
                attackTimer = 0;
                attackCollider.enabled = false;
            }
        }
    }
    void CleanUpAttack()
    {
        if(attackAnimation1 || attackAnimation2 || chargeAttackAnimation)
        {
            cleanUpTimer += Time.deltaTime;
            if(cleanUpTimer >= cleanUpTime)
            {
                animator.SetTrigger("reset");
                cleanUpTimer = 0;
                
                attackAnimation1 = false;
                attackAnimation2 = false;
                
                attack1Queued = false;
                attack2Queued = false;
                chargeAttackQueued = false;
                
                foreach (MeshRenderer r in rangedWeapon)
                {
                    r.enabled = false;
                }
                
                rangedWeaponScript.enabled = true;
                meleeWeapon.enabled = true;
                meleeHand.enabled = true;
                canAttack = true;
                attackTimer = 0;
                attackCollider.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (attackAnimation1)
            {
                other.GetComponent<EnemyStats>().Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION1);

            }
            else if (attackAnimation2)
            {
                other.GetComponent<EnemyStats>().Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION2);

            }


            Instantiate(blood, other.ClosestPoint(transform.position), Quaternion.identity);
            Instantiate(attackHitEffect, other.ClosestPoint(transform.position), Quaternion.identity);
            attackCollider.enabled = false;
        }
    }
    private void FeelerRay()
    {
        if (attackCharged && !strikeLocked && canAttack)
        {
            DrawBoxCast.DrawBoxCastBox(feelerPoint.position, feelerLowTierHalfExtents,transform.rotation, transform.forward, feelerRange, Color.red);
            DrawBoxCast.DrawBoxCastBox(feelerPoint.position, feelerMidTierExtents, transform.rotation, transform.forward, feelerRange, Color.blue);
            DrawBoxCast.DrawBoxCastBox(feelerPoint.position, feelerOuterTierExtents, transform.rotation, transform.forward, feelerRange, Color.green);
            RaycastHit hit;
            if (Physics.BoxCast(feelerPoint.position, feelerLowTierHalfExtents, feelerPoint.forward, out hit, Quaternion.identity, feelerRange, feelerMask))
            {
                if (blinkTarget != hit.collider.gameObject)
                {
                    if (blinkTarget != null)
                    {
                        blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    }
                    blinkTarget = hit.collider.gameObject;
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 5f;
                }

                if (chargeAttackQueued)
                {
                    strikeLocked = true;
                    chargeAttackAnimation = true;
                    chargeAttackQueued = false;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, feelerMidTierExtents, feelerPoint.forward, out hit, Quaternion.identity, feelerRange, feelerMask))
            {
                if (blinkTarget != hit.collider.gameObject)
                {
                    if (blinkTarget != null)
                    {
                        blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    }
                    blinkTarget = hit.collider.gameObject;
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 5f;
                }

                if (chargeAttackQueued)
                {
                    strikeLocked = true;
                    chargeAttackAnimation = true;
                    chargeAttackQueued = false;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, feelerOuterTierExtents, feelerPoint.forward, out hit, Quaternion.identity, feelerRange, feelerMask))
            {
                if (blinkTarget != hit.collider.gameObject)
                {
                    if (blinkTarget != null)
                    {
                        blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    }
                    blinkTarget = hit.collider.gameObject;
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 5f;
                }

                if (chargeAttackQueued)
                {
                    strikeLocked = true;
                    chargeAttackAnimation = true;
                    chargeAttackQueued = false;
                }
            }
            else
            {
                if (blinkTarget != null)
                {
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    blinkTarget = null;
                }
                if (chargeAttackQueued)
                {
                    chargeAttackQueued = false;
                    chargeAttackAnimation = true;

                }
            }
        }
        
    }
    private void HandleStrikeLocked()
    {
        if (strikeLocked || controller.isBlinkStrikeActivated)
        {
            controller.BlinkToPosition(blinkTarget.transform.position, blinkSpeed, chargeAttackStoppingDistance);
        }
       
    }
    private void CheckStrikeLocked()
    {
        Debug.Log(strikeLocked);
        if (strikeLocked == true && controller.isBlinkStrikeActivated == false)
        {
            strikeLocked = false;
        }
    }

    private void RunAnimation1()
    {
        if (controller.isBlinkStrikeActivated == false 
            && attack1Queued 
            && !attackAnimation1
            && !strikeLocked)
        {
            weaponTrail.Play();
            attack1Queued = false;
            attack2Queued = false;
            attackCollider.enabled = true;
            attackAnimation1 = true;
            attackAnimation2 = false;
            cleanUpTimer = 0;
            canAttack = false;
            animator.SetTrigger("Attack1");
            AudioHandler.instance.PlaySound("SwordSlash1", 1, true, 2);
            chargeAttackTimer = 0;
        }
    }
    private void RunAnimation2()
    {
        if (GetComponentInParent<Controller>().isBlinkStrikeActivated == false 
            && attack2Queued 
            && !attackAnimation2
            && !strikeLocked)
        {
            weaponTrail.Play();
            strikeLocked = false;
            attackCollider.enabled = true;
            AudioHandler.instance.PlaySound("SwordSlash2", 1, true, 2);
            animator.SetTrigger("Attack2");
            attack1Queued = false;
            attack2Queued = false;
            attackAnimation1 = false;
            attackAnimation2 = true;
            cleanUpTimer = 0;
            canAttack = false;
            chargeAttackTimer = 0;
        }
    }
    private void RunChargeAnimation()
    {
        if (controller.isBlinkStrikeActivated == false
           && chargeAttackAnimation
           && !strikeLocked)
        {
            weaponTrail.Play();
            attack1Queued = false;
            attack2Queued = false;
            chargeAttackQueued = false;
            attackCollider.enabled = true;
            attackAnimation1 = true;
            attackAnimation2 = false;
            chargeAttackAnimation = false;
            attackCharged = false;
            cleanUpTimer = 0;
            canAttack = false;
            animator.SetTrigger("Attack1");
            AudioHandler.instance.PlaySound("SwordSlash1", 1, true, 2);
            
            if (blinkTarget != null)
            {
                blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
            }
            blinkTarget = null;
            chargeAttackTimer = 0;
        }
    }
    private void AttackButtonHeld()
    {
        if (meleeButton.wasPressedThisFrame)
        {
            buttonPressed = true;
        }
        if (meleeButton.wasReleasedThisFrame)
        {
            buttonPressed = false;
            
        }
        if (!meleeButton.isPressed)
        {
            chargeAttackTimer = 0f;
            controller.ToggleChargingAttack(1, false);
        }
    }
    private void QueueAttack()
    {
        if (meleeButton.isPressed)
        {
            chargeAttackTimer += Time.deltaTime;
            
            if (chargeAttackTimer >= 0.09)
            {
                chargeParticles.gameObject.SetActive(true);
                controller.ToggleChargingAttack(slowdownOnCharge, true);
            }
            if(chargeAttackTimer >= chargeAttackTime)
            {
                chargeAttackTimer = chargeAttackTime;
                attackCharged = true;
                chargeParticles.gameObject.SetActive(false);
                chargedParticles.gameObject.SetActive(true);

            }
            
        }
        if (meleeButton.wasPressedThisFrame && canAttack)
        {
            chargeParticles.gameObject.SetActive(false);
            chargedParticles.gameObject.SetActive(false);
            if (attackCharged)
            {
                chargeAttackQueued = true;
                chargeAttackTimer = 0f;
            }

            else
            {
                if (!attackAnimation1)
                {
                    attack1Queued = true;
                    attack2Queued = false;
                }
                else if (!attackAnimation2)
                {
                    attack1Queued = false;
                    attack2Queued = true;
                }
            }
            chargeAttackTimer = 0;

        }
        if(meleeButton.wasReleasedThisFrame && canAttack)
        {
            chargeParticles.gameObject.SetActive(false);
            chargedParticles.gameObject.SetActive(false);
            if (attackCharged)
            {
                chargeAttackQueued = true;
                chargeAttackTimer = 0f;
            }
            chargeAttackTimer = 0;
        }
    }
}
