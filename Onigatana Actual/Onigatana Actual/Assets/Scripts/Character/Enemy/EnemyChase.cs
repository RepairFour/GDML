using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
	#region Variables
	PlayerStats player;
    NavMeshAgent agent;
    WaypointManager wayManager;
    [HideInInspector]
    public int patrolPath;
    List<Transform> path;

    int pathNode = 0;
    bool chasePlayer = false;

    bool gettingKnockbacked = false;
    bool hitwall = false;
    bool knockbackSetUp = false;
    Vector3 knockbackDirection;
    Vector3 knockbackPos0;
    Vector3 knockbackPos1;
    Vector3 knockbackPos2;
    Vector3 knockbackPos3;
    float knockbackTimer = 0f;
    float knockbackDuration;
    float knockbackDistance;
    float knockbackHeight;
    int wallsmashDamage;
    public Vector3 setKnockbackDirection { set => knockbackDirection = value; }
    public bool isGettingKnockbacked { get => gettingKnockbacked; set => gettingKnockbacked = value; }
    public float setKnockbackDuration { set => knockbackDuration = value; }
    public float setKnockbackDistance { set => knockbackDistance = value; }
    public float setKnockbackHeight { set => knockbackHeight = value; }
    public int setWallSmashDamage { set => wallsmashDamage = value; }

    public LayerMask knockbackMask;

    EnemyAttack enemyAttack;
    public float basicAttackDistance;

    public Animator tenguAnimator;

    // melee units
    Collider playerCollider;


    [SerializeField] float visionDistance = 100;
    [SerializeField] LayerMask layersToIgnore;
    [SerializeField] float aknowledgementTimerMax;
    float aknowledgementTimer = 0;
    Vector3 playerPos;

    NavMeshAgent NavMeshAgent;
    [Tooltip("The distance this can run before needing a break")]
    [SerializeField]float runawayDistance;
    [Tooltip("How long of a break this needs after expending its runaway distance")]
    [SerializeField] float runawayCooldown;
    [HideInInspector]
    public float runawayTimer = 0;
    float runawayDistanceMax;

    [SerializeField] float loseInterestTimerMax;
    float loseInterestTimer = 0;
    bool wasChasing = false;

    Vector3 lastLocation = Vector3.zero;
    bool relocate = false;
    Vector3 relocatePos;
    [SerializeField]float relocateTimerMax;
    float relocateTimer = 0;
    float standingStillTimerMax = 0.8f;
    float standingStillTimer = 0;

    [SerializeField] Transform debugDestination;
    float debugTimer = 0;
    float debugTimerMax = 1;

    [SerializeField] Transform enemyTransform; //because the this script my be attached to a parent not the enemy itself
    [SerializeField] bool isFlying;
    [SerializeField] Tether tether;

    public GameObject test;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollider = player.GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        enemyAttack = GetComponent<EnemyAttack>();
        if(enemyAttack == null)//for flying units
		{
            enemyAttack = GetComponentInChildren<EnemyAttack>();
		}
        wayManager = FindObjectOfType<WaypointManager>();

        if (enemyAttack.turretMode == false)
        {
            path = new List<Transform>(wayManager.paths[patrolPath].waypoints);
        }

        NavMeshAgent = GetComponent<NavMeshAgent>();
        runawayDistanceMax = runawayDistance;

        if(enemyAttack.turretMode)

		{
            visionDistance *= 500;
            basicAttackDistance *= 500;

            
		}
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(agent.velocity.magnitude > 0)
        {
            if(Vector3.Dot(agent.velocity, transform.forward) > 0)
            {
                tenguAnimator.SetBool("Walking", true);
            }
            else
            {
                tenguAnimator.SetBool("Walking", false);
            }
            

        }
        else
        {
            tenguAnimator.SetBool("Walking", false);
            
        }
        
        if (!gettingKnockbacked)
        {
            UpdateRotation();
            if (player == null)
            {
                player = FindObjectOfType<PlayerStats>();
            }
            if (isFlying)
            {
                if (GetComponentInChildren<EnemyStats>() == null) // when the enemy dies,needs this to remove this(the parent)
                {
                    Destroy(gameObject);
                    return;
                }
                var temp = enemyTransform.localPosition;
                temp.x = 0;
                temp.z = 0;
                enemyTransform.localPosition = temp;
            }
            if (debugDestination == null)
            {
                if (enemyAttack.turretMode == false)
                {
                    if (enemyAttack.type == EnemyAttack.EnemyType.WHEEL_ENEMY)
                    {
                        FollowPatrol();
                        return;
                    }
                    CanISeeThePlayer(); // fills out chasePlayer bool 
                    if (chasePlayer == false)
                    {
                        if (wasChasing)
                        {
                            loseInterestTimer += Time.deltaTime;
                            if (loseInterestTimer >= loseInterestTimerMax)
                            {
                                wasChasing = false;
                                loseInterestTimer = 0;
                            }
                            ChasePlayer();
                            //Debug.Log("Chasing player");
                        }
                        else
                        {
                            FollowPatrol();
                            //Debug.Log("Following patrol");
                        }
                    }
                    else // get to optimal attack range
                    {
                        wasChasing = true;
                        enemyAttack.attackMode = true;
                        ChasePlayer();
                        //Debug.Log("Chasing player 2");
                    }
                }
                else
                {
                    CanISeeThePlayer();
                    if (chasePlayer)
                    {
                        enemyAttack.attackMode = true;
                    }
                }
            }
            else
            {
                debugTimer += Time.deltaTime;
                if (debugTimer >= debugTimerMax)
                {
                    debugTimer = 0;
                    FindPath(debugDestination.position);
                }
            }
        }
        else
        {
            agent.enabled = false;
            Knockback();
        }
        
    }

    void UpdateRotation()
	{
        if (enemyAttack.type != EnemyAttack.EnemyType.JUMPER_COMBATANT)
        {
            Vector3 dir;
            if (Vector3.Distance(transform.position, player.transform.position) <= basicAttackDistance)
            {
                dir = player.transform.position - transform.position;
            }
            else
            {
                dir = agent.destination - transform.position;
                if (enemyAttack.type == EnemyAttack.EnemyType.RANGED_FODDER || enemyAttack.type == EnemyAttack.EnemyType.WELL_ENEMY)
                {
                    dir = player.transform.position - transform.position;
                }
            }
            dir.y = 0;//This allows the object to only rotate on its y axis
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5 * Time.deltaTime);
        }
    }
    void FindPath(Vector3 desiredDestination)
	{
        if (Vector3.Distance(transform.position, lastLocation) < 0.7f &&
            Vector3.Distance(transform.position, desiredDestination) > 1f &&
            standingStillTimer > standingStillTimerMax &&
            relocate == false)
        {
            if (enemyAttack.type != EnemyAttack.EnemyType.SHIELD_COMBATANT && !enemyAttack.turretMode && enemyAttack.type != EnemyAttack.EnemyType.WELL_ENEMY)
            {
                agent.SetDestination((Quaternion.Euler(0, Random.Range(100, 260), 0) * desiredDestination).normalized * basicAttackDistance);
                relocatePos = agent.destination;
                relocate = true;
                Debug.Log("Relocate to " + agent.destination);
            }
        }

        if (!relocate)
		{
            if (agent.isOnNavMesh)
            {
                agent.destination = desiredDestination;
            }
		}
		else
		{
            relocateTimer += Time.deltaTime;
            if (relocateTimer >= relocateTimerMax)
            {
                relocateTimer = 0;
                relocate = false;                
            }            
        }
        
        standingStillTimer += Time.deltaTime;
        if(standingStillTimer >= standingStillTimerMax)
		{
            lastLocation = transform.position;
            standingStillTimer = 0;
        }

    }

    private void CanISeeThePlayer()
	{
        chasePlayer = false;
        // try to see player 
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - enemyTransform.position;
        
        if (Physics.Raycast(enemyTransform.position, directionToPlayer, out hit,visionDistance,~layersToIgnore))
        {
            if (hit.collider.GetComponent<PlayerStats>() != null /*&& Vector3.Dot(transform.forward, directionToPlayer.normalized) > 0*/)
            {
                chasePlayer = true;
            }
        }
    }

    private void ChasePlayer()
	{
        if (enemyAttack.type == EnemyAttack.EnemyType.MELEE_FODDER)
        {
            if (!enemyAttack.chargingAttack)
            {
                var dir = (player.transform.position - transform.position).normalized;
                var temp = playerCollider.bounds.ClosestPoint(transform.position);
                temp -= dir * (basicAttackDistance - 2);
                agent.destination = temp;
                if (!agent.updateRotation)
                {
                    agent.updateRotation = true;
                }
            }
			else if (agent.updateRotation)
			{
                agent.updateRotation = false;
			}
        }
        else if (enemyAttack.type == EnemyAttack.EnemyType.RANGED_FODDER)
        {
            if (!enemyAttack.turretMode)
            {
                DistanceAndAknowledgementTracker();
                if (runawayTimer < 0 && !enemyAttack.chargingAttack)
                {
                    //the desired distance away from the player to shoot hit                    
                    if (!isFlying)
                    {
                        if (Vector3.Distance(playerPos, transform.position) > basicAttackDistance)
                        {
                            FindPath(playerPos + ((transform.position - playerPos).normalized * basicAttackDistance));
						}
						else if(Vector3.Distance(playerPos, transform.position) < enemyAttack.minDistanceFromPlayer)
						{
                            FindPath(playerPos + ((transform.position - playerPos).normalized * basicAttackDistance));
                        }
                    }
                    else
                    {
                        FindPath(playerPos + ((enemyTransform.position - playerPos).normalized * basicAttackDistance));
                    }
                }
            }
        }
        else if (enemyAttack.type == EnemyAttack.EnemyType.SHIELD_COMBATANT)
        {
            DistanceAndAknowledgementTracker();
            if (runawayTimer < 0) //if I can move
            {
                //the desired distance away from the player to shoot hit
                FindPath(transform.position + ((playerPos - transform.position) * 0.5f));
            }
        }
        else if (enemyAttack.type == EnemyAttack.EnemyType.JUMPER_COMBATANT)
        {
            DistanceAndAknowledgementTracker();
            if (runawayTimer < 0) //if I can move
            {
                //the desired distance away from the player to shoot hit
                FindPath(transform.position + ((playerPos - transform.position) * 0.5f));
            }
        }
        else if (enemyAttack.type == EnemyAttack.EnemyType.WELL_ENEMY)
        {
            DistanceAndAknowledgementTracker();
            if (Vector3.Distance(enemyAttack.well.lineStart.position, player.transform.position) < enemyAttack.tetherRange)
            {
                FindPath(playerPos);
                tether.InRange();
            }
            else
            {
                FindPath(enemyAttack.well.transform.position + ((player.transform.position - enemyAttack.well.transform.position).normalized * enemyAttack.tetherRange));
                tether.MaxRanged();
            }
        }
        Debug.DrawLine(transform.position, agent.destination, Color.green);
    }
    private void DistanceAndAknowledgementTracker()
	{
        aknowledgementTimer += Time.deltaTime;
        runawayTimer -= Time.deltaTime;
        if (aknowledgementTimer > aknowledgementTimerMax && runawayTimer < 0)
        {
            runawayDistance -= Vector3.Distance(player.transform.position, playerPos);
            playerPos = player.transform.position;
            aknowledgementTimer = 0;
            if (runawayDistance <= 0)
            {
                runawayTimer = runawayCooldown;
                runawayDistance = runawayDistanceMax;
                FindPath(transform.position);
                Debug.Log("Wating");
            }

        }
    }
    private void FollowPatrol()
    {
        enemyAttack.attackMode = false;
        if (agent.enabled && agent.isOnNavMesh)
        {
            if (agent.isOnNavMesh)
            {
                agent.destination = path[pathNode].position;
            }
            if (Mathf.Abs(transform.position.x - path[pathNode].position.x) < 1 &&
               Mathf.Abs(transform.position.z - path[pathNode].position.z) < 1)
            {
                ++pathNode;
                if (pathNode == path.Count)
                {
                    pathNode = 0;
                }
            }
        }
        Debug.DrawLine(transform.position, agent.destination, Color.green);
    }

    private void Knockback()
    {
        
        KnockbackSetup();

        

        Vector3 posToLerpTo = MathHelper.CubicBezier(knockbackPos0
            , knockbackPos1, knockbackPos2, knockbackPos3, knockbackTimer / knockbackDuration);
        Debug.DrawLine(knockbackPos0, knockbackPos1);
        Debug.DrawLine(knockbackPos1, knockbackPos2);
        Debug.DrawLine(knockbackPos2, knockbackPos3);

        transform.position = posToLerpTo;

        knockbackTimer += Time.deltaTime;

        if (knockbackTimer >= knockbackDuration)
        {
            gettingKnockbacked = false;
            knockbackTimer = 0f;
            agent.enabled = true;
            knockbackSetUp = false;
            if (hitwall)
            {
                GetComponent<EnemyStats>().Hurt(wallsmashDamage);
                hitwall = false;
            }
            return;
        }
    }

    void KnockbackSetup()
    {

        if (!knockbackSetUp)
        {
            Debug.Log("Called");
            knockbackPos0 = transform.position;
            knockbackPos1 = transform.position;
            knockbackPos1.y += knockbackHeight;
            knockbackPos2 = transform.position + (knockbackDirection.normalized * knockbackDistance);
            knockbackPos2.y += knockbackHeight;
            knockbackPos3 = transform.position + (knockbackDirection.normalized * knockbackDistance);
            


            if (Physics.Raycast(knockbackPos0, (knockbackPos1 - knockbackPos0).normalized,
                out RaycastHit hit,
                Vector3.Distance(knockbackPos0, knockbackPos1),
                knockbackMask))
            {
                
                knockbackPos1 = GetComponent<Collider>().ClosestPointOnBounds(hit.point);
                knockbackPos2 = GetComponent<Collider>().ClosestPointOnBounds(hit.point);
                knockbackPos3 = GetComponent<Collider>().ClosestPointOnBounds(hit.point);
                hitwall = true;
            }
            else if (Physics.Raycast(knockbackPos1,
                (knockbackPos2 - knockbackPos1).normalized,
                out RaycastHit hit1,
                Vector3.Distance(knockbackPos1, knockbackPos2), 
                knockbackMask))
            {
                knockbackPos2 = GetComponent<Collider>().ClosestPointOnBounds(hit1.point);
                knockbackPos3 = GetComponent<Collider>().ClosestPointOnBounds(hit1.point);
                hitwall = true;
            }
            else if (Physics.Raycast(knockbackPos2,
                (knockbackPos3 - knockbackPos2).normalized,
                out RaycastHit hit2,
                Vector3.Distance(knockbackPos2, knockbackPos3),
                knockbackMask))
            {                
                knockbackPos3 = GetComponent<Collider>().ClosestPointOnBounds(hit2.point);
                hitwall = true;
            }

            //Instantiate(test, knockbackPos3, Quaternion.identity);
            knockbackSetUp = true;

        }
    }

}
