using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool melee;
    public int dmgPerHit;
    [HideInInspector]
    public bool attackMode = false;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    PlayerStats player;

    float attackCDtimer = 0;
    [SerializeField] float attackCDtimerMax;
    [HideInInspector]
    public float attackDistance;
    bool firstAttack = true;
    [SerializeField][Range(1,3)] float firstAttackDelayMod = 1;

    [SerializeField] float attackChargeTimerMax;
    float attackChargeTimer = 0;
    bool chargingAttack = false;
    [SerializeField] Animation chargeAttackAni;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        attackDistance = GetComponent<EnemyChase>().attackDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerStats>();
        }
        if (attackMode)
        {
            attackCDtimer += Time.deltaTime;
            if(firstAttack)
			{
                firstAttack = false;
                attackCDtimer -= Random.value * firstAttackDelayMod;
			}
        }
        if(!melee && attackMode && attackCDtimer > attackCDtimerMax)
		{
            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance + 1)
            {
                chargingAttack = true;
                chargeAttackAni.Play("Idle");
            }
            if(chargingAttack)
			{
                attackChargeTimer += Time.deltaTime;
                if(attackChargeTimer > attackChargeTimerMax)
				{
                    chargingAttack = false;
                    chargeAttackAni.Play("Attack2");
                    chargeAttackAni.PlayQueued("Run");
                    attackChargeTimer = 0;
                    attackCDtimer = 0;
                    var bullet = Instantiate(projectile, transform.position, transform.rotation);
                    bullet.GetComponent<EnemyProjectile>().dmg = dmgPerHit;
                    bullet.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position).normalized * projectileSpeed);                    
                }
			}
        }
    }

	private void OnTriggerEnter(Collider other)
    {
        if (melee && attackCDtimer > attackCDtimerMax)
        {
            var player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.Hurt(dmgPerHit);
                attackCDtimer = 0;
            }
        }
    }
}
