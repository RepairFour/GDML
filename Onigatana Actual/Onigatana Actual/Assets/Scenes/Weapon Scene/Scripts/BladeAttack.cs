using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;


public class BladeAttack : MonoBehaviour
{
    [SerializeField] int damage;
    PlayerMap input;
    private ButtonControl meleeButton;

    public Animator animator;
    public ParticleSystem weaponTrail;

    public List<MeshRenderer> rangedWeapon = new List<MeshRenderer>();
    public GunBase rangedWeaponScript;
    public MeshRenderer meleeWeapon;
    public SkinnedMeshRenderer meleeHand;

    public BoxCollider attackCollider;
    public Transform feelerPoint;
    public Vector3 feelerHalfExtents;
    public float feelerRange;
    public LayerMask feelerMask;
    private Vector3 strikeHit;
    private bool strikeLocked;
    public float blinkSpeed;

    private bool attackAnimation1;
    private bool attack1Queued;
    private bool attack2Queued;
    private bool attackAnimation2;
    private float cleanUpTimer;
    public float cleanUpTime;
    public float attackSpeed;
    private float attackTimer;
    bool canAttack = true;

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
        
        QueueAttack();
        CheckStrikeLocked();
        FeelerRay();
        HandleStrikeLocked();
        RunAnimation1();
        RunAnimation2();
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
        if(attackAnimation1 || attackAnimation2)
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

    IEnumerator CleanUpHitBox(float time)
    {
        yield return new WaitForSeconds(time);
        attackCollider.enabled = false;
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
        if ((attack1Queued || attack2Queued) && !strikeLocked && canAttack)
        {
            RaycastHit hit;
            if (Physics.BoxCast(feelerPoint.position, feelerHalfExtents, feelerPoint.forward, out hit, Quaternion.identity, feelerRange, feelerMask))
            {
                strikeHit = hit.point;
                strikeLocked = true;
            }
        }
        
    }
    private void HandleStrikeLocked()
    {
        if (strikeLocked)
        {
            controller.BlinkToPosition(strikeHit, blinkSpeed);
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
        }
    }
    private void QueueAttack()
    {
        if (meleeButton.wasReleasedThisFrame && canAttack)
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
    }
}
