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

    public MeshRenderer rangedWeapon;
    public GunBase rangedWeaponScript;
    public MeshRenderer meleeWeapon;

    public BoxCollider attackCollider;

    private bool attackAnimation1;
    private bool attackAnimation2;
    bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        input = new PlayerMap();
        input.Enable();

        meleeButton = (ButtonControl)input.Player.Melee.controls[0];

        attackCollider.enabled = false;

        meleeWeapon.enabled = false;
        rangedWeapon.enabled = true;
        rangedWeaponScript.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (meleeButton.isPressed)
        {
            meleeWeapon.enabled = true;
            rangedWeapon.enabled = false;
            rangedWeaponScript.enabled = false;
        }

        if (meleeButton.wasReleasedThisFrame && canAttack)
        {
            //canAttack = false;
            if (!attackAnimation1)
            {
                attackCollider.enabled = true;

                animator.SetTrigger("Attack1");
                attackAnimation1 = true;
                attackAnimation2 = false;
                StopAllCoroutines();
                StartCoroutine(CleanUpAttack(0.5f));
                StartCoroutine(CleanUpHitBox(0.02f));
            }
            else if (!attackAnimation2)
            {
                attackCollider.enabled = true;

                animator.SetTrigger("Attack2");
                attackAnimation1 = false;
                attackAnimation2 = true;
                StopAllCoroutines();
                StartCoroutine(CleanUpAttack(0.5f));
                StartCoroutine(CleanUpHitBox(0.02f));

            }
            //StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator CleanUpAttack(float time)
    {
        yield return new WaitForSeconds(time);
        attackAnimation1 = false;
        attackAnimation2 = false;
        animator.SetTrigger("reset");

        rangedWeapon.enabled = true;
        rangedWeaponScript.enabled = true;
        meleeWeapon.enabled = false;
    }
    IEnumerator CleanUpHitBox(float time)
    {
        yield return new WaitForSeconds(time);
        attackCollider.enabled = false;
    }
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.002f);
        canAttack = true;

    }
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Enemy")
		{
            if (attackAnimation1)
            {
                other.GetComponent<EnemyStats>().Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION1);
            }
            else if(attackAnimation2)
			{
                other.GetComponent<EnemyStats>().Hurt(damage, EnemyStats.MeleeAnimation.ANIMATION2);
            }
        }
	}
}
