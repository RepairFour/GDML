using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class SwordAttack : MonoBehaviour
{
    
    [SerializeField] int normalDamage;
    [SerializeField] int chargeAttackDamage;
    int damage;


    PlayerMap input;
    private ButtonControl meleeButton;

    [Header("Input")]
    public float holdButtonDelay;
    public float attackSpeed;

    [Header("Animation and Particles")]
    public Animator animator;
    public ParticleSystem weaponTrail;
    public ParticleSystem attackHitEffect;
    public ParticleSystem blood;
    public GameObject chargeParticles;
    public List<string> attacks = new List<string>();
    public MeshRenderer swordRenderer;

    [Header("Player")]
    public MainCharacterController controller;

    [Header("AttackColliders")]
    public BoxCollider attackCollider;
    
    [Header("ChargeAttackStuff")]
    public Transform feelerPoint;
    public Vector3 feelerLowTierHalfExtents;
    public Vector3 feelerMidTierExtents;
    public Vector3 feelerOuterTierExtents;
    public float feelerRange;
    public LayerMask feelerMask;
    public float dashSpeed;
    public float chargeAttackStoppingDistance;
    public float timeToCharge;
    public float slowdownOnCharge;
    public float attackResetDelay;

    public float maxElectricityAmount;
    float currentElectricityAmount; 

    private bool attackQueued;
    private bool chargingAttack;
    private bool canAttack = true;
    private bool attackCharged;
    private float coolDownTimer;
    private float inputTimer;
    //private bool isButtonHeld;
    private float buttonHeldTime;
    private float chargingTimer;
    private int animationCounter = 0;

    GameObject blinkTarget;

    private bool strikeLocked;
    bool blocked = false;




    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.instance.input;

        meleeButton = (ButtonControl)input.Player.Melee.controls[0];

        attackCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        QueryInput();
        HandleChargingAttack();
        HandleDashToTarget();
        CheckIfDashing();
        HandleAttacks();
        
        HandleAttackCleanUp();
    }

    void HandleAttackCleanUp()
    {
        if (!canAttack)
        {
            coolDownTimer += Time.deltaTime;
            if(coolDownTimer >= attackSpeed)
            {
                coolDownTimer = 0;
                canAttack = true;
            }
        }
    }

    void QueryInput()
    {
        if (meleeButton.wasReleasedThisFrame && canAttack)
        {
            if (!attackCharged)
            {
                animator.SetBool("ChargeStart", false);
                animator.SetBool("Charging", false);
            }
            chargeParticles.SetActive(false);
            swordRenderer.materials[1].SetFloat("electric_amount", 0);

            attackQueued = true;
            buttonHeldTime = 0;
            chargingTimer = 0;
        }
        if (meleeButton.isPressed)
        {
            buttonHeldTime += Time.deltaTime;
            if(buttonHeldTime >= holdButtonDelay)
            {
                chargingAttack = true;
                chargingTimer += Time.deltaTime;
                
                animator.SetBool("ChargeStart", true);
            }
        }
        if (!meleeButton.isPressed)
        {
            chargingAttack = false;
            chargingTimer = 0;
            
        }
    }

    void FeelerRay()
    {
        if (attackCharged && canAttack)
        {
            DrawBoxCast.DrawBoxCastBox(feelerPoint.position, feelerLowTierHalfExtents, transform.rotation, transform.forward, feelerRange, Color.red);
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
                    strikeLocked = true;
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
                    strikeLocked = true;
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
                    strikeLocked = true;
                }
            }
            else
            {
                if (blinkTarget != null)
                {
                    blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
                    blinkTarget = null;
                    strikeLocked = false;
                }
            }
        }
    }

    void HandleChargingAttack()
    {
        if (chargingAttack)
        {
            FeelerRay();
            animator.SetBool("Charging", true);

            if (chargingTimer >= timeToCharge)
            {
                chargingAttack = false;
                attackCharged = true;

                swordRenderer.materials[1].SetFloat("electric_amount", 5);
                chargeParticles.SetActive(true);

                
            }
        }
    }
    void HandleDashToTarget()
    {
        if(attackCharged && attackQueued && strikeLocked)
        {
            controller.BlinkToPosition(blinkTarget.transform.position, dashSpeed, chargeAttackStoppingDistance);
            strikeLocked = false;
        }
    }
    void CheckIfDashing()
    {
        if (!controller.isBlinkStrikeActivated && !strikeLocked )
        {
            if (blinkTarget != null)
            {
                blinkTarget.GetComponent<Outline>().OutlineWidth = 0f;
            }
            blinkTarget = null;
        }
    }

    void HandleAttacks()
    {
        if (attackCharged && attackQueued && !controller.isBlinkStrikeActivated)
        {
            animator.SetTrigger("ChargeAttack");
            animator.SetBool("Charging", false);
            animator.SetBool("ChargeStart", false);
            Debug.Log("Charge Attacked");
            weaponTrail.Play();
            attackCollider.enabled = true;
            AudioHandler.instance.PlaySound("SwordSlash2", 1, true, 2);

            blocked = false;
            attackCharged = false;
            attackQueued = false;
            canAttack = false;
            chargingTimer = 0;
            //animator.SetBool("ChargeAttack", false);
            damage = chargeAttackDamage;
        }
        else if (attackQueued && !attackCharged && !chargingAttack)
        {
            animator.SetBool("Attacking", true);
            animator.SetTrigger(attacks[animationCounter]);
            inputTimer = 0;
            animationCounter++;
            if(animationCounter > attacks.Count - 1)
            {
                animationCounter = 0;
            }
            weaponTrail.Play();
            attackCollider.enabled = true;
            AudioHandler.instance.PlaySound("SwordSlash2", 1, true, 2);
            attackCharged = false;
            attackQueued = false;
            canAttack = false;
            blocked = false;
            damage = normalDamage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            var enemyStats = other.gameObject.GetComponent<EnemyStats>();
            if (enemyStats.hasShield == false)
            {
                enemyStats.Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION1);
                Instantiate(blood, other.ClosestPoint(transform.position), Quaternion.identity);
                Instantiate(attackHitEffect, other.ClosestPoint(transform.position), Quaternion.identity);
            }
			else
			{
                //Is player infront of enemy
                if(Vector3.Dot((GameManager.instance.playerStats.transform.position - other.gameObject.transform.position).normalized,
                               other.gameObject.transform.forward) > 0)
				{
                    var shield = other.gameObject.GetComponentInChildren<ShieldHealth>();
                    if(shield.Hurt(damage))//if the shield dies then
					{
                        enemyStats.hasShield = false;
                    }
                }
				else
				{
                    enemyStats.Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION1);
                    Instantiate(blood, other.ClosestPoint(transform.position), Quaternion.identity);
                    Instantiate(attackHitEffect, other.ClosestPoint(transform.position), Quaternion.identity);
                }
            }                          
        }
        else if(other.gameObject.tag == "Shield")
		{
            var shield = other.gameObject.GetComponentInParent<ShieldHealth>();
            if (shield.Hurt(damage))//if the shield dies then
            {
                shield.GetComponentInParent<EnemyStats>().hasShield = false;
            }
        }
        attackCollider.enabled = false;
    }
}
