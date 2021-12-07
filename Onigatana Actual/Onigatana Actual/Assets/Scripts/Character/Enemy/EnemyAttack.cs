using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public enum EnemyType
    {
        MELEE_FODDER,
        RANGED_FODDER,
        RANGED_COMBATANT,
        SHIELD_COMBATANT,
        JUMPER_COMBATANT
    }

    [Header("General Enemy Variables")]
    public EnemyType type;
    public int dmgPerHit;
    [SerializeField] float attackCDtimerMax;
    [SerializeField] [Range(1, 3)] float firstAttackDelayMod = 1;
    [SerializeField] float attackChargeTimerMax;
    [SerializeField] Animation chargeAttackAni;
    //Hiddens
    [HideInInspector] public bool attackMode = false;
    [HideInInspector] public float basicAttackDistance;


    [Header("Ranged Enemy Variables")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    [Tooltip("Turns ranged enemies into stationary shooting enemy")]
    public bool turretMode;

    [Header("Jumper Combatant Variables")]
    [SerializeField] float distanceToTriggerLeap;
    [SerializeField] float leapDuration;
    [SerializeField] float leapHeight;
    [Tooltip("Percentage of the distance between this and the player")]
    [SerializeField] [Range(0, 1)] float leapDistance;
    [SerializeField] float leapCooldown;
    [SerializeField] GameObject shockwaveHitbox;
    [SerializeField] float slowShockwaveDuration;
    [Header("Shield Combatant Variables")]
    [SerializeField] float distanceToTriggerSwipe;
    [SerializeField] int swipeDamage;
    [SerializeField] LayerMask layersAttackMiss;
    [Tooltip("The hitbox distance")]
    [SerializeField] float swipeDistance;

    //Timers
    float attackCDtimer = 0;
    float attackChargeTimer = 0;
    float leapTimer = 0;
    float leapCDTimer = 0;
    float slowShockwaveTimer;

    //Fetch Vars
    PlayerStats player;
    EnemyChase enemyChase;
    Material mat;
    Color originalColor;
    NavMeshAgent agent;
    Vector3 playerJumpPos;
    Vector3 shockwaveNormalPos;

    //States
    bool firstAttack = true;
    bool chargingAttack = false;
    bool leaping = false;
    bool shockwave = false;
    bool shockwaveSpawned = false;
    bool swiping = false;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        enemyChase = GetComponent<EnemyChase>();
        basicAttackDistance = enemyChase.basicAttackDistance;
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        originalColor = mat.color;
        agent = GetComponent<NavMeshAgent>();
        if (type == EnemyType.JUMPER_COMBATANT)
        {
            shockwaveNormalPos = shockwaveHitbox.transform.localPosition;
        }
    }

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
        
        if(type == EnemyType.RANGED_FODDER)
		{
            if(attackMode && attackCDtimer > attackCDtimerMax)
            {
                if (Vector3.Distance(transform.position, player.transform.position) <= basicAttackDistance + 1 || enemyChase.runawayTimer > 0)
                {
                    chargingAttack = true;
                    if (chargeAttackAni != null)
                    {
                        chargeAttackAni.Play("Idle"); //replace for attack charge ani
                    }
                }
                if (chargingAttack)
                {
                    attackChargeTimer += Time.deltaTime;
                    mat.color = Color.blue;
                    if (attackChargeTimer > attackChargeTimerMax)
                    {
                        chargingAttack = false;
                        if (chargeAttackAni != null)
                        {
                            chargeAttackAni.Play("Attack2");
                            chargeAttackAni.PlayQueued("Run");
                        }
                        mat.color = originalColor;
                        attackChargeTimer = 0;
                        attackCDtimer = 0;
                        Vector3 bulletPos = transform.position;
                        bulletPos.y += 5;
                        var bullet = Instantiate(projectile, bulletPos, transform.rotation);
                        bullet.GetComponent<EnemyProjectile>().dmg = dmgPerHit;
                        bullet.GetComponent<Rigidbody>().AddForce((player.transform.position - bulletPos).normalized * projectileSpeed);
                    }
                }
            }
        }

        else if(type == EnemyType.JUMPER_COMBATANT)
		{
            leapCDTimer += Time.deltaTime;
            if (attackMode && attackCDtimer > attackCDtimerMax)
            {
                //need some decision logic here
                //jump attack
                if(Vector3.Distance(transform.position, player.transform.position) > distanceToTriggerLeap
                   && !leaping
                   && leapCDTimer >= leapCooldown)
				{
                    //arc towards player
                    agent.enabled = false;
                    leaping = true;
                    playerJumpPos = transform.position + ((player.transform.position - transform.position) * leapDistance);
                    playerJumpPos.y = transform.position.y; //bug city
                    leapCDTimer = 0;
                }

            }

            if(leaping)
			{
                LeapSlam();
			}
            if(shockwave)
			{
                Shockwave();
			}
        }
        else if(type == EnemyType.SHIELD_COMBATANT)
		{
            if (attackMode && attackCDtimer > attackCDtimerMax)
			{
                if (Vector3.Distance(transform.position, player.transform.position) > distanceToTriggerSwipe)
				{
                    swiping = true;
				}

            }
            if (swiping)
            {
                SwipeAttack();
            }

        }
    }

    void SwipeAttack()
	{
        //play animation
        //box cast
        attackChargeTimer += Time.deltaTime;
        mat.color = Color.red;
        if (attackChargeTimer > attackChargeTimerMax)
        {
            RaycastHit hit;
            if (Physics.BoxCast(transform.position, Vector3.one, player.transform.position - transform.position, out hit, transform.rotation, swipeDistance,~layersAttackMiss))
            {
                player = hit.collider.GetComponent<PlayerStats>();
                if (player != null)
                {
                    player.Hurt(swipeDamage); //need to update to new hurt
                }                
            }
            attackChargeTimer = 0;
            attackCDtimer = 0;
            mat.color = originalColor;
            swiping = false;
            //if (hit.collider != null)
            //{
            //    Debug.Log(hit.collider.gameObject.name + " on the " + hit.collider.gameObject.layer + " layer.");
            //}
        }

        //if hit deal dmg
	}
    void LeapSlam()
	{
        leapTimer += Time.deltaTime;
        if(leapTimer > leapDuration) //has landed
		{
            leaping = false;
            leapTimer = 0;
            agent.enabled = true;
            shockwave = true;
            return;
        }
        //calculate curve positions
        Vector3 pos0 = transform.position;
        Vector3 pos1 = transform.position;
        pos1.y += leapHeight;
        Vector3 pos2 = playerJumpPos;
        pos2.y += leapHeight;
        Vector3 pos3 = playerJumpPos;
        Vector3 posToLerpTo = CubicBezier(pos0,pos1,pos2,pos3, leapTimer / leapDuration);

        transform.position = Vector3.Lerp(transform.position, posToLerpTo, 1);

        
    }

    void Shockwave()
	{
        if (!shockwaveSpawned)
        {
            shockwaveSpawned = true;
            shockwaveHitbox.SetActive(true);
            Vector3 dir = player.transform.position;
            dir.y = shockwaveHitbox.transform.position.y;
            shockwaveHitbox.transform.LookAt(dir);
            shockwaveHitbox.transform.localScale = Vector3.zero;
            shockwaveHitbox.transform.SetParent(GameManager.instance.transform); 
        }

        slowShockwaveTimer += Time.deltaTime;
        shockwaveHitbox.transform.localScale += (Vector3.one / slowShockwaveDuration) * Time.deltaTime;
        if (slowShockwaveTimer > slowShockwaveDuration)
		{
            slowShockwaveTimer = 0;
            shockwaveHitbox.SetActive(false);
            shockwave = false;
            shockwaveSpawned = false;
            shockwaveHitbox.transform.SetParent(transform);
            shockwaveHitbox.transform.localPosition = shockwaveNormalPos;
        }
    }

    public void DisposeShockwave()
	{
        if (shockwaveHitbox != null)
        {
            Destroy(shockwaveHitbox);
        }
    }

	private void OnTriggerEnter(Collider other)
    {
        if (type == EnemyType.MELEE_FODDER )
        {
            if (attackCDtimer > attackCDtimerMax)
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

    public static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float r = 1f - t;
        float f0 = r * r * r;
        float f1 = r * r * t * 3;
        float f2 = r * t * t * 3;
        float f3 = t * t * t;
        return new Vector3( f0 * p0.x + f1 * p1.x + f2 * p2.x + f3 * p3.x,
                            f0 * p0.y + f1 * p1.y + f2 * p2.y + f3 * p3.y,
                            f0 * p0.z + f1 * p1.z + f2 * p2.z + f3 * p3.z);
    }
}
