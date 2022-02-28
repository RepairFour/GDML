using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public enum EnemyType
    {
        NONE,
        MELEE_FODDER,
        RANGED_FODDER,
        RANGED_COMBATANT,
        SHIELD_COMBATANT,
        JUMPER_COMBATANT,
        WHEEL_ENEMY,
        WELL_ENEMY
    }
    #region Vars
    #region General Enemy vars
    [Header("General Enemy Variables")]
    public EnemyType type;
    public int dmgPerHit;
    [SerializeField] float attackCDtimerMax;
    [SerializeField] [Range(1, 3)] float firstAttackDelayMod = 1;
    [SerializeField] float attackChargeTimerMax;
    [SerializeField] Animation chargeAttackAni;
    public Animator tenguAnimator;
    //Hiddens
    [HideInInspector] public bool attackMode = false;
    [HideInInspector] public float basicAttackDistance;
    #endregion

    #region Melee Fodder
    [Header("Melee Fodder Variables")]
    [SerializeField] BoxCollider meleeAttackHitbox;
	#endregion

	#region Ranged vars
	[Header("Ranged Enemy Variables")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    [SerializeField] Transform projectileSpawn;
    [Tooltip("Turns ranged enemies into stationary shooting enemy")]
    public bool turretMode;
    public float minDistanceFromPlayer;
	#endregion

	#region Jumper vars
	[Header("Jumper Combatant Variables")]
    [SerializeField] float distanceToTriggerLeap;
    [SerializeField] float leapDuration;
    [SerializeField] float leapHeight;
    [SerializeField] float leapDistance;
    [SerializeField] float leapCooldown;
    [SerializeField] GameObject shockwaveHitbox;
    [SerializeField] float slowShockwaveDuration;
	#endregion

	#region Shield Combatant vars
	[Header("Shield Combatant Variables")]
    [SerializeField] float distanceToTriggerSwipe;
    [SerializeField] int swipeDamage;
    [SerializeField] LayerMask layersAttackMiss;
    [Tooltip("The hitbox distance")]
    [SerializeField] float swipeDistance;
	#endregion

	#region Wheel enemy vars
	[Header("Wheel Enemy Variables")]
    [SerializeField] GameObject fireHitbox;
    [SerializeField] float fireSpawnRate;

    #endregion

    #region Well enemy vars
    [Header("Well Enemy Variables")]
    public float tetherRange;
    public Well well;
	#endregion

	#region Timers
	//Timers
	float attackCDtimer = 0;
    float attackChargeTimer = 0;
    float leapTimer = 0;
    float leapCDTimer = 0;
    float slowShockwaveTimer;
    float fireHitboxTimer = 0;
	#endregion

	#region Fetch Vars
	//Fetch Vars
	PlayerStats player;
    EnemyChase enemyChase;
    EnemyStats enemyStats;
    Material mat;
    Color originalColor;
    NavMeshAgent agent;
    Vector3 playerJumpPos;
    Vector3 shockwaveNormalPos;
	#endregion

	#region States
	//States
	bool firstAttack = true;
    [HideInInspector] 
    public bool chargingAttack = false;
    bool leaping = false;
    bool shockwave = false;
    bool shockwaveSpawned = false;
    bool swiping = false;
	#endregion

	#endregion
	void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        enemyStats = GetComponent<EnemyStats>();
        enemyChase = GetComponent<EnemyChase>();
        if(enemyChase == null)// for flying units
		{
            enemyChase = GetComponentInParent<EnemyChase>();
		}
        basicAttackDistance = enemyChase.basicAttackDistance;
        try
        {
            mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
            originalColor = mat.color;
        }
        catch
		{

		}

        
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
                    //chargingAttack = true;
                    if (enemyChase.gameObject.GetComponent<NavMeshAgent>().velocity.magnitude > 0)
                    {
                        tenguAnimator.SetTrigger("Walkback");
                    }
                    else
                    {
                        tenguAnimator.SetTrigger("Attack");
                    }
                    if (chargeAttackAni != null)
                    {
                        
                        //chargeAttackAni.Play("Idle"); //replace for attack charge ani
                    }
                }
                //if (chargingAttack)
                //{
                //    attackChargeTimer += Time.deltaTime;
                //    mat.color = Color.blue;
                //    if (attackChargeTimer > attackChargeTimerMax)
                //    {
                //        //chargingAttack = false;
                //        //if (chargeAttackAni != null)
                //        //{
                //        //    chargeAttackAni.Play("Attack2");
                //        //    chargeAttackAni.PlayQueued("Run");
                //        //}
                //        //mat.color = originalColor;
                //        //attackChargeTimer = 0;
                //        //attackCDtimer = 0;
                //        ////Vector3 bulletPos = transform.position;
                //        ////bulletPos.y += 5;
                //        //var bullet = Instantiate(projectile, projectileSpawn.position, transform.rotation);
                //        //bullet.GetComponent<EnemyProjectile>().dmg = dmgPerHit;
                //        ////if (InterceptionDir(GameManager.instance.playerAimArea.transform.position, transform.position, player.GetComponent<CharacterController>().velocity, projectileSpeed, out var direction))
                //        ////{
                //        ////    bullet.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
                //        ////}
                        
                //        //bullet.GetComponent<Rigidbody>().velocity = (GameManager.instance.playerAimArea.transform.position - projectileSpawn.position).normalized * projectileSpeed;

                        
                //    }
                //}
            }
        }
        else if(type == EnemyType.MELEE_FODDER)
		{
            if (attackMode && attackCDtimer > attackCDtimerMax)
            {
                if (Vector3.Distance(transform.position, player.transform.position) < basicAttackDistance && !chargingAttack)
                {
                    chargingAttack = true;
                }
                if (chargingAttack)
                {
                    attackChargeTimer += Time.deltaTime;
                    mat.color = Color.blue;
                    if (attackChargeTimer > attackChargeTimerMax)
                    {
                        chargingAttack = false;
                        mat.color = originalColor;
                        attackChargeTimer = 0;
                        attackCDtimer = 0;
                        meleeAttackHitbox.gameObject.SetActive(true);
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

                    var distance = Vector3.Distance(transform.position, player.GetComponent<Collider>().bounds.ClosestPoint(transform.position)) - 5;
                    if(distance > leapDistance)
					{
                        distance = leapDistance;
					}

                    playerJumpPos = transform.position + ((player.transform.position - transform.position).normalized * distance);


     //               if(Mathf.Abs(playerJumpPos - player.transform.position) < 1)
					//{

					//}
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
        else if(type == EnemyType.WHEEL_ENEMY)
		{
            SpawnFireHitbox();
        }
    }

    private void SpawnFireHitbox()
	{
        fireHitboxTimer += Time.deltaTime;
        if(fireHitboxTimer > fireSpawnRate)
		{
            fireHitboxTimer = 0;
            var pos = transform.position;
            pos.y -= transform.GetComponent<BoxCollider>().bounds.size.y / 2;
            Instantiate(fireHitbox,pos, transform.rotation);
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
                    player.Hurt(swipeDamage, ref enemyStats); //need to update to new hurt
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
        Vector3 posToLerpTo = MathHelper.CubicBezier(pos0,pos1,pos2,pos3, leapTimer / leapDuration);

        transform.position = Vector3.Lerp(transform.position, posToLerpTo, 1);

        
    }

    void RangedAttack()
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
        //Vector3 bulletPos = transform.position;
        //bulletPos.y += 5;
        var bullet = Instantiate(projectile, projectileSpawn.position, transform.rotation);
        bullet.GetComponent<EnemyProjectile>().dmg = dmgPerHit;
        //if (InterceptionDir(GameManager.instance.playerAimArea.transform.position, transform.position, player.GetComponent<CharacterController>().velocity, projectileSpeed, out var direction))
        //{
        //    bullet.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        //}

        bullet.GetComponent<Rigidbody>().velocity = (GameManager.instance.playerAimArea.transform.position - projectileSpawn.position).normalized * projectileSpeed;
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
        if (type == EnemyType.WELL_ENEMY)
        {
            if (attackCDtimer > attackCDtimerMax)
            {
                var player = other.GetComponent<PlayerStats>();
                if (player != null)
                {
                    player.Hurt(dmgPerHit, ref enemyStats);
                    attackCDtimer = 0;
                }
            }
        }

    }
	private void OnTriggerStay(Collider other)
	{
        if (type == EnemyType.MELEE_FODDER || type == EnemyType.WELL_ENEMY)
        {
            if (attackCDtimer > attackCDtimerMax)
            {
                var player = other.GetComponent<PlayerStats>();
                if (player != null)
                {
                    player.Hurt(dmgPerHit, ref enemyStats);
                    attackCDtimer = 0;
                }
            }
        }
    }


    public bool InterceptionDir(Vector3 a, Vector3 b, Vector3 vA, float sB, out Vector3 result)
	{
        var aToB = b - a;
        var dC = aToB.magnitude;
        var alpha = Vector2.Angle(aToB, vA) * Mathf.Deg2Rad;
        var sA = vA.magnitude;
        var r = sA / sB;
        if(MathHelper.SolveQuadratic(1-r*r, 2*r*dC*Mathf.Cos(alpha), -(dC*dC), out var root1, out var root2) == 0)
		{
            result = Vector2.zero;
            return false;
		}
        var dA = Mathf.Max(root1, root2);
        var t = dA / sB;
        var c = a + vA * t;
        result = (c - b).normalized;
        return true;
	}
}


