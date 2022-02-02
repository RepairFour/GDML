using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Weapon : MonoBehaviour
{
    public enum ParryState { START, PERFECT, NORMAL, RECOVERY }
    [Header("Weapon Stats")]
    [SerializeField] protected int normalDamage;
    [SerializeField] protected int chargeAttackDamage;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackSpeed;
    
    [Header("Weapon Hitbox Stats")]
    [SerializeField] protected LayerMask attackMask;
    [SerializeField] protected Vector3 attackFeelerLowTierHalfExtents;
    [SerializeField] protected Vector3 attackFeelerMidTierExtents;
    [SerializeField] protected Vector3 attackFeelerOuterTierExtents;

    [Header("Charge Attack Stats")]
    [SerializeField] protected float timeToCharge;
    [SerializeField] protected float slowdownOnCharge;
    [SerializeField] protected float attackResetDelay;

    [Header("Charge Attack Hitbox Stats")]
    [SerializeField] protected Transform feelerPoint;
    [SerializeField] protected Vector3 feelerLowTierHalfExtents;
    [SerializeField] protected Vector3 feelerMidTierExtents;
    [SerializeField] protected Vector3 feelerOuterTierExtents;
    [SerializeField] protected float chargeAttackRange;
    [SerializeField] protected LayerMask chargeAttackMask;

    [Header("Parry Stats")]
    [SerializeField] protected ParryState parryState;
    [SerializeField] protected float parryWindUpTime;
    [SerializeField] protected float perfectParryTime;
    [SerializeField] protected float normalParryTime;
    [SerializeField] protected float recoveryTime;

    protected float parryTimer;

    bool canParry;
    bool parrying;
    bool perfectParrySuccess = false;
    bool normalParrySuccess = false;

    public bool getParrying { get => parrying; set => parrying = value; }
    public ParryState getParryState { get => parryState; set => parryState = value; }




    protected bool attackQueued;
    protected bool chargingAttack;
    protected bool canAttack;
    protected bool attackCharged;
    protected bool blocked;
    
    protected float cooldownTimer;
    protected float buttonHeldTime;
    protected float chargingTimer;

    protected int damage;

    PlayerMap input;
    private ButtonControl meleeButton;
    private ButtonControl parryButton;

    [Header("Input")]
    public float holdButtonDelay;

    [Header("Player")]
    public MainCharacterController controller;
    
    
    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.instance.input;
        meleeButton = (ButtonControl)input.Player.Melee.controls[0];
        parryButton = (ButtonControl)input.Player.Parry.controls[0];
    }

    // Update is called once per frame
    void Update()
    {
        QueryInput();
        HandleParrying();
        HandleChargingAttack();
        HandleChargeAttackEffect();
        HandleAttacks();
        HandleAttackCleanUp();
        
    }

    #region Parrying
    void HandleParrying()
    {
        if (parrying)
        {
            switch (parryState)
            {
                case ParryState.START:
                    parryTimer += Time.deltaTime;
                    Debug.Log("Parry Starting");
                    if (parryTimer >= parryWindUpTime)
                    {
                        parryState = ParryState.PERFECT;
                        parryTimer = 0;
                    }
                    break;
                case ParryState.PERFECT:
                    Debug.Log("Perfect Parry");
                    parryTimer += Time.deltaTime;
                    if (parryTimer >= perfectParryTime)
                    {
                        parryState = ParryState.NORMAL;
                        parryTimer = 0;
                    }
                    break;
                case ParryState.NORMAL:

                    Debug.Log("Normal Parry");
                    parryTimer += Time.deltaTime;
                    canParry = false;

                    if (parryTimer >= normalParryTime)
                    {
                        parryState = ParryState.RECOVERY;
                        parryTimer = 0;
                    }
                    break;
                case ParryState.RECOVERY:
                    Debug.Log("Recovery");

                    parryTimer += Time.deltaTime;

                    if (parryTimer >= recoveryTime)
                    {
                        ResetParry();
                    }
                    break;
            }
        }
    }

    public void ResetParry()
    {
        parryState = ParryState.START;
        parrying = false;
        parryTimer = 0;
        canParry = true;
        canAttack = true;
    }
    #endregion

    #region Input
    protected virtual void QueryInput()
    {
        QueryMeleeInput();

        QueryParryInput();
    }

    protected virtual void QueryMeleeInput()
    {
        if (meleeButton.wasReleasedThisFrame && canAttack)
        {
            if (!attackCharged)
            {
                ResetChargeAnimation();
            }
            TurnOffChargeAttackParticles();

            attackQueued = true;
            buttonHeldTime = 0;
            chargingTimer = 0;
        }
        if (meleeButton.isPressed)
        {
            buttonHeldTime += Time.deltaTime;
            if (buttonHeldTime >= holdButtonDelay)
            {
                chargingAttack = true;
                chargingTimer += Time.deltaTime;

                ChargeStartAnimation();
            }
        }
        if (!meleeButton.isPressed)
        {
            chargingAttack = false;
            chargingTimer = 0;

        }

    }
    void QueryParryInput()
    {
        if (parryButton.wasPressedThisFrame && canParry)
        {
            parrying = true;
            parryState = ParryState.START;
            canParry = false;
            canAttack = false;
        }
        if (!parryButton.isPressed && parryState != ParryState.START && parryState != ParryState.PERFECT && parryState != ParryState.RECOVERY)
        {
            if (parryState == ParryState.NORMAL)
            {
                canParry = false;
                parryState = ParryState.RECOVERY;
                return;
            }
            parrying = false;
            parryTimer = 0;
        }
    }
    #endregion

    #region ChargeAttack
    protected virtual void ChargeAttackFeelerRay() { }
    protected virtual void HandleChargingAttack() 
    {
        if (chargingAttack)
        {
            ChargeAttackFeelerRay();
            ChargingAttackAnimation();

            if(chargingTimer >= timeToCharge)
            {
                chargingAttack = false;
                attackCharged = true;

                TurnOnChargeAttackParticles();
            }
        }
    }
    protected virtual void HandleChargeAttackEffect() { }
    #endregion
    protected virtual void HandleAttacks() 
    {
        if (attackCharged && attackQueued && !controller.isBlinkStrikeActivated)
        {
            ChargeAttackAnimation();
            ChargeAttackSound();
            blocked = false;
            attackCharged = false;
            attackQueued = false;
            canAttack = false;
            
            chargingTimer = 0;
            //animator.SetBool("ChargeAttack", false);
            damage = chargeAttackDamage;
            HandleTarget();
        }
        else if (attackQueued && !attackCharged && !chargingAttack)
        {
            BasicAttackAnimation();
            //attackCollider.enabled = true;
            BasicAttackSound();
            
            
            attackCharged = false;
            attackQueued = false;
            canAttack = false;
            blocked = false;
            
            damage = normalDamage;
            HandleTarget();
        }

    }
    protected virtual void HandleTarget() { }
    protected virtual void BasicAttackAnimation() { }
    protected virtual void ResetChargeAnimation(){ }
    protected virtual void ChargeAttackAnimation() { }
    protected virtual void ChargingAttackAnimation() { }
    protected virtual void ChargeStartAnimation() { }
    protected virtual void BasicAttackSound() { 
    }
    protected virtual void ChargeAttackSound() { }
    protected virtual void TurnOnChargeAttackParticles() { }
    protected virtual void TurnOffChargeAttackParticles() { }
    protected void HandleAttackCleanUp()
    {
        if (!canAttack)
        {
            cooldownTimer += Time.deltaTime;
            if(cooldownTimer >= attackSpeed)
            {
                cooldownTimer = 0;
                canAttack = true;
            }
        }
    }

}
